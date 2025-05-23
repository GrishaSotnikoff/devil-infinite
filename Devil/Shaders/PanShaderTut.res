RSRC                    VisualShader            �L�;R	�"                                            k      resource_local_to_scene    resource_name    output_port_for_preview    default_input_values    expanded_output_ports    linked_parent_graph_frame    parameter_name 
   qualifier    texture_type    color_default    texture_filter    texture_repeat    texture_source    script    lightmap_size_hint 	   material    custom_aabb    flip_faces    add_uv2    uv2_padding    radius    height    radial_segments    rings    is_hemisphere    mesh    base_texture    image_size    source    texture    input_name    op_type 	   operator    hint    min    max    step    default_value_enabled    default_value 	   function    code    graph_offset    mode    modes/blend    modes/depth_draw    modes/cull    modes/diffuse    modes/specular    flags/depth_prepass_alpha    flags/depth_test_disabled    flags/sss_mode_skin    flags/unshaded    flags/wireframe    flags/skip_vertex_transform    flags/world_vertex_coords    flags/ensure_correct_normals    flags/shadows_disabled    flags/ambient_light_disabled    flags/shadow_to_opacity    flags/vertex_lighting    flags/particle_trails    flags/alpha_to_coverage     flags/alpha_to_coverage_and_one    flags/debug_shadow_splits    flags/fog_disabled    nodes/vertex/0/position    nodes/vertex/connections    nodes/fragment/0/position    nodes/fragment/2/node    nodes/fragment/2/position    nodes/fragment/3/node    nodes/fragment/3/position    nodes/fragment/4/node    nodes/fragment/4/position    nodes/fragment/5/node    nodes/fragment/5/position    nodes/fragment/6/node    nodes/fragment/6/position    nodes/fragment/7/node    nodes/fragment/7/position    nodes/fragment/8/node    nodes/fragment/8/position    nodes/fragment/9/node    nodes/fragment/9/position    nodes/fragment/10/node    nodes/fragment/10/position    nodes/fragment/11/node    nodes/fragment/11/position    nodes/fragment/12/node    nodes/fragment/12/position    nodes/fragment/connections    nodes/light/0/position    nodes/light/connections    nodes/start/0/position    nodes/start/connections    nodes/process/0/position    nodes/process/connections    nodes/collide/0/position    nodes/collide/connections    nodes/start_custom/0/position    nodes/start_custom/connections     nodes/process_custom/0/position !   nodes/process_custom/connections    nodes/sky/0/position    nodes/sky/connections    nodes/fog/0/position    nodes/fog/connections        1   local://VisualShaderNodeTexture2DParameter_2bap0 �         local://SphereMesh_0d1e8 %         local://MeshTexture_rqp5a @      &   local://VisualShaderNodeTexture_dj4kv l      $   local://VisualShaderNodeInput_7e3dq �      '   local://VisualShaderNodeVectorOp_8ft2k �      .   local://VisualShaderNodeVectorDecompose_mhktn m      &   local://VisualShaderNodeFloatOp_23h53 �      -   local://VisualShaderNodeFloatParameter_4mhli       %   local://VisualShaderNodeUVFunc_n3c54 e      ,   local://VisualShaderNodeVec2Parameter_sdny6 �      $   local://VisualShaderNodeInput_64fo1 �      '   local://VisualShaderNodeVectorOp_s7fiu *      %   res://Devil/Shaders/PanShaderTut.res �      #   VisualShaderNodeTexture2DParameter             MainTex                   SphereMesh             MeshTexture                         VisualShaderNodeTexture                                  VisualShaderNodeInput             color          VisualShaderNodeVectorOp                                                                                             VisualShaderNodeVectorDecompose                                                         VisualShaderNodeFloatOp                       VisualShaderNodeFloatParameter             MainTexPower %         &        �@         VisualShaderNodeUVFunc             VisualShaderNodeVec2Parameter             Vector2Parameter %         &   
     �?  �?         VisualShaderNodeInput             time          VisualShaderNodeVectorOp                                                                                            VisualShader !   (      �  shader_type spatial;
render_mode blend_premul_alpha, depth_draw_opaque, cull_disabled, diffuse_lambert, specular_schlick_ggx, sss_mode_skin, unshaded, world_vertex_coords, ambient_light_disabled, fog_disabled;

uniform vec2 Vector2Parameter = vec2(1.000000, 1.000000);
uniform sampler2D MainTex : repeat_enable;
uniform float MainTexPower = 5.0;



void fragment() {
// Input:4
	vec4 n_out4p0 = COLOR;


// Input:11
	float n_out11p0 = TIME;


// Vector2Parameter:10
	vec2 n_out10p0 = Vector2Parameter;


// VectorOp:12
	vec4 n_out12p0 = vec4(n_out11p0) * vec4(n_out10p0, 0.0, 0.0);


// UVFunc:9
	vec2 n_in9p1 = vec2(1.00000, 1.00000);
	vec2 n_out9p0 = vec2(n_out12p0.xy) * n_in9p1 + UV;


	vec4 n_out3p0;
// Texture2D:3
	n_out3p0 = texture(MainTex, n_out9p0);


// FloatParameter:8
	float n_out8p0 = MainTexPower;


// FloatOp:7
	float n_out7p0 = pow(n_out3p0.x, n_out8p0);


// VectorOp:5
	vec4 n_out5p0 = n_out4p0 * vec4(n_out7p0);


// VectorDecompose:6
	float n_out6p0 = n_out5p0.x;
	float n_out6p1 = n_out5p0.y;
	float n_out6p2 = n_out5p0.z;
	float n_out6p3 = n_out5p0.w;


// Output:0
	ALBEDO = vec3(n_out5p0.xyz);
	ALPHA = n_out6p3;
	EMISSION = vec3(n_out5p0.xyz);


}
 +         -         2         3         6         9         @         C   
    @E    D             E   
   �oyD��/CF            G   
     �D  �CH            I   
    ��D  �BJ            K   
    ��D  4CL            M   
   ���Dk��CN            O   
    ��D  �CP            Q   
     �D  DR         	   S   
     D  �CT         
   U   
     �A  �BV            W   
   ��CD��@X            Y   
     4D  �BZ       4                                                                                                                              
                    	      	                                  RSRC