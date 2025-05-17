using DevilInfinite.Devil.Interfaces;
using Godot;
using System;
namespace DevilInfinite.Core;

/// <summary>
/// Base class for spells, handles cooldown with logging.
/// </summary>
public abstract partial class SpellBase : Node3D, ISpell
{
    [Export] public float Cooldown = 0.5f;
    protected bool _canCast = true;


    public override void _Ready()
    {

        SetProcess(true);
        GD.Print("[SpellBase] Ready");
    }
    public override void _Process(double delta) { }

    protected async void StartCooldown()
    {
        _canCast = false;
        GD.Print($"[SpellBase] Cooldown started for {Cooldown} seconds");
        await ToSignal(GetTree().CreateTimer(Cooldown), "timeout");
        _canCast = true;
        GD.Print("[SpellBase] Cooldown ended");
    }

    public abstract void Cast();
    public abstract void CastAlternative();
}
