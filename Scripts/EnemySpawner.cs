using Godot;
using System;

public partial class EnemySpawner : Node3D
{
    [Export] public PackedScene EnemyScene;
    [Export] public float RespawnDelay = 5f;

    private Enemy _currentEnemy;

    public override void _Ready()
    {
        SpawnEnemy();
    }

    public void SpawnEnemy()
    {
        if (EnemyScene == null)
        {
            GD.PrintErr("[EnemySpawner] No EnemyScene assigned!");
            return;
        }

        // 1) Instantiate without generic cast
        var inst = EnemyScene.Instantiate();
        if (!(inst is Node3D node))
        {
            GD.PrintErr("[EnemySpawner] EnemyScene root is not a Node3D!");
            return;
        }

        // 2) Place at exactly this spawner's transform
        node.GlobalTransform = this.GlobalTransform;
        // 3) Add to the current scene
        AddChild(node);

        // 4) Wire up the spawner reference if it really is an Enemy
        if (node is Enemy enemy)
        {
            enemy.Spawner = this;
            _currentEnemy = enemy;
        }
        else
        {
            GD.PrintErr($"[EnemySpawner] Instanced scene is not Enemy but {node.GetType().Name}");
        }

        GD.Print($"[EnemySpawner] Spawned enemy at {node.GlobalPosition}");
    }

    public void RequestRespawn()
    {
        GD.Print($"[EnemySpawner] Scheduling respawn in {RespawnDelay}s");
        GetTree()
            .CreateTimer(RespawnDelay)
            .Timeout += () => SpawnEnemy();
    }
}
