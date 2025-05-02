using Godot;
using System;

/// <summary>
/// Lightning bolt spell, with logging.
/// </summary>
public partial class LightningSpell : SpellBase
{
	[Export] public PackedScene LightningBolt;
	[Export] public float BoltRange = 100f;
	[Export] public float Damage = 15f;
	public override void Cast()
	{
		if (!_canCast) return;
		GD.Print("[LightningSpell] Cast");
		StartCooldown();

		var camera = GetParent<Camera3D>();
		var origin = camera.GlobalTransform.Origin;
		var to = origin + camera.GlobalTransform.Basis.Z * -BoltRange;
		var result = GetWorld3D()
					 .DirectSpaceState
					 .IntersectRay(PhysicsRayQueryParameters3D.Create(origin, to));

		if (result.Count > 0
			&& result.TryGetValue("collider", out var col)
			&& col.As<Node>() is Node hit) // Fix: Use As<Node>() to safely cast Variant to Node
		{
			GD.Print($"[LightningSpell] Hit collider: {hit.Name}, Type: {hit.GetType().Name}");

			// Search up the parent tree for IDamageable
			Node damageTarget = hit;
			while (damageTarget != null && !(damageTarget is IDamageable))
				damageTarget = damageTarget.GetParent();

			if (damageTarget is IDamageable dmg)
			{
				GD.Print($"[LightningSpell] Damageable found: {damageTarget.Name}");
				dmg.TakeDamage((int)Damage);
			}
			else
			{
				GD.Print("[LightningSpell] No damageable found.");
			}
		}
	}

	public override void CastAlternative()
	{
		GD.Print("[LightningSpell] Alternative cast");
		Cast();
	}
}
