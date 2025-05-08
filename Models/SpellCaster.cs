using Godot;
using System;
using System.Diagnostics;
using System.Threading;

/// <summary>
/// Routes mapped input actions to individual spell components, with logging.
/// </summary>
public partial class SpellCaster : Node3D
{
	private FireSpell _fireSpell;
	private LightningSpell _lightningSpell;
	private Parry _parry;
	[Export] public NodePath AnimationPlayerPath { get; set; } = "AnimationPlayer";
	private string[] _animations;

	private AnimationPlayer _animPlayer;

	public override void _Ready()
	{
		_animPlayer = GetNode<AnimationPlayer>(AnimationPlayerPath);
		_animations = _animPlayer.GetAnimationList();
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

		}

		if (Input.IsActionJustPressed("alt_fire"))
		{
			GD.Print("[SpellCaster] FireChargeStart");
			_fireSpell.CastAlternative();
		}
		if (Input.IsActionJustReleased("alt_fire"))
		{
			GD.Print("[SpellCaster] FireChargeRelease");
			_fireSpell.Cast();
			_fireSpell.ReleaseCharge();
			
		}

		if (Input.IsActionJustPressed("lightning"))
		{
			GD.Print($"[SpellCaster] Playing animation {_animations[0]}");
			_animPlayer.Play(_animations[0]);
			//stop animation aftere 3 seconds
			
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
