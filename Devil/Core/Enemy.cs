using DevilInfinite.Core;
using DevilInfinite.Core.Scripts;
using DevilInfinite.Devil.Interfaces;
using Godot;
using System;

namespace DevilInfinite.Core
{
    public partial class Enemy : CharacterBody3D, IDamageable
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
            SetPhysicsProcess(true);

            _player = GetTree().CurrentScene.GetNode<Player>("Player");
            GD.Print($"[Enemy] Target is {_player.Name}");

            _animPlayer = GetNode<AnimationPlayer>("HandsAnimation");
            _animations = _animPlayer.GetAnimationList();
            GD.Print($"[Enemy] Animations: {string.Join(", ", _animations)}");

            var hitbox = GetNode<Area3D>("Hitbox");
            hitbox.BodyEntered += OnHitboxBodyEntered;

            GD.Print("[Enemy] Ready");
        }

        private void OnHitboxBodyEntered(Node3D body)
        {
            if (body is Player player)
            {
                // _animPlayer.Play(_animations[2]);
                GD.Print($"[Enemy] {Name} touched {player.Name}, dealing {Damage} damage");
                player.HP -= Damage;
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            if (_player == null) return;

            // 1) Compute horizontal direction
            Vector3 dir = (_player.GlobalPosition - GlobalPosition).Normalized();
            dir.Y = 0;

            // 2) Set velocity
            Velocity = dir * Speed;

            // 3) Move and collide against environment
            MoveAndSlide();

            // 4) Rotate to face the player
            Vector3 lookAt = _player.GlobalPosition;
            lookAt.Y = GlobalPosition.Y;
            lookAt.X = lookAt.Z = 0;
            LookAt(lookAt, Vector3.Up);
        }

        public void TakeDamage(int amount)
        {
            if (_player.Mana != 0)
            {
                _health -= amount;
                GD.Print($"[Enemy] {Name} took {amount} damage! Health: {_health}");
            }

            _player.Mana -= amount;

            if (_health <= 0)
            {
                Spawner?.RequestRespawn();
                QueueFree();
            }
        }
    }
}