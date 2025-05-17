using System;
using System.Collections.Generic;
using Godot;

namespace DevilInfinite.Core.Scripts;

public partial class Settings : Control
{

    AudioStreamPlayer2D hoverPlayer;
    // Exposed via the Inspector
    [Export] private Slider MasterSlider;
    [Export] private OptionButton ResolutionDropdown;
    [Export] private CheckBox FullscreenToggle;
    [Export] private CheckBox VsyncToggle;        // NEW
    [Export] private OptionButton displayDropdown;

    private readonly ConfigFile _config = new ConfigFile();
    // Define the resolutions you want to support
    private int _currentScreen;

    public override void _Ready()
    {
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
        Button exitButton = null;
        try
        {
            exitButton = GetNode<Button>("VBoxContainer/Exit");
        }
        catch (Exception ex)
        {
            GD.PrintErr("[MainMenu] Start Button path invalid: " + ex.Message);
        }

        if (exitButton != null)
            exitButton.Pressed += () => sm?.GoTo("res://Scenes/MainMenu.tscn");

        // 1) Load saved settings (if any)
        Error err = _config.Load("user://settings.cfg");
        float savedVol = (float)_config.GetValue("audio", "master_volume", -10f);
        bool savedFull = (bool)_config.GetValue("display", "fullscreen", false);
        string savedResStr = (string)_config.GetValue("display", "resolution", "1280x720");
        bool savedVsync = (bool)_config.GetValue("display", "vsync", true);      // NEW
        int savedDispIdx = (int)_config.GetValue("display", "monitor", 0);

        // 2) Populate UI controls
        MasterSlider.Value = savedVol;
        FullscreenToggle.ButtonPressed = savedFull;
        _currentScreen = DisplayServer.WindowGetCurrentScreen(); // which monitor :contentReference[oaicite:2]{index=2}

        PopulateDisplayDropdown();
        /// 2) Populate and hook up the Display dropdown:
        PopulateResolutions();

        // Select the monitor the window is currently on:
        int currentScreen = DisplayServer.WindowGetCurrentScreen();                  // :contentReference[oaicite:1]{index=1}
        displayDropdown.Select(currentScreen);

        // 3) Connect UI signals
        MasterSlider.ValueChanged += OnMasterVolumeChanged;
        FullscreenToggle.Toggled += OnFullscreenToggled;
        ResolutionDropdown.ItemSelected += OnResolutionSelected;
        VsyncToggle.Toggled += OnVsyncToggled;           // NEW
        displayDropdown.ItemSelected += OnDisplaySelected;

        // 4) Apply settings immediately
        ApplyMasterVolume(savedVol);
        ApplyFullscreen(savedFull);
        ApplyVsync(savedVsync);             // NEW

        var parts = savedResStr.Split('×');
        ApplyResolution(int.Parse(parts[0]), int.Parse(parts[1]));
        base._Ready();
    }
    private void PopulateResolutions()
    {
        ResolutionDropdown.Clear();

        // 1) Which monitor the window is on now
        int screenIndex = DisplayServer.WindowGetCurrentScreen();
        // 2) Get that monitor’s true resolution
        Vector2I screenSize = DisplayServer.ScreenGetSize(screenIndex);
        // :contentReference[oaicite:0]{index=0}

        // 3) Build a list of “supported” resolutions: 
        //    always include the real one, then some commons up to that.
        var candidates = new List<Vector2I>
            {
                screenSize,                 // the actual resolution
                new Vector2I(1920, 1080),
                new Vector2I(1600,  900),
                new Vector2I(1366,  768),
                new Vector2I(1280,  720),
                new Vector2I(1024,  768),
                new Vector2I(800,   600)
            };

        // 4) Filter out anything larger than the real screen, then add them:
        foreach (var res in candidates)
        {
            if (res.X <= screenSize.X && res.Y <= screenSize.Y)
            {
                string label = $"{res.X}×{res.Y}";
                ResolutionDropdown.AddItem(label, ResolutionDropdown.ItemCount);
            }
        }
    }
    private void PopulateDisplayDropdown()
    {
        displayDropdown.Clear();
        int screenCount = DisplayServer.GetScreenCount();                          // :contentReference[oaicite:2]{index=2}
        for (int i = 0; i < screenCount; i++)
        {
            Vector2I size = DisplayServer.ScreenGetSize(i);                         // :contentReference[oaicite:3]{index=3}
            displayDropdown.AddItem($"Display {i + 1}: {size.X}×{size.Y}", i);
        }
    }
    private void OnDisplaySelected(long index)
    {
        // Move the game window to the selected screen:
        DisplayServer.WindowSetCurrentScreen((int)index);                               // :contentReference[oaicite:5]{index=5}

        // (Optional) Center the window on that monitor:
        // var winSize   = DisplayServer.WindowGetSize();
        // var screenPos = DisplayServer.ScreenGetPosition(index);
        // var screenSz  = DisplayServer.ScreenGetSize(index);
        // DisplayServer.WindowSetPosition(screenPos + (screenSz - winSize) / 2);
    }
    private void OnSceneChanged(string obj)
    {
        throw new NotImplementedException();
    }
    private void OnVsyncToggled(bool isPressed)      // NEW
    {
        ApplyVsync(isPressed);
        _config.SetValue("display", "vsync", isPressed);
        _config.Save("user://settings.cfg");
    }
    private void ApplyVsync(bool enable)            // NEW
    {
        DisplayServer.WindowSetVsyncMode(enable ? DisplayServer.VSyncMode.Enabled : DisplayServer.VSyncMode.Disabled);
    }
    private void OnHover()
    {
        hoverPlayer.Play();
    }
    private void OnMasterVolumeChanged(double newValue)
    {
        ApplyMasterVolume((float)newValue);
        _config.SetValue("audio", "master_volume", (float)newValue);
        _config.Save("user://settings.cfg");
    }

    private void OnFullscreenToggled(bool isPressed)
    {
        ApplyFullscreen(isPressed);
        _config.SetValue("display", "fullscreen", isPressed);
        _config.Save("user://settings.cfg");
    }

    private void OnResolutionSelected(long index)
    {
        // 1) Grab the text of the selected item, e.g. "1280×720"
        string label = ResolutionDropdown.GetItemText((int)index);

        // 2) Split into width/height
        var parts = label.Split('×');
        if (parts.Length != 2)
            return; // safety check

        if (!int.TryParse(parts[0], out int width) ||
            !int.TryParse(parts[1], out int height))
            return; // safety check

        // 3) Apply the new window size on the current screen
        int screenIndex = DisplayServer.WindowGetCurrentScreen();
        DisplayServer.WindowSetSize(new Vector2I(width, height), screenIndex);

        // 4) Save to your config so it sticks
        _config.SetValue("display", "resolution", label);
        _config.Save("user://settings.cfg");
    }


    // Helpers to actually apply Godot settings at runtime
    private void ApplyMasterVolume(float db)
    {
        int idx = AudioServer.GetBusIndex("Master");
        AudioServer.SetBusVolumeDb(idx, db);
    }

    private void ApplyFullscreen(bool enable)
    {
        DisplayServer.WindowSetMode(
            enable
                ? DisplayServer.WindowMode.Fullscreen
                : DisplayServer.WindowMode.Windowed
        );
    }

    private void ApplyResolution(int width, int height)
    {
        DisplayServer.WindowSetSize(new Vector2I(width, height), 0);
    }
}