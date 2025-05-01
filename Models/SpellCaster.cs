using Godot;
using System;

/// <summary>
/// Routes mapped input actions to individual spell components, with logging.
/// </summary>
public partial class SpellCaster : Node3D
{
	private FireSpell _fireSpell;
	private LightningSpell _lightningSpell;
	private Parry _parry;

	public override void _Ready()
	{
		SetProcess(true);
		var player = GetParent<Player>();
		if (player == null)
		{
			GD.PushWarning("SpellCaster must be a direct child of Player node.");
			return;
		}

		var camera = player.GetNode<Camera3D>("Camera3D");
		_fireSpell = camera.GetNode<FireSpell>("FireSpell");
		_lightningSpell = camera.GetNode<LightningSpell>("LightningSpell");
		_parry = player.GetNode<Parry>("Parry");
		GD.Print("[SpellCaster] Ready");
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("shoot"))
		{
			GD.Print("[SpellCaster] FireDefault");
			_fireSpell.Cast();
		}

		if (Input.IsActionJustPressed("alt_fire"))
		{
			GD.Print("[SpellCaster] FireChargeStart");
			_fireSpell.CastAlternative();
		}
		if (Input.IsActionJustReleased("alt_fire"))
		{
			GD.Print("[SpellCaster] FireChargeRelease");
			_fireSpell.ReleaseCharge();
		}

		if (Input.IsActionJustPressed("lightning"))
		{
			GD.Print("[SpellCaster] CastLightning");
			_lightningSpell.Cast();
		}

		if (Input.IsActionJustPressed("parry"))
		{
			GD.Print("[SpellCaster] AttemptParry");
			_parry.AttemptParry();
		}
	}
}
