using Godot;
using System;

public partial class Overlay : CanvasLayer
{
    private Control _pauseMenu;

    public override void _Ready()
    {
        _pauseMenu = GetNode<Control>("PauseMenu");

        // Wire pause menu buttons
        _pauseMenu.GetNode<Button>("Panel/VBoxContainer/Resume")
            .Pressed += TogglePause;

        _pauseMenu.GetNode<Button>("Panel/VBoxContainer/Main Menu")
            .Pressed += () => GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");

        _pauseMenu.GetNode<Button>("Panel/VBoxContainer/Quit Game")
            .Pressed += () => GetTree().Quit();
        var sm = GetNode<SceneManager>("/root/SceneManager");
        // Subscribe to the C# event directly:
        sm.SceneChanged += OnSceneChanged;
    }

    private void OnSceneChanged(string newScene)
    {
        GD.Print($"Now loading: {newScene}");
    }
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey ev && ev.Pressed && Input.IsActionJustPressed("pause"))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        bool isPaused = !GetTree().Paused;
        GetTree().Paused = isPaused;
        _pauseMenu.Visible = isPaused;
        GD.Print($"[Overlay] Pause toggled: {isPaused}");
    }
}
