using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace DevilInfinite.Models
{
	public partial class MainMenu : CanvasLayer
	{

		public override void _Ready()
		{
			GD.Print("[MainMenu] _Ready() called");  // Prove that MainMenu scene loaded
			GD.Print($"[MainMenu] SceneManager autoload present: {GetTree().Root.HasNode("SceneManager")}");
			GD.Print($"[MainMenu] Start button present: {HasNode("Panel/VBoxContainer/Start/Button")}");

			SceneManager sm = null;
			try
			{
				sm = GetTree().Root.GetNode<SceneManager>("SceneManager");
			}
			catch (Exception ex)
			{
				GD.PrintErr("[MainMenu] SceneManager not found or path incorrect: " + ex.Message);
			}

			if (sm != null)
			{
				sm.SceneChanged += OnSceneChanged;
				GD.Print("[MainMenu] SceneManager connected");
			}

			// Same for button fetching:
			Button startBtn = null;
			try
			{
				startBtn = GetNode<Button>("Panel/VBoxContainer/Start/Button");
			}
			catch (Exception ex)
			{
				GD.PrintErr("[MainMenu] Start Button path invalid: " + ex.Message);
			}

			if (startBtn != null)
				startBtn.Pressed += () => sm?.GoTo("res://Scenes/test_level_1.tscn");
		}

		private void OnSceneChanged(string obj)
		{
			throw new NotImplementedException();
		}

	}
}
