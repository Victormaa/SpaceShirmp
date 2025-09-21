using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public enum Stage 
{
    None,
    Stage1,
    Stage2,
    Stage3,
}
public enum GameState
{
    EntryAnimation,
    Tutorial,
    TaskListShown,
    FreeRoam,
    Checking,
    TaskComplete,
    GameOver,
    Result
}
public class GameManager : MonoBehaviour
{
    public Transform enemyGenerateRange;

    public TMP_Text stageText;

    public Stage curStage;

    public static GameManager Instance { get { return _instance; } private set { } }

    private static GameManager _instance;

    public PolygonCollider2D mainConfiner;

    public PolygonCollider2D[] stageConfiners;

    public GameState CurrentState { get; private set; }

    public UnityEvent<GameState> OnGameStateChanged = new UnityEvent<GameState>();

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        ChangeState(GameState.EntryAnimation);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void ChangeState(GameState newState)
    {
        CurrentState = newState;
        OnGameStateChanged.Invoke(newState);
        Debug.Log($"ÓÎÏ·×´Ì¬ÇÐ»»Îª£º{newState}");
    }
}
