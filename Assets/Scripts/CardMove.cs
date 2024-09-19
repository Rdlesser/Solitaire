using Abstracts;
using UnityEngine;

public class CardMove : Move
{
    private GameObject _card;
    private GameObject _originalParent;
    private Vector3 _localPosition;

    public CardMove(GameObject card, GameObject originalParent)
    {
        _card = card;
        _originalParent = originalParent;
        _localPosition = card.transform.localPosition;
    }

    public override void Undo()
    {
        _card.transform.SetParent(_originalParent.transform);
        _card.transform.position = _originalParent.transform.position;
        _card.transform.localPosition = _localPosition;
        Debug.Log($"Undid move for card {_card.name}");
    }
}