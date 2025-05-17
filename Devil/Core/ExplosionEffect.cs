using Godot;
namespace DevilInfinite.Core;

public partial class ExplosionEffect : Node3D
{
    [Export] public float Lifetime = 0.6f;

    public override void _Ready()
    {
        // 1) Turn on the particle burst manually
        var particles = GetNode<CpuParticles3D>("CPUParticles3D");
        particles.Emitting = true;

        // 2) (Optional) restart in case it was already emitted
        particles.Restart();

        // 3) Auto-free this node after Lifetime seconds
        GetTree()
            .CreateTimer(Lifetime)
            .Timeout += () => QueueFree();
    }
}
