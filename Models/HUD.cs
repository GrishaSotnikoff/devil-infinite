using Godot;
using System;

public partial class HUD : CanvasLayer
{
    private ProgressBar _hpBar;
    private ProgressBar _manaBar;
    private Player _player;

    public override void _Ready()
    {
        // 1) Cache UI nodes
        _hpBar = GetNode<ProgressBar>("Control/VBoxContainer/HPBar");
        _manaBar = GetNode<ProgressBar>("Control/VBoxContainer/ManaBar");

        // 2) Find the player in the scene tree
        _player = GetTree().CurrentScene.GetNode<Player>("Player");

        // 3) Initialize bars to the player's current stats
        _hpBar.MaxValue = _player.MaxHP;
        _manaBar.MaxValue = _player.MaxMana;
        _hpBar.Value = _player.HP;
        _manaBar.Value = _player.Mana;

        // 4) Connect to the StatsChanged signal
    }

    private void OnStatsChanged(int hp, int mana)
    {
        // Update UI when the player’s stats change
        _hpBar.Value = hp;
        _manaBar.Value = mana;
    }
}
