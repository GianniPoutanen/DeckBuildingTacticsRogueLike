
public class DrawCardAction : Ability, IUndoRedoAction
{
    private Card card;

    public DrawCardAction(Card card)
    {
        this.card = card;
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
        UIManager.Instance.Hand.RemoveCard(card);
    }

    public override void Redo()
    {
        foreach (var action in card.abilities)
        {
            action.Redo();
        }
        PlayerManager.Instance.activeDeck.Insert(card, 0);
    }
}