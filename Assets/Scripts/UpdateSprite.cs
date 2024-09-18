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
    [SerializeField] private UserInput _userInput;
    
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
        
        if (_userInput.GetSlot1())
        {
            // todo: change this to only happen when the card is clicked on instead of every cycle
            _spriteRenderer.color = name == _userInput.GetSlot1().name ? Color.yellow : Color.white;
        }
    }

    public void Inject(Solitaire solitaire, UserInput userInput)
    {
        _solitaire = solitaire;
        _userInput = userInput;
    }
}
