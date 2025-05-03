using Godot;
using System;

/// <summary>
/// Attaches to your water MeshInstance3D node.
/// Animates the shader parameters (time, wave offsets) for dynamic water motion.
/// </summary>
[Tool]
public partial class WaterAnimator : Node3D
{
	// Reference to the water material
	[Export]
	public ShaderMaterial WaterMaterial { get; set; }

	// Speed multiplier for time uniform (in seconds)
	[Export]
	public float TimeSpeed = 1.0f;

	// Wave direction rotation speed (degrees per second)
	[Export]
	public float WaveRotateSpeed = 10.0f;

	// Internal time accumulator
	private float _time = 0.0f;

	public override void _Ready()
	{
		if (WaterMaterial == null)
		{
			GD.PushWarning("WaterAnimator: No WaterMaterial assigned. Animation will not run.");
		}
	}

	public override void _Process(double delta)
	{
		if (WaterMaterial == null)
			return;

		// Update time uniform
		_time += (float)delta * TimeSpeed;
		WaterMaterial.SetShaderParameter("u_time", _time);

		// Optionally rotate primary wave direction over time
		var baseDir = new Vector2(1, 0);
		float angleRad = Mathf.DegToRad(_time * WaveRotateSpeed);
		Vector2 rotDir = new Vector2(
			baseDir.X * Mathf.Cos(angleRad) - baseDir.Y * Mathf.Sin(angleRad),
			baseDir.X * Mathf.Sin(angleRad) + baseDir.Y * Mathf.Cos(angleRad)
		).Normalized();
		WaterMaterial.SetShaderParameter("u_wave_direction", rotDir);

		// You can animate additional uniforms similarly:
		// WaterMaterial.SetShaderParameter("u_wave_amplitude", someValue);
	}
}
