using Godot;
using System;

/// <summary>
/// Handles player movement, camera control, jumping (with double‐jump), dashing,
/// and HP/Mana stats (with adjustable mana regeneration).
/// </summary>
public partial class Player : CharacterBody3D
{
	// Movement & look
	[Export] public float Speed = 5.0f;
	[Export] public float MouseSensitivity = 0.2f;

	// Jumping & gravity
	[Export] public float JumpVelocity = 5.0f;
	[Export] public float Gravity = -9.8f;
	[Export] public int MaxJumps = 2;

	// Dashing
	[Export] public float DashMultiplier = 2.0f;
	[Export] public float DashDuration = 1.0f;
	private float _dashTimer = 0f;

	// Health & mana
	[Signal] public delegate void StatsChangedEventHandler(int hp, int mana);
	[Export] public int MaxHP = 100;
	[Export] public int MaxMana = 100;
	[Export] public float ManaRegenRate = 5f; // mana points per second
	[Export] public PackedScene NailScene;
	[Export] public NodePath MuzzlePath;
	// Internal state
	private int _jumpsUsed = 0;
	private float _rotationX = 0.0f;
	private Camera3D _camera;
	[Export] private AnimationPlayer _animPlayer;
	AudioStreamPlayer2D _runningPlayer;
	private Node3D _muzzle;

	private int _hp;
	private int _mana;
	private float _manaRegenAccumulator = 0f;

	public int HP
	{
		get => _hp;
		set
		{
			_hp = Mathf.Clamp(value, 0, MaxHP);
			EmitSignal(nameof(StatsChanged), _hp, _mana);
		}
	}

	public int Mana
	{
		get => _mana;
		set
		{
			_mana = Mathf.Clamp(value, 0, MaxMana);
			EmitSignal(nameof(StatsChanged), _hp, _mana);
		}
	}


	public override void _Ready()
	{
		SetPhysicsProcess(true);
		GD.Print("[Player] Ready");
		_muzzle = GetNode<Node3D>(MuzzlePath);

		Input.MouseMode = Input.MouseModeEnum.Captured;
		_camera = GetNode<Camera3D>("Camera3D");
		_runningPlayer = GetNode<AudioStreamPlayer2D>("RunningPlayer");
		// Initialize stats
		foreach(var a in _animPlayer.GetAnimationList())
			GD.Print($"Player animations: {a.GetBaseName()}");
		_hp = MaxHP;
		_mana = MaxMana;
		EmitSignal(nameof(StatsChanged), _hp, _mana);
	}
	bool _jumping = false;
	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;

		// 0) Mana regeneration
		if (_mana < MaxMana && ManaRegenRate > 0f)
		{
			_manaRegenAccumulator += ManaRegenRate * dt;
			if (_manaRegenAccumulator >= 1f)
			{
				int toRegen = (int)_manaRegenAccumulator;
				_manaRegenAccumulator -= toRegen;
				Mana += toRegen; // use the property to emit signal
				GD.Print($"[Player] Regenerated {toRegen} mana (now {Mana}/{MaxMana})");
			}
		}

		// 1) Reset jumps when grounded
		if (IsOnFloor())
			_jumpsUsed = 0;

		// 2) Dash logic
		if (Input.IsActionJustPressed("dash") && _dashTimer <= 0f)
		{
			_dashTimer = DashDuration;
			GD.Print("[Player] Dash started");
		}
		if (_dashTimer > 0f)
		{
			_dashTimer -= dt;
			if (_dashTimer <= 0f)
				GD.Print("[Player] Dash ended");
		}

		// 3) Movement input
		Vector3 dir = Vector3.Zero;
		if (Input.IsActionPressed("move_forward")) dir -= Transform.Basis.Z;
		if (Input.IsActionPressed("move_backward")) dir += Transform.Basis.Z;
		if (Input.IsActionPressed("move_left")) dir -= Transform.Basis.X;
		if (Input.IsActionPressed("move_right")) dir += Transform.Basis.X;
		Vector3 horizontalVel = dir.Normalized() * Speed * (_dashTimer > 0f ? DashMultiplier : 1f);

		// 4) Jumping & gravity
		Vector3 vel = Velocity;
		if (Input.IsActionJustPressed("jump") && _jumpsUsed < MaxJumps)
		{
			_jumping = true;
			vel.Y = JumpVelocity;
			_jumpsUsed++;
			GD.Print(_jumpsUsed == 1 ? "[Player] Jump" : "[Player] Double Jump");
		}
		else if (IsOnFloor())
		{
			vel.Y = 0;
		}
		else
		{
			vel.Y += Gravity * dt;
		}

		// 5) Apply movement
		vel.X = horizontalVel.X;
		vel.Z = horizontalVel.Z;
		Velocity = vel;
		MoveAndSlide();

		if (dir != Vector3.Zero)
		{
			if (_runningPlayer.Playing == false)
			{
				_runningPlayer.Play();
			}
			_animPlayer.Play("idleAnimation");
		}
		else {
			  _runningPlayer.Stop();
		}
		//GD.Print($"[Player] Moving: {Velocity}");
	}

	public override void _Input(InputEvent @event)
	{
		// Mouse look
		if (@event is InputEventMouseMotion mm)
		{
			RotateY(Mathf.DegToRad(-mm.Relative.X * MouseSensitivity));
			_rotationX = Mathf.Clamp(_rotationX - mm.Relative.Y * MouseSensitivity, -90f, 90f);
			_camera.RotationDegrees = new Vector3(_rotationX, 0, 0);
		}

		// Release mouse on Esc
		if (Input.IsActionJustPressed("ui_cancel"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
			GD.Print("[Player] Mouse released");
			SceneManager sm = GetTree().Root.GetNode<SceneManager>("SceneManager");
			sm.GoTo("res://Scenes/MainMenu.tscn");
		}

		if (@event.IsActionPressed("shoot"))
			Shoot();
	}

	private void Shoot()
	{
		_animPlayer.Stop();
		// instance the nail
		var nail = NailScene.Instantiate<Nail>();

		// set its starting position
		nail.GlobalTransform = _muzzle.GlobalTransform;

		// **** HERE’S THE KEY: use the camera’s forward direction ****
		// In Godot 3D, “forward” is -Z in the basis
		Vector3 camForward = -_camera.GlobalTransform.Basis.Z;
		nail.Direction = camForward.Normalized();

		// add it to the scene
		GetTree().CurrentScene.AddChild(nail);
	}
}
