using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    [SerializeField] private Selectable[] _topStacks;
    [SerializeField] private GameObject _winScreen;
    
    // Start is called before the first frame update
    void Start()
    {
        _winScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (HasWon())
        {
            ShowWinScreen();
        }
    }

    public bool HasWon()
    {
        var numberOfCardsInTop = 0;

        foreach (var topStack in _topStacks)
        {
            numberOfCardsInTop += topStack.Value;
        }

        return numberOfCardsInTop >= 52;
    }

    private void ShowWinScreen()
    {
        _winScreen.SetActive(true);
    }
}
