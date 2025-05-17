using DevilInfinite.Core;
using DevilInfinite.Core.Scripts;
using DevilInfinite.Devil.Core.Common;
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
        [Export] public NodePath AgentPath;
        private NavigationAgent3D _agent;

        public override void _Ready()
        {
            SetPhysicsProcess(true);
            _agent = GetNode<NavigationAgent3D>("NavigationAgent3D");
            _player = GetTree().CurrentScene.GetNode<Player>("Player");


            var mapRid = GetWorld3D().NavigationMap;
            GlobalPosition = NavigationServer3D.MapGetClosestPoint(mapRid, GlobalPosition);

            // Snap the initial target as well
            _agent.TargetPosition = NavigationServer3D.MapGetClosestPoint(mapRid, _player.GlobalPosition);
            _animPlayer = GetNode<AnimationPlayer>("HandsAnimation");
            _animations = _animPlayer.GetAnimationList();
            GD.Print($"[Enemy] Animations: {string.Join(", ", _animations)}");

            var hitbox = GetNode<Area3D>("Hitbox");
            hitbox.BodyEntered += OnHitboxBodyEntered;
            Logger.Log("Ready");
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
            if (_player == null)
                return;

            // 1) Update agent target if the player has moved
            if (_agent.TargetPosition.DistanceTo(_player.GlobalPosition) > 0.5f)
                _agent.TargetPosition = _player.GlobalPosition;

            // 2) Ensure there’s a path
            if (_agent.IsNavigationFinished())
            {
                // no path or already at target
                Velocity = Vector3.Zero;
                MoveAndSlide();
                return;
            }

            // 3) Get the next waypoint
            Vector3 nextPoint = _agent.GetNextPathPosition();

            // 4) Compute horizontal direction
            Vector3 dir = nextPoint - GlobalPosition;
            dir.Y = 0;
            if (dir.LengthSquared() < 0.01f)
            {
                Velocity = Vector3.Zero;
                MoveAndSlide();
                return;
            }
            dir = dir.Normalized();

            // 5) Move & slide (physics collisions)
            Velocity = dir * Speed;
            MoveAndSlide();

            // 6) Rotate to face motion
            LookAt(GlobalPosition + dir, Vector3.Up);
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
