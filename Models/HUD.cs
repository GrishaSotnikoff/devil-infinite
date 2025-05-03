using Godot;
using System;

public partial class HUD : CanvasLayer
{
    private ProgressBar _hpBar;
    private ProgressBar _manaBar;
    private Player _player;

    public override void _Ready()
    {
        GD.Print("[HUD] _Ready called");

        // 1) Cache UI nodes
        _hpBar = GetNode<ProgressBar>("Control/VBoxContainer/HPBar");
        _manaBar = GetNode<ProgressBar>("Control/VBoxContainer/ManaBar");
        GD.Print("[HUD] Cached UI nodes");

        // 2) Find the player in the scene tree
        _player = GetTree().CurrentScene.GetNode<Player>("Player");
        if (_player == null)
        {
            GD.PrintErr("[HUD] Player node not found!");
            return;
        }
        GD.Print($"[HUD] Player found: {_player.Name}");

        // 3) Initialize bars to the player's current stats
        _hpBar.MaxValue = _player.MaxHP;
        _manaBar.MaxValue = _player.MaxMana;
        _hpBar.Value = _player.HP;
        _manaBar.Value = _player.Mana;
        GD.Print($"[HUD] Initialized bars: HP={_hpBar.Value}/{_hpBar.MaxValue}, Mana={_manaBar.Value}/{_manaBar.MaxValue}");

        // 4) Connect to the StatsChanged signal
        _player.StatsChanged += OnStatsChanged;
        GD.Print("[HUD] Connected to Player.StatsChanged");
    }

    private void OnStatsChanged(int hp, int mana)
    {
        GD.Print($"[HUD] StatsChanged received: HP={hp}, Mana={mana}");
        _hpBar.Value = hp;
        _manaBar.Value = mana;
    }
}
