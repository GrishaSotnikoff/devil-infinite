using Godot;
using System;

/// <summary>
/// Handles player movement, camera control, jumping, and HP/Mana stats with UI updates.
/// </summary>
public partial class Player : CharacterBody3D
{
	// Movement & look
	[Export] public float Speed = 5.0f;
	[Export] public float MouseSensitivity = 0.2f;

	// Jumping & gravity
	[Export] public float JumpVelocity = 5.0f;
	[Export] public float Gravity = -9.8f;

	// Health & mana
	[Signal] public delegate void StatsChangedEventHandler(int hp, int mana);
	[Export] public int MaxHP = 100;
	[Export] public int MaxMana = 100;

	private int _hp ;
	private int _mana;

	public int HP
	{
		get => _hp;
		set
		{
			GD.Print("[Player] Jump");

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
	private float _rotationX = 0.0f;
	private Camera3D _camera;
	private bool _isJumping = false;

	public override void _Ready()
	{
		// Enable physics processing
		SetPhysicsProcess(true);
		GD.Print("[Player] Ready");

		// Capture the mouse for look controls
		Input.MouseMode = Input.MouseModeEnum.Captured;
		_camera = GetNode<Camera3D>("Camera3D");

		// Initialize stats
		_hp = MaxHP;
		_mana = MaxMana;
		EmitSignal(nameof(StatsChangedEventHandler), _hp, _mana);
	}

	public override void _PhysicsProcess(double delta)
	{
		// 1) Read horizontal input
		Vector3 dir = Vector3.Zero;
		if (Input.IsActionPressed("move_forward")) dir -= Transform.Basis.Z;
		if (Input.IsActionPressed("move_backward")) dir += Transform.Basis.Z;
		if (Input.IsActionPressed("move_left")) dir -= Transform.Basis.X;
		if (Input.IsActionPressed("move_right")) dir += Transform.Basis.X;
		Vector3 horizontalVel = dir.Normalized() * Speed;

		// 2) Manage vertical velocity via a local copy
		Vector3 vel = Velocity;
		if (IsOnFloor())
		{
			if (Input.IsActionJustPressed("jump"))
			{
				vel.Y = JumpVelocity;
				GD.Print("[Player] Jump");
				_hp = Mathf.Clamp(HP + 1, 0, MaxHP); // Example: reduce HP on jump
				_isJumping = true;
			}
			else if (!_isJumping)
			{
				// Keep grounded when not jumping
				vel.Y = 0;
			}
		}
		else
		{
			// In air: apply gravity
			vel.Y += Gravity * (float)delta;
		}

		// 3) Combine horizontal and vertical, then assign back
		vel.X = horizontalVel.X;
		vel.Z = horizontalVel.Z;
		Velocity = vel;

		// 4) Move and slide
		MoveAndSlide();

		// 5) Landing detection
		if (_isJumping && IsOnFloor())
		{
			GD.Print("[Player] Landed");
			_isJumping = false;
		}

		// 6) Debug movement
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
