using Godot;
using System;


public partial class Enemy : StaticBody3D
{
    [Export] public float Speed = 2.5f;
    private int _health = 20;
    private Player _player;

    public override void _Ready()
    {
        // Get the current scene's Player node
        var root = GetTree().GetCurrentScene();
        _player = root.GetNode<Player>("Player");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_player != null)
        {
            var direction = (_player.GlobalTransform.Origin - GlobalTransform.Origin).Normalized();
            GlobalTranslate(direction * Speed * (float)delta);
        }
    }

    public void TakeDamage(int amount)
    {
        _health -= amount;
        GD.Print($"{Name} took {amount} damage! Health: {_health}");
        if (_health <= 0)
            QueueFree();
    }
}
