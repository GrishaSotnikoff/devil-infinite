using Godot;
using System;
using System.Collections.Generic;
namespace DevilInfinite.Core;

public partial class Hotbar : CanvasLayer
{
    [Signal] public delegate void SlotSelectedEventHandler(int slotIndex);

    private Dictionary<int, TextureButton> _slots = new();
    private int _currentSlot = 1;

    public override void _Ready()
    {
        // Cache slots
        for (int i = 1; i <= 4; i++)
        {
            var btn = GetNode<TextureButton>($"Bar/Slots/Slot{i}");
            _slots[i] = btn;
            int idx = i;
            btn.Pressed += () => OnSlotPressed(idx);
        }

        HighlightSlot(_currentSlot);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // Number keys 1-4
        if (@event is InputEventKey key && key.Pressed && !key.Echo)
        {
            if (key.Keycode >= Key.Key1 && key.Keycode <= Key.Key4)
            {
                int idx = (int)key.Keycode - (int)Key.Key1 + 1;
                SelectSlot(idx);
            }
        }
    }

    private void OnSlotPressed(int idx)
    {
        SelectSlot(idx);
    }

    private void SelectSlot(int idx)
    {
        if (idx < 1 || idx > 4) return;
        if (_currentSlot == idx) return;

        _currentSlot = idx;
        HighlightSlot(idx);
        EmitSignal(nameof(SlotSelected), idx);
        GD.Print($"[Hotbar] Slot {idx} selected");
    }

    private void HighlightSlot(int idx)
    {
        // Simple highlight: adjust modulate or style
        foreach (var kv in _slots)
        {
            kv.Value.SelfModulate = kv.Key == idx ? new Color(1, 1, 1, 1) : new Color(0.7f, 0.7f, 0.7f, 1);
        }
    }
}
