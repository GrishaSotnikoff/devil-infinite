using Godot;
using System;

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

        _flashTimer = new Timer();
        _flashTimer.WaitTime = FlashDuration;
        _flashTimer.OneShot = true;
        _flashTimer.Timeout += () => _muzzleFlash.Visible = false;
        AddChild(_flashTimer);
    }

    public void Fire()
    {
        _muzzleFlash.Visible = true;
        _flashTimer.Start();

        var from = _camera.GlobalTransform.Origin;
        var to = from + _camera.GlobalTransform.Basis.Z * -Range;
        GD.Print("Weapon fired");
        var space = GetWorld3D().DirectSpaceState;
        var result = space.IntersectRay(PhysicsRayQueryParameters3D.Create(from, to));
        if (result.Count > 0)
        {
            var hit = result["collider"].AsGodotObject();
            GD.Print($"[Weapon] Hit {hit}");
            if (hit.HasMethod("TakeDamage"))
                hit.Call("TakeDamage", Damage);
        }
    }
}
