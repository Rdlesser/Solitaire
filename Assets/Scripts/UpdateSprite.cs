using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpdateSprite : MonoBehaviour
{
    [SerializeField] private Sprite _cardFace;
    [SerializeField] private Sprite _cardBack;
    [SerializeField] private Solitaire _solitaire;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Selectable _selectable;
    
    // Start is called before the first frame update
    void Start()
    {
        var deck = Solitaire.GenerateDeck();

        foreach (var card in deck.Where(card => name == card))
        {
            _cardFace = _solitaire.GetCardFace(card);
            break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //todo: shouldn't be here as it is wasteful - make it only update when needed
        _spriteRenderer.sprite = _selectable.IsFaceUp ? _cardFace : _cardBack;
    }

    public void InjectSolitaire(Solitaire solitaire)
    {
        _solitaire = solitaire;
    }
}
