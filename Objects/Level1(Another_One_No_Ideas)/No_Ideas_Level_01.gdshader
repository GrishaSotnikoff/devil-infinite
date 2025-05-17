shader_type spatial;

uniform float damage : hint_range(0.0, 1.0, 0.01);
uniform sampler2D damage_texture;
uniform sampler2D albedo : source_color;

void vertex() {
	vec4 rand = texture(damage_texture, VERTEX.xz);
	rand = (rand / damage);
	vec3 goal = VERTEX * clamp(rand.x, 0.45, 0.5);
	goal *= 2.0;
	VERTEX = goal;
}

void fragment() {
	vec4 albedo_color = texture(albedo, UV);
	ALBEDO = albedo_color.xyz;
}
