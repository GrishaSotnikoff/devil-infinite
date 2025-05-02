using Godot;
using System;

public partial class SceneManager : Node
{
	// Remove [Signal] attribute if you like; we’ll use a plain C# event:
	public event Action<string> SceneChanged;

	public void GoTo(string scenePath)
	{
		var err = GetTree().ChangeSceneToFile(scenePath);
		if (err != Error.Ok)
			GD.PrintErr($"Scene load failed: {err}");
		else
			// fire the C# event
			SceneChanged?.Invoke(scenePath);
	}
}
