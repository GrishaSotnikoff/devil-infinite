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

        // 1️⃣ Chase the player
        Vector3 direction = (_player.GlobalPosition - GlobalPosition).Normalized();
        GlobalPosition += direction * Speed * (float)delta;

        // 2️⃣ Rotate to face the player horizontally
        Vector3 target = _player.GlobalPosition;
        target.Y = GlobalPosition.Y ;  // Keep the enemy upright
                                       // Rotate the enemy to face the player

        LookAt(target , Vector3.Up);
    }


    public void TakeDamage(int amount)
    {
        _health -= amount;
        GD.Print($"[Enemy] {Name} took {amount} damage! Health: {_health}");
        if (_health <= 0)
            QueueFree();
    }
}
