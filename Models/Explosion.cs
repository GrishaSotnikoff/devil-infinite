using Godot;
using System;

public partial class Explosion : Node3D
{
    [Export] public float Lifetime = 1.0f;

    public override void _Ready()
    {
        // Recursively find and emit every GPUParticles3D in this subtree
        EmitAllParticles(this);

        // Schedule auto-free
        GetTree()
            .CreateTimer(Lifetime)
            .Timeout += () => QueueFree();
    }

    private void EmitAllParticles(Node node)
    {
        foreach (var child in node.GetChildren())
        {
            if (child is GpuParticles3D p)
            {
                p.OneShot = true;
                p.Emitting = true;
                p.Restart();
                GD.Print($"[Explosion] Emitting particles on {p.Name}");
            }
            else if (child is Node n)
            {
                EmitAllParticles(n);
            }
        }
    }
}
