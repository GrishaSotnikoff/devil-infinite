using Godot;
using System;
namespace DevilInfinite.Core;

/// <summary>
/// Basic weapon with hitscan and muzzle flash, adds logging.
/// </summary>
public partial class Weapon : Node3D
{
    [Export] public int Damage = 10;
    [Export] public float Range = 1000f;
    [Export] public float FlashDuration = 0.05f;

    private OmniLight3D _muzzleFlash;
    private Timer _flashTimer;
    private Camera3D _camera;

    public override void _Ready()
    {
        _muzzleFlash = GetNode<OmniLight3D>("MuzzleFlash");
        _camera = GetParent<Camera3D>();

        _flashTimer = new Timer
        {
            WaitTime = FlashDuration,
            OneShot = true
        };
        _flashTimer.Timeout += () => _muzzleFlash.Visible = false;
        AddChild(_flashTimer);
        GD.Print("[Weapon] Ready");
    }

    public void Fire()
    {
        GD.Print("[Weapon] Fire");
        _muzzleFlash.Visible = true;
        _flashTimer.Start();

        var from = _camera.GlobalTransform.Origin;
        var to = from + _camera.GlobalTransform.Basis.Z * -Range;

        var space = GetWorld3D().DirectSpaceState;
        var result = space.IntersectRay(PhysicsRayQueryParameters3D.Create(from, to));
        if (result.Count > 0)
        {
            if (result.TryGetValue("collider", out var col) && typeof(Node).IsAssignableFrom(col.GetType()))
            {
                var hit = (Node)col;
                GD.Print($"[Weapon] Hit {hit.Name}");
                if (hit.HasMethod("TakeDamage"))
                    hit.Call("TakeDamage", Damage);
            }
        }
    }

    public void Reload()
    {
        GD.Print("[Weapon] Reload");
        // implement reload logic if needed
    }
}
