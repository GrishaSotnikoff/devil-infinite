// GroundShader.gdshader
shader_type spatial;
// Cull back faces and enable standard lighting
render_mode cull_back, depth_draw_opaque, blend_mix;

// Textures for albedo and normals
uniform sampler2D albedo_texture : hint_anisotropy;
uniform sampler2D normal_texture : hint_normal;
// UV scaling for tiling
uniform vec2 uv_scale = vec2(10.0, 10.0);

void fragment() {
    // Scale UVs for tiling effect
    vec2 scaled_uv = UV * uv_scale;
    // Sample albedo and normal maps
    vec4 albedo_color = texture(albedo_texture, scaled_uv);
    vec3 normal_map = texture(normal_texture, scaled_uv).rgb;

    // Output base color and transform normal map
    ALBEDO = albedo_color.rgb;
    NORMAL = normal_map * 2.0 - 1.0;
}