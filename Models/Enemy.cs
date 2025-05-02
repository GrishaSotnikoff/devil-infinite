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

            //rotate towards the player
            var lookAt = _player.GlobalPosition - GlobalPosition;
            lookAt.Y = 0; // Keep the enemy upright
            if (lookAt.LengthSquared() > 0)
            {
                var rotation = lookAt.AngleTo(lookAt); 
                Rotation = new Vector3(0, rotation, 0);
            }
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
