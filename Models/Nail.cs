using Godot;
using System;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.Mime.MediaTypeNames;

public partial class Nail : RigidBody3D
{
    [Export] public float Speed = 40f;
    [Export] public int Damage = 10;
    public Vector3 Direction = Vector3.Forward; // override when instancing
    [Export] public Vector3 Velocity = new Vector3(0, 0, 0);
    // Name of the VFX registered in VFXManager
    [Export] public PackedScene ImpactVFX;
    public override void _Ready()
    {
        // 1) Enable contact callbacks
        ContactMonitor = true;     // exposes body_entered/body_exited

        // 2) Connect the built-in body_entered signal
        var err = Connect("body_entered",
                          new Callable(this, nameof(OnBodyEntered)));
        if (err != Error.Ok)
            GD.PrintErr($"[Nail] Failed to connect body_entered: {err}");
        else
            GD.Print($"[Nail] Connected body_entered signal");
        // Fly straight
        GravityScale = 0;
        LinearVelocity = Direction * Speed;

        // Auto-free after 2 seconds
        GetTree().CreateTimer(2.0).Timeout += () => QueueFree();
    }

    private void OnBodyEntered(Node body)
    {
        GD.Print($"[Nail] Hit {body.Name}");
        // 3) Spawn the VFX at the collision point
        GD.Print($"[Nail] Calling VFXManager.Spawn(\"{ImpactVFX.ResourceName}\")");
        var vfxInstance = ImpactVFX.Instantiate<Node3D>();
        vfxInstance.GlobalTransform = new Transform3D(Basis.Identity, GlobalTransform.Origin);
        GetTree().CurrentScene.AddChild(vfxInstance);
        vfxInstance.EmitSignal("ready");
        GD.Print($"[Nail] Spawned VFX: {ImpactVFX} at {GlobalTransform.Origin}");
        // 4) Deal damage if the target can take it
        if (body is IDamageable dmg)
            dmg.TakeDamage(Damage);
        // e.g. apply damage here...
        QueueFree();
    }
}
