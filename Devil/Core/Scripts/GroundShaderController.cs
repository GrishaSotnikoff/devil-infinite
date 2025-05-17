// GroundShaderController.cs
using Godot;
using System;
namespace DevilInfinite.Core.Scripts;

public partial class GroundShaderController : Node3D
{
	[Export]
	public MeshInstance3D GroundMesh { get; set; }

	[Export]
	public Texture2D AlbedoTexture { get; set; }

	[Export]
	public Texture2D NormalTexture { get; set; }

	[Export]
	public Vector2 UVScale { get; set; } = new Vector2(10f, 10f);

	private ShaderMaterial _material;

	public override void _Ready()
	{
		// Create ShaderMaterial and load shader resource
		_material = new ShaderMaterial();
		var shader = GD.Load<Shader>("res://shaders/GroundShader.gdshader");
		_material.Shader = shader;

		// Assign textures and UV scale
		_material.SetShaderParameter("albedo_texture", AlbedoTexture);
		_material.SetShaderParameter("normal_texture", NormalTexture);
		_material.SetShaderParameter("uv_scale", UVScale);

		// Apply material override to the ground mesh
		if (GroundMesh != null)
		{
			GroundMesh.MaterialOverride = _material;
		}
		else
		{
			GD.PushWarning("GroundMesh not set in GroundShaderController");
		}
	}

	// Optional: method to update UV scaling at runtime
	public void SetUVScale(Vector2 newScale)
	{
		UVScale = newScale;
		_material.SetShaderParameter("uv_scale", UVScale);
	}
}
