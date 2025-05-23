using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevilInfinite.Core.Scripts;
using Godot;

namespace DevilInfinite.Core;

public partial class MainMenu : CanvasLayer
{
    AudioStreamPlayer2D hoverPlayer;

    public override void _Ready()
    {
        GD.Print("[MainMenu] _Ready() called");  // Prove that MainMenu scene loaded
        GD.Print($"[MainMenu] SceneManager autoload present: {GetTree().Root.HasNode("SceneManager")}");
        GD.Print($"[MainMenu] Start button present: {HasNode("Panel/VBoxContainer/Start/Button")}");
        hoverPlayer = GetNode<AudioStreamPlayer2D>("HoverPlayer");
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

        Button settingsBtn = null;
        try
        {
            settingsBtn = GetNode<Button>("Panel/VBoxContainer/Options/Button");
        }
        catch (Exception ex)
        {
            GD.PrintErr("[MainMenu] Start Button path invalid: " + ex.Message);
        }

        if (settingsBtn != null)
            settingsBtn.Pressed += () => sm?.GoTo("res://Scenes/Settings.tscn");
    }

    private void OnSceneChanged(string obj)
    {
        throw new NotImplementedException();
    }


    private void OnHover()
    {
        hoverPlayer.Play();
    }
}