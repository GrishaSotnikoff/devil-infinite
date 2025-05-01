using Godot;

/// <summary>
/// Interface for spells.
/// </summary>
public interface ISpell
{
    void Cast();
    void CastAlternative();
}
