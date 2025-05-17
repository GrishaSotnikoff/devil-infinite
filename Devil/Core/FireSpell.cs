using Godot;
using System;
using System.Collections.Generic;
namespace DevilInfinite.Core;

/// <summary>
/// Shotgun default + rocket alternative fire spell, with logging.
/// </summary>
public partial class FireSpell : SpellBase
{
	[Export] public int DefaultPellets = 8;
	[Export] public float DefaultSpread = 15f;
	[Export] public int ShotgunDamage = 8;
	[Export] public PackedScene RocketProjectile;
	[Export] public float ChargeTimeMax = 2.0f;
	private string[] _animations;
	private int _currentIndex = 0;
	private float _chargeTimer = 0f;
	private bool _charging = false;

	public override void Cast()
	{
		GD.Print("[FireSpell] Cast default");
		StartCooldown();
		var camera = GetParent<Camera3D>();
		var origin = camera.GlobalTransform.Origin;
		var basis = camera.GlobalTransform.Basis;
		RocketProjectile.Instantiate();
		for (int i = 0; i < DefaultPellets; i++)
		{
			float angle = (GD.Randf() - 0.5f) * DefaultSpread;
			var dir = basis.Z.Rotated(Vector3.Up, Mathf.DegToRad(angle)) * -1;
			var space = GetWorld3D().DirectSpaceState;
			var result = space.IntersectRay(PhysicsRayQueryParameters3D.Create(origin, origin + dir * 50f));

			if (result.Count > 0 && result.TryGetValue("collider", out var col)  && typeof(Node).IsAssignableFrom(col.GetType()))

			{

				var hit = (Node)col;
				if (hit.HasMethod("TakeDamage"))
					hit.Call("TakeDamage", ShotgunDamage);
			}
		}
	}

	public override void CastAlternative()
	{
		if (!_charging && _canCast)
		{
			GD.Print("[FireSpell] Charge start");
			_charging = true;
			_chargeTimer = 0f;
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		SetProcess(true);

		if (_charging)
		{
			_chargeTimer += (float)delta;
			if (_chargeTimer >= ChargeTimeMax)
				_chargeTimer = ChargeTimeMax;
		}
	}

	public void ReleaseCharge()
	{
		if (!_charging || !_canCast) return;

		float chargeRatio = Mathf.Clamp(_chargeTimer / ChargeTimeMax, 0f, 1f);
		GD.Print($"[FireSpell] Release charge → ratio {chargeRatio:F2}");

		_charging = false;
		StartCooldown();

		// 1. Compute spawn transform
		var camera = GetParent<Camera3D>();
		var origin = camera.GlobalTransform.Origin;
		var basis = camera.GlobalTransform.Basis;
		var spawnPos = origin + (basis.Z * -1) * 1.0f;

		// 2. Instantiate the **correct** packed scene
		var inst = RocketProjectile.Instantiate<Node3D>();
		inst.GlobalTransform = new Transform3D(basis, spawnPos);
		GetTree().CurrentScene.AddChild(inst);

		// 3. Cast to your scripted type and launch
		if (inst is RocketProjectile3D proj)
			proj.Launch(chargeRatio);
		else
			GD.PrintErr($"[FireSpell] Expected RocketProjectile3D but got {inst.GetType().Name}");
	}


}
