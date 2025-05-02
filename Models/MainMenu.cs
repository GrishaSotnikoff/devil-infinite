using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace DevilInfinite.Models
{
	public partial class MainMenu : Node3D
	{
		public override void _Ready()
		{
			// Fetch the singleton by its autoload name  
			var sm = GetTree().Root.GetNode<SceneManager>("SceneManager");
			sm.SceneChanged += OnSceneChanged;

			var startBtn = GetNode<Button>("Panel/VBoxContainer/Start");
			startBtn.Pressed += () => sm.GoTo("res://Scenes/Game.tscn"); // Removed 'static' keyword  

			GetNode<Button>("Panel/VBoxContainer/Options")
				.Pressed += () => GD.Print("[MainMenu] Show options dialog");

			GetNode<Button>("Panel/VBoxContainer/Quit")
				.Pressed += () => sm.GoTo(""); // or GetTree().Quit()  
		}

		private void OnSceneChanged(string obj)
		{
			throw new NotImplementedException();
		}

		void _on_devil_infinite_focus_entered()
		{
			// Called when the node enters the scene tree for the first time.
			GD.Print("[MainMenu] Focus entered");
		}
	}
}
