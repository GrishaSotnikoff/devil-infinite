using Godot;
using System;

/// <summary>
/// Simple enemy that chases the player and takes damage, with logging.
/// </summary>
public partial class Enemy : StaticBody3D
{
    [Export] public float Speed = 2.5f;
    private int _health = 20;
    private Player _player;

    public override void _Ready()
    {
        var root = GetTree().GetCurrentScene();
        _player = root.GetNode<Player>("Player");
        GD.Print($"[Enemy] {_player.Name} target acquired");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_player != null)
        {
            var direction = (_player.GlobalPosition - GlobalPosition).Normalized();
            GlobalPosition += direction * Speed * (float)delta;
            //GD.Print($"[Enemy] Moving by {direction * Speed * (float)delta}");
        }
    }

    public void TakeDamage(int amount)
    {
        _health -= amount;
        GD.Print($"[Enemy] {Name} took {amount} damage! Health: {_health}");
        if (_health <= 0)
            QueueFree();
    }
}
