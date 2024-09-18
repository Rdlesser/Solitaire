using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    [SerializeField] private bool _isTop = false;
    [SerializeField] private int _row;

    public bool IsTop
    {
        get => _isTop;
        set => _isTop = value;
    }

    public string Suit { get; set; }
    public int Value { get; set; }

    public int Row
    {
        get => _row;
        set => _row = value;
    }
    
    public bool IsFaceUp { get; set; }
    public bool IsInDeckPile { get; set; }

    private string _valueString;
    // Start is called before the first frame update
    void Start()
    {
        if (CompareTag(Tags.CARD))
        {
            Suit = transform.name[0].ToString();

            for (int i = 1; i < transform.name.Length; i++)
            {
                var character = transform.name[i];
                _valueString += character;
            }

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

    // Update is called once per frame
    void Update()
    {
        
    }
}
