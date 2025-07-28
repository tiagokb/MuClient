using System;

public static class CharacterSelectionEvents
{
    public static Action<CharacterList> OnCharacterSelected;
    public static Action OnCharacterDeleted;
    public static Action<CharacterDeleteResult> OnCharacterDeleteStatusReturn;
}