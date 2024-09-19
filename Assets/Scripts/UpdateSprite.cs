using System;
using UnityEngine;

public class UpdateSprite : MonoBehaviour
{
    [SerializeField] private Sprite _cardFace;
    [SerializeField] private Sprite _cardBack;
    [SerializeField] private Solitaire _solitaire;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Selectable _selectable;
    [SerializeField] private UserInput _userInput;

    private void Start()
    {
        _spriteRenderer.sprite = _selectable.IsFaceUp ? _cardFace : _cardBack;
        _selectable.OnFaceUpUpdated += HandleFaceUpUpdate;
    }

    private void OnDestroy()
    {
        _selectable.OnFaceUpUpdated -= HandleFaceUpUpdate;
    }

    private void HandleFaceUpUpdate()
    {
        _spriteRenderer.sprite = _selectable.IsFaceUp ? _cardFace : _cardBack;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (_userInput.GetSlot1())
        {
            // todo: change this to only happen when the card is clicked on instead of every cycle
            _spriteRenderer.color = name == _userInput.GetSlot1().name ? Color.yellow : Color.white;
        }
    }

    public void Inject(Solitaire solitaire, UserInput userInput, Sprite cardFace)
    {
        _solitaire = solitaire;
        _userInput = userInput;
        _cardFace = cardFace;
    }
}
