// Player.cs
using Godot;
using System;

public partial class Player : CharacterBody3D
{
    [Export] public float Speed = 5.0f;
    [Export] public float MouseSensitivity = 0.2f;

    private float _rotationX = 0.0f;
    private Camera3D _camera;
    private Weapon _weapon;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        _camera = GetNode<Camera3D>("Camera3D");
        _weapon = _camera.GetNode<Weapon>("Weapon");
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 direction = Vector3.Zero;

        if (Input.IsActionPressed("move_forward")) direction -= Transform.Basis.Z;
        if (Input.IsActionPressed("move_backward")) direction += Transform.Basis.Z;
        if (Input.IsActionPressed("move_left")) direction -= Transform.Basis.X;
        if (Input.IsActionPressed("move_right")) direction += Transform.Basis.X;

        direction = direction.Normalized();
        Velocity = direction * Speed;
        MoveAndSlide();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            RotateY(Mathf.DegToRad(-mouseMotion.Relative.X * MouseSensitivity));
            _rotationX -= mouseMotion.Relative.Y * MouseSensitivity;
            _rotationX = Mathf.Clamp(_rotationX, -90f, 90f);
            _camera.RotationDegrees = new Vector3(_rotationX, 0, 0);
        }

        if (@event is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Left && mb.Pressed)
        {
            _weapon.Fire();
        }

        if (@event.IsActionPressed("ui_cancel"))
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
    }
}
