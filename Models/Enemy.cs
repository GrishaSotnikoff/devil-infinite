using Godot;
using System;

public partial class Enemy : StaticBody3D, IDamageable
{
    [Export] public float Speed = 2.5f;
    private int _health = 20;
    private Player _player;

    public override void _Ready()
    {
        _player = GetTree().CurrentScene.GetNode<Player>("Player");
        GD.Print($"[Enemy] {_player.Name} target acquired");
    }


    public override void _PhysicsProcess(double delta)
    {
        if (_player == null) return;

        // chase
        var dir = (_player.GlobalPosition - GlobalPosition).Normalized();
        GlobalPosition += dir * Speed * (float)delta;

        // look rotation (fixed; the original AngleTo was wrong)
        var lookAt = _player.GlobalPosition - GlobalPosition;
        lookAt.Y = 0;
        if (lookAt != Vector3.Zero)
            LookAt(GlobalPosition + lookAt, Vector3.Up);
    }

    public void TakeDamage(int amount)
    {
        _health -= amount;
        GD.Print($"[Enemy] {Name} took {amount} damage! Health: {_health}");
        if (_health <= 0)
            QueueFree();
    }
}
