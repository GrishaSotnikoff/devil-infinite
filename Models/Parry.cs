using Godot;
using System;

/// <summary>
/// Reaction-based parry system with logging.
/// </summary>
public partial class Parry : Node
{
	[Export] public float ParryWindow = 0.3f;
	private bool _canParry = false;

	public async void AttemptParry()
	{
		GD.Print("[Parry] Attempt");
		_canParry = true;
		await ToSignal(GetTree().CreateTimer(ParryWindow), "timeout");
		_canParry = false;
	}

	public bool CheckParry(Node attacker)
	{
		if (_canParry)
		{
			GD.Print("[Parry] Success");
			_canParry = false;
			return true;
		}
		GD.Print("[Parry] Failed");
		return false;
	}
}
