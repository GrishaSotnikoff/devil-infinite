using Godot;
using System;

public partial class Enemy : StaticBody3D, IDamageable
{
    [Export] public float Speed = 2.5f;
    [Export] public int Damage = 10;    // ← configurable damage
    private int _health = 20;
    private Player _player;
    public EnemySpawner Spawner { get; set; }

    public override void _Ready()
    {
        // existing setup…
        _player = GetTree().CurrentScene.GetNode<Player>("Player");
        GD.Print($"[Enemy] Target is {_player.Name}");

        // Hook the hitbox
        var hitbox = GetNode<Area3D>("Hitbox");
        hitbox.BodyEntered += OnHitboxBodyEntered;
    }

    private void OnHitboxBodyEntered(Node3D body)
    {
        if (body is Player player)
        {
            GD.Print($"[Enemy] {Name} touched {player.Name}, dealing {Damage} damage");
            player.HP -= Damage;
        }
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
        if(_player.Mana != 0){
            _health -= amount;
            GD.Print($"[Enemy] {Name} took {amount} damage! Health: {_health}");
        }


        // Spend 10 mana
        _player.Mana -= amount;
        if (_health <= 0){
            // Ask the spawner to respawn after delay
            if (Spawner != null)
                Spawner.RequestRespawn();
            QueueFree();
        }
    }
}
