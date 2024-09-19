using System;
using System.Linq;
using DefaultNamespace;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    [SerializeField] private bool _isTop = false;
    [SerializeField] private int _row;
    [SerializeField] private bool _isFaceUp;

    public Action OnFaceUpUpdated;

    public bool IsTop { get; set; }
    public string Suit { get; set; }
    public int Value { get; set; }
    public int Row { get; set; }

    public bool IsFaceUp
    {
        get => _isFaceUp;
        set
        {
            
            _isFaceUp = value;
            OnFaceUpUpdated?.Invoke();
        }
    }

    public bool IsInDeckPile { get; set; }
    private string _valueString;

    // Start is called before the first frame update
    void Start()
    {
        if (!CompareTag(Tags.CARD))
        {
            return;
        }

        Suit = transform.name.FirstOrDefault().ToString();
        _valueString = transform.name[1..];
        

        Value = _valueString switch
        {
            "A" => 1,
            "2" => 2,
            "3" => 3,
            "4" => 4,
            "5" => 5,
            "6" => 6,
            "7" => 7,
            "8" => 8,
            "9" => 9,
            "10" => 10,
            "J" => 11,
            "Q" => 12,
            "K" => 13,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
