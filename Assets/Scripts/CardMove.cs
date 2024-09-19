using Abstracts;
using UnityEngine;

public class CardMove : Move
{
    private GameObject _card;
    private GameObject _originalParent;

    public CardMove(GameObject card, GameObject originalParent)
    {
        _card = card;
        _originalParent = originalParent;
    }

    public override void Undo()
    {
        _card.transform.SetParent(_originalParent.transform);
        _card.transform.position = _originalParent.transform.position;
        Debug.Log($"Undid move for card {_card.name}");
    }
}