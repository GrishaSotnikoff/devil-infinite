using Godot;
using System;

public partial class Enemy : StaticBody3D, IDamageable
{
    [Export] public float Speed = 2.5f;
    [Export] public int Damage = 10;
    private int _health = 20;
    private Player _player;
    public EnemySpawner Spawner { get; set; }
    private AnimationPlayer _animPlayer;
    private string[] _animations;

    public override void _Ready()
    {
        // existing setup…
        _player = GetTree().CurrentScene.GetNode<Player>("Player");
        GD.Print($"[Enemy] Target is {_player.Name}");
        _animPlayer = GetNode<AnimationPlayer>("HandsAnimation");
        _animations = _animPlayer.GetAnimationList();
        GD.Print($"[Enemy] Animations: {string.Join(", ", _animations)}");
        // Hook the hitbox
        var hitbox = GetNode<Area3D>("Hitbox");
        hitbox.BodyEntered += OnHitboxBodyEntered;
        GD.Print($"[Enemy] Ready");
    }

    private void OnHitboxBodyEntered(Node3D body)
    {
        if (body is Player player)
        {
            //_animPlayer.Play(_animations[2]); play animation here
            GD.Print($"[Enemy] {Name} touched {player.Name}, dealing {Damage} damage");
            player.HP -= Damage;
        }
    }
    public override void _PhysicsProcess(double delta)
    {
        if (_player == null) return;
        _animPlayer.Play(_animations[1]);
        // 1️⃣ Chase the player
        Vector3 direction = (_player.GlobalPosition - GlobalPosition).Normalized();
        GlobalPosition += direction * Speed * (float)delta;

        // 2️⃣ Rotate to face the player horizontally
        Vector3 target = _player.GlobalPosition;
        target.Y = GlobalPosition.Y ;  // Keep the enemy upright
        target.X = target.Z = 0; // Ignore Y axis for rotation
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
