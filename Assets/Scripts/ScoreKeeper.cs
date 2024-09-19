using System.Linq;
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

    private bool HasWon()
    {
        return _topStacks.Sum(topStack => topStack.Value) >= 52;
    }

    private void ShowWinScreen()
    {
        _winScreen.SetActive(true);
    }
}
