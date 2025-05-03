using Godot;
using System;
using System.Collections.Generic;

public partial class VFXManager : Node
{
    // A dictionary to store named VFX PackedScenes
    private Dictionary<string, PackedScene> _vfxLibrary = new();

    public override void _Ready()
    {
        GD.Print("[VFXManager] Ready");

        // Preload and register your VFX here
        RegisterVFX("Explosion", GD.Load<PackedScene>("res://Scenes/ExplosionEffect.tscn"));
        RegisterVFX("LightningImpact", GD.Load<PackedScene>("res://Scenes/VFX/LightningImpactEffect.tscn"));
        // Add more as needed
    }

    /// <summary>
    /// Registers a named VFX scene.
    /// </summary>
    public void RegisterVFX(string name, PackedScene scene)
    {
        if (scene == null)
        {
            GD.PrintErr($"[VFXManager] Tried to register null VFX: {name}");
            return;
        }

        _vfxLibrary[name] = scene;
        GD.Print($"[VFXManager] Registered VFX: {name}");
    }

    /// <summary>
    /// Spawns a named VFX at the given position and auto-frees it.
    /// </summary>
    public void Spawn(string vfxName, Vector3 position)
    {
        if (!_vfxLibrary.ContainsKey(vfxName))
        {
            GD.PrintErr($"[VFXManager] VFX not found: {vfxName}");
            return;
        }

        var scene = _vfxLibrary[vfxName];
        if (scene == null)
        {
            GD.PrintErr($"[VFXManager] Scene for VFX {vfxName} is null.");
            return;
        }

        var vfxInstance = scene.Instantiate<Node3D>();
        vfxInstance.GlobalTransform = new Transform3D(Basis.Identity, position);
        GetTree().CurrentScene.AddChild(vfxInstance);

        GD.Print($"[VFXManager] Spawned VFX: {vfxName} at {position}");
    }
}
