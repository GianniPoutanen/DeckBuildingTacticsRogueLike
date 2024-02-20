
public class PlayCardAction : Ability, IUndoRedoAction
{
    private Card card;

    public PlayCardAction(Card card)
    {
        this.card = card;
        Performer = PlayerManager.Instance.Player;
    }

    public override void Perform()
    {
        base.Perform();
        foreach (var action in card.abilities)
        {
            action.Perform();
        }
    }

    public override void Undo()
    {
        for (int i = card.abilities.Length - 1; i >= 0; i--)
        {
            UndoRedoManager.Instance.Undo();
        }
        UIManager.Instance.Hand.SpawnCard(card);
    }

    public override void Redo()
    {
        foreach (var action in card.abilities)
        {
            UndoRedoManager.Instance.Redo();
        }
        PlayerManager.Instance.discardPile.Insert(card, 0);
    }
}