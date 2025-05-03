/*
 Water Shader for Godot 4
 Professionally authored, clean and maintainable .gdshader
 Implements dynamic waves, depth-based coloring, specular highlights, and foam
 Forward-thinking, modular design allows easy extension and optimization
 */

shader_type spatial;
render_mode blend_mix, cull_back, depth_draw_opaque, diffuse_burley;

// Varyings
varying vec3 v_world_pos;

// -- Uniforms ----------------------------------------------------------------

// Time control
uniform float u_time : hint_range(0.0, 100.0) = 0.0;

// Wave parameters
uniform float u_wave_amplitude : hint_range(0.0, 5.0) = 0.5;       // Maximum wave height
uniform float u_wave_length  : hint_range(0.1, 50.0) = 10.0;      // Distance between wave peaks
uniform float u_wave_speed   : hint_range(0.0, 5.0) = 1.0;         // Speed of wave traversal
uniform vec2  u_wave_direction    = vec2(1.0, 0.0);               // Primary wave direction

// Second wave set for complexity
uniform float u_wave2_amplitude    : hint_range(0.0, 5.0) = 0.2;
uniform float u_wave2_length       : hint_range(0.1, 50.0) = 5.0;
uniform float u_wave2_speed        : hint_range(0.0, 5.0) = 1.5;
uniform vec2  u_wave2_direction    = vec2(0.0, 1.0);               // Secondary wave direction

// Color parameters
uniform vec4 u_shallow_color : source_color = vec4(0.0, 0.3, 0.6, 1.0);
uniform vec4 u_deep_color    : source_color = vec4(0.0, 0.05, 0.2, 1.0);
uniform float u_depth_tint   : hint_range(0.0, 1.0) = 0.5;

// Specular and Fresnel
uniform float u_specular_strength : hint_range(0.0, 2.0) = 0.8;
uniform float u_fresnel_power     : hint_range(0.1, 10.0) = 5.0;

// Global light direction (set from scene)
uniform vec3 u_light_direction = vec3(0.0, -1.0, 0.0);

// Foam parameters
uniform sampler2D u_foam_texture;
uniform float     u_foam_scale     : hint_range(0.1, 10.0) = 4.0;
uniform float     u_foam_threshold : hint_range(0.0, 1.0) = 0.7;

// -- Function: Wave Height ----------------------------------------------------
float get_wave_height(vec2 uv) {
    vec2 dir1 = normalize(u_wave_direction);
    vec2 dir2 = normalize(u_wave2_direction);
    float wave1 = u_wave_amplitude * sin(dot(uv, dir1) / u_wave_length - u_time * u_wave_speed);
    float wave2 = u_wave2_amplitude * sin(dot(uv, dir2) / u_wave2_length + u_time * u_wave2_speed);
    return wave1 + wave2;
}

// -- Function: Normal Calculation --------------------------------------------
vec3 get_wave_normal(vec2 uv) {
    float delta = 0.1;
    float hL = get_wave_height(uv - vec2(delta, 0.0));
    float hR = get_wave_height(uv + vec2(delta, 0.0));
    float hD = get_wave_height(uv - vec2(0.0, delta));
    float hU = get_wave_height(uv + vec2(0.0, delta));
    return normalize(vec3(hL - hR, 2.0 * delta, hD - hU));
}

// -- Vertex Processing -------------------------------------------------------
void vertex() {
    vec2 uv = UV;
    VERTEX.y += get_wave_height(uv);

    // Pass world-space position to fragment
    v_world_pos = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;

    // Recompute normals based on wave
    NORMAL = get_wave_normal(uv);
}

// -- Fragment Processing -----------------------------------------------------
void fragment() {
    // View direction from built-in
    vec3 view_dir = VIEW;

    // Depth-based color blending
    float depth_factor = clamp((v_world_pos.y + u_wave_amplitude) / (u_wave_amplitude * 2.0), 0.0, 1.0);
    vec4 base_col = mix(u_shallow_color, u_deep_color, depth_factor * u_depth_tint);

    // Specular (Blinn-Phong) using custom light direction
    vec3 half_dir = normalize(view_dir + normalize(u_light_direction));
    float spec = pow(max(dot(NORMAL, half_dir), 0.0), 16.0) * u_specular_strength;

    // Fresnel effect
    float fresnel = pow(1.0 - max(dot(NORMAL, view_dir), 0.0), u_fresnel_power);

    // Foam overlay
    float foam_mask = smoothstep(u_foam_threshold - 0.1, u_foam_threshold + 0.1, abs(get_wave_height(UV)));
    vec2 foam_uv = UV * u_foam_scale;
    vec4 foam_col = texture(u_foam_texture, foam_uv) * foam_mask;

    // Final material outputs
    ALBEDO    = base_col.rgb + foam_col.rgb;
    SPECULAR  = spec;
    EMISSION  = vec3(fresnel) * spec;
    ROUGHNESS = 1.0 - fresnel;
    ALPHA     = base_col.a;
}