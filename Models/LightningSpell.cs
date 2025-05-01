using Godot;
using System;

/// <summary>
/// Lightning bolt spell, with logging.
/// </summary>
public partial class LightningSpell : SpellBase
{
	[Export] public PackedScene LightningBolt;
	[Export] public float BoltRange = 100f;

	public override void Cast()
	{
		if (!_canCast) return;
		GD.Print("[LightningSpell] Cast");
		StartCooldown();
		var camera = GetParent<Camera3D>();
		var origin = camera.GlobalTransform.Origin;
		var to = origin + camera.GlobalTransform.Basis.Z * -BoltRange;
		var space = GetWorld3D().DirectSpaceState;
		var result = space.IntersectRay(PhysicsRayQueryParameters3D.Create(origin, to));

		if (result.Count > 0 && result.TryGetValue("collider", out var col)  && typeof(Node).IsAssignableFrom(col.GetType()))

		{

			var hit = (Node)col;
			var bolt = LightningBolt.Instantiate<Node3D>();
			bolt.GlobalTransform = new Transform3D(Basis.Identity, (Vector3)result["position"]);
			GetTree().CurrentScene.AddChild(bolt);
			if (hit.HasMethod("TakeDamage"))
				hit.Call("TakeDamage", 15);
		}
	}

	public override void CastAlternative()
	{
		GD.Print("[LightningSpell] Alternative cast");
		Cast();
	}
}
