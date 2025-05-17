// HterrainShaderController.cs
// Attach this script to a parent of your HTerrain node (e.g., the scene root).
// It applies a custom ShaderMaterial to the HTerrain and sets all shader parameters.
using Godot;
using System;

[Tool]
public partial class HterrainShaderController : Node3D
{
    [Export]
    public ShaderMaterial TerrainShaderMaterial { get; set; }

    [Export]
    public Texture2D GrassTexture { get; set; }
    [Export]
    public Texture2D RockTexture { get; set; }
    [Export]
    public Texture2D SnowTexture { get; set; }

    [Export]
    public Color BaseColor { get; set; } = new Color(0.4f, 0.33f, 0.27f);

    [Export]
    public float HeightGrassMax { get; set; } = 15.0f;
    [Export]
    public float HeightRockMin { get; set; } = 10.0f;
    [Export]
    public float HeightRockMax { get; set; } = 40.0f;
    [Export]
    public float HeightSnowMin { get; set; } = 30.0f;

    [Export]
    public float SlopeRockThreshold { get; set; } = 0.3f;
    [Export]
    public float SlopeSnowThreshold { get; set; } = 0.6f;

    public override void _EnterTree()
    {
        ApplyMaterial();
    }

    public override void _Ready()
    {
        ApplyMaterial();
    }

    private void ApplyMaterial()
    {
        if (TerrainShaderMaterial == null)
            return;

        // Adjust the path if your HTerrain node is named differently
        var terrainNode = GetNode<Node3D>("HTerrain");
        if (terrainNode == null)
            return;

        // Assign the shader material as the override
        terrainNode.Set("material_override", TerrainShaderMaterial);

        // Update shader parameters
        TerrainShaderMaterial.SetShaderParameter("tex_grass", GrassTexture);
        TerrainShaderMaterial.SetShaderParameter("tex_rock", RockTexture);
        TerrainShaderMaterial.SetShaderParameter("tex_snow", SnowTexture);
        TerrainShaderMaterial.SetShaderParameter("base_color", BaseColor);
        TerrainShaderMaterial.SetShaderParameter("height_grass_max", HeightGrassMax);
        TerrainShaderMaterial.SetShaderParameter("height_rock_min", HeightRockMin);
        TerrainShaderMaterial.SetShaderParameter("height_rock_max", HeightRockMax);
        TerrainShaderMaterial.SetShaderParameter("height_snow_min", HeightSnowMin);
        TerrainShaderMaterial.SetShaderParameter("slope_rock_threshold", SlopeRockThreshold);
        TerrainShaderMaterial.SetShaderParameter("slope_snow_threshold", SlopeSnowThreshold);
    }
}