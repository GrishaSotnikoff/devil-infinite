using Godot;
using System;

public partial class Nail : RigidBody3D
{
    [Export] public float Speed = 40f;
    public Vector3 Direction = Vector3.Forward; // override when instancing

    public override void _Ready()
    {
        // Fly straight
        GravityScale = 0;
        LinearVelocity = Direction * Speed;

        // Auto-free after 2 seconds
        GetTree().CreateTimer(2.0).Timeout += () => QueueFree();
    }

    private void OnBodyEntered(Node3D body)
    {
        // e.g. apply damage here...
        QueueFree();
    }
}
