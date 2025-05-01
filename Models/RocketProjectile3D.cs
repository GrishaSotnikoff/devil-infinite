using Godot;
using System;
using Godot.Collections;

/// <summary>
/// Lobbed rocket projectile with charge-based launch, gravity arc, and explosion.
/// </summary>
public partial class RocketProjectile3D : Node3D
{
	[Export] public float MinSpeed = 5f;
	[Export] public float MaxSpeed = 20f;
	[Export] public float MinArcAngle = 10f;
	[Export] public float MaxArcAngle = 45f;
	[Export] public float Gravity = -9.8f;
	[Export] public float Damage = 50f;
	[Export] public float ExplosionRadius = 5f;
	[Export] public PackedScene ExplosionEffectScene;

	private Vector3 _velocity;
	private Vector3 _lastPosition;

	public override void _Ready()
	{
		// Enable physics processing so _PhysicsProcess is called
		SetPhysicsProcess(true);
		_lastPosition = GlobalTransform.Origin;
		GD.Print("[RocketProjectile3D] Ready");
	}

	/// <summary>
	/// Launches the rocket with a charge ratio (0–1) that scales speed and arc.
	/// </summary>
	public void Launch(float chargeRatio)
	{
		// Determine initial speed and arc angle
		float speed = Mathf.Lerp(MinSpeed, MaxSpeed, chargeRatio);
		float arcAngle = Mathf.Lerp(MinArcAngle, MaxArcAngle, chargeRatio);

		// Compute launch direction tilted upward by arcAngle
		var forward = GlobalTransform.Basis.Z * -1;
		var dir = forward.Rotated(forward.Cross(Vector3.Up),
								   Mathf.DegToRad(arcAngle)).Normalized();
		_velocity = dir * speed * 10;

		GD.Print($"[RocketProjectile3D] Launched with speed {speed:F2} and angle {arcAngle:F1}°");
	}

	public override void _PhysicsProcess(double delta)
	{
		// Apply gravity each physics frame
		_velocity += Vector3.Up * Gravity * (float)delta;
		var newPos = GlobalTransform.Origin + _velocity * (float)delta;

		// Raycast to detect collision along the trajectory
		var space = GetWorld3D().DirectSpaceState;
		var query = PhysicsRayQueryParameters3D.Create(_lastPosition, newPos);
		var result = space.IntersectRay(query);
		if (result.Count > 0)
		{
			Explode(result);
			return;
		}

		// No collision: move rocket forward
		GlobalTransform = new Transform3D(GlobalTransform.Basis, newPos);
		_lastPosition = newPos;
	}

	/// <summary>
	/// External trigger (e.g. lightning synergy) to detonate immediately.
	/// </summary>
	public void Detonate()
	{
		var fakeResult = new Dictionary { { "position", GlobalTransform.Origin } };
		Explode(fakeResult);
	}

	private void Explode(Dictionary result)
	{
		var hitPosition = (Vector3)result["position"];
		GD.Print($"[RocketProjectile3D] Explode at {hitPosition}");

		// Spawn explosion effect
		if (ExplosionEffectScene != null)
		{
			var effect = ExplosionEffectScene.Instantiate<Node3D>();
			effect.GlobalTransform = new Transform3D(Basis.Identity, hitPosition);
			GetTree().CurrentScene.AddChild(effect);
		}

		// Damage all colliders within radius
		var sphere = new SphereShape3D { Radius = ExplosionRadius };
		var shapeParams = new PhysicsShapeQueryParameters3D
		{
			Shape = sphere,
			Transform = new Transform3D(Basis.Identity, hitPosition),
			CollisionMask = uint.MaxValue
		};
		var bodies = GetWorld3D().DirectSpaceState.IntersectShape(shapeParams);
		foreach (Dictionary hit in bodies)
		{
			if (hit.TryGetValue("collider", out var col)
				&& typeof(Node).IsAssignableFrom(col.GetType()))
			{
				var colliderNode = (Node)col;
				if (colliderNode.HasMethod("TakeDamage"))
					colliderNode.Call("TakeDamage", Damage);
			}
		}

		QueueFree();
	}
}
