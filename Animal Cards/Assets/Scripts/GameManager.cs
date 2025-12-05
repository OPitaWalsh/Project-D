using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]private int p1Health;
    [SerializeField]private int p2Health;
    private int turnCount = 1;      //player 1 or 2?
    private int phaseCount = 0;     //draw, play, or attack?

    public DeckManager deckManager { get; private set; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            deckManager = GetComponentInChildren<DeckManager>();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }



    public int P1Health
    {
        get { return p1Health; }
        set { p1Health = value; }
    }

    public int P2Health
    {
        get { return p2Health; }
        set { p2Health = value; }
    }

    public int TurnCount
    {
        get { return turnCount; }
    }
    
    public int PhaseCount
    {
        get { return phaseCount; }
    }


    public void IncreaseTurnCount()
    {
        if (turnCount == 1) { turnCount++; }
        else { turnCount = 1; }
        //turnCount should always be either 1 or 2, but even if it's not, it will be corrected to 1
    }
    
    public void IncreasePhaseCount()
    {
        if (phaseCount == 0 || phaseCount == 1) { phaseCount++; }
        else { phaseCount = 0; }
        //phaseCount should always be either 0, 1, or 2, but even if it's not, it will be corrected to 0
    }



    public void ResetGame()
    {
        p1Health = 3;
        p2Health = 3;
        turnCount = 1;
        phaseCount = 0;
        
        deckManager.ResetGame();
    }
}
