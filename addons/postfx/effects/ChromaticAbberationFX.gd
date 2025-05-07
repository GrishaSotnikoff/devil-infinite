@tool
extends FXBase
class_name ChromaticAbberationFX

@export var strength : float = 2.0:
	set(value):
		strength = value
		notify_change()


func _get_shader_code() -> String:
	return load("res://addons/postfx/shaders/chromatic_abberation.gdshader").code

func _update_shader() -> void:
	properties["offset"] = strength
