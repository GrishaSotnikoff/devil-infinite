using Godot;
namespace DevilInfinite.Devil.Interfaces;

/// <summary>
/// Interface for spells.
/// </summary>
public interface ISpell
{
    void Cast();
    void CastAlternative();
}
