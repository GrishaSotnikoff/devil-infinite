using Godot;
using System;

/// <summary>
/// Handles player movement, camera control, jumping (with double‐jump), dashing,
/// and HP/Mana stats with UI updates.
/// </summary>
public partial class Player : CharacterBody3D
{
	// Movement & look
	[Export] public float Speed = 5.0f;
	[Export] public float MouseSensitivity = 0.2f;
	[Export] public float AnimationSpeed { get; set; } = 5f;

	// Jumping & gravity
	[Export] public float JumpVelocity = 5.0f;
	[Export] public float Gravity = -9.8f;
	[Export] public int MaxJumps = 2;  // allow up to double‐jump

	// Dashing
	[Export] public float DashMultiplier = 2.0f;
	[Export] public float DashDuration = 1.0f;
	private float _dashTimer = 0f;

	// Health & mana
	[Signal] public delegate void StatsChangedEventHandler(int hp, int mana);
	[Export] public int MaxHP = 100;
	[Export] public int MaxMana = 100;
	private int _hp;
	private int _mana;

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

	// Internal state
	private int _jumpsUsed = 0;
	private float _rotationX = 0.0f;
	private Camera3D _camera;
	private AnimationPlayer _animPlayer;

	public override void _Ready()
	{
		// Enable physics processing
		SetPhysicsProcess(true);
		GD.Print("[Player] Ready");

		Input.MouseMode = Input.MouseModeEnum.Captured;
		_camera = GetNode<Camera3D>("Camera3D");
		_animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		// Initialize stats
		_hp = MaxHP;
		_mana = MaxMana;
		EmitSignal(nameof(StatsChanged), _hp, _mana);
	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;

		// 0) Reset jumps when grounded
		if (IsOnFloor())
		{
			_jumpsUsed = 0;
		}

		// 1) Handle dash timing
		if (Input.IsActionJustPressed("dash") && _dashTimer <= 0f)
		{
			_dashTimer = DashDuration;
			GD.Print("[Player] Dash started");
		}
		if (_dashTimer > 0f)
		{
			_dashTimer -= dt;
			if (_dashTimer <= 0f)
			{
				_dashTimer = 0f;
				GD.Print("[Player] Dash ended");
			}
		}

		// 2) Read horizontal input
		Vector3 dir = Vector3.Zero;
		if (Input.IsActionPressed("move_forward")) dir -= Transform.Basis.Z;
		if (Input.IsActionPressed("move_backward")) dir += Transform.Basis.Z;
		if (Input.IsActionPressed("move_left")) dir -= Transform.Basis.X;
		if (Input.IsActionPressed("move_right")) dir += Transform.Basis.X;
		Vector3 horizontalVel = dir.Normalized() * Speed * (_dashTimer > 0f ? DashMultiplier : 1f);

		// 3) Manage vertical velocity
		Vector3 vel = Velocity;
		if (Input.IsActionJustPressed("jump") && _jumpsUsed < MaxJumps)
		{
			vel.Y = JumpVelocity;
			_jumpsUsed++;
			GD.Print(_jumpsUsed == 1 ? "[Player] Jump" : "[Player] Double Jump");
		}
		else if (IsOnFloor())
		{
			// ensure we stick when not mid‐jump
			vel.Y = 0;
		}
		else
		{
			vel.Y += Gravity * dt;
		}

		// 4) Combine velocities & move
		vel.X = horizontalVel.X;
		vel.Z = horizontalVel.Z;
		Velocity = vel;
		MoveAndSlide();

		// 5) Debug movement
		if (dir != Vector3.Zero)
			GD.Print($"[Player] Moving: {Velocity}");
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
		}
	}
}
