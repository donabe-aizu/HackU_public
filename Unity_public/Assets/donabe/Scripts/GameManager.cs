using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameStatus NowGameStatus
    {
        get => _nowGameStatus;
        set
        {
            OnGameStatusChanged?.Invoke(value);
            _nowGameStatus = value;
            Debug.Log("now GameStatus: " + value);
        }
    }
    private GameStatus _nowGameStatus;

    public event Action<GameStatus> OnGameStatusChanged;

    public Dictionary<int, bool> ClearStages = new Dictionary<int, bool>();

    private void Awake()
    {
        if (instance == null) {
            instance = this;//このインスタンスをstatic な instanceに登録
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);//２回目以降重複して作成してしまったgameObjectを削除
        }
        
        SetupGame();
        
        StartOpening();
    }

    private void OnDisable()
    {
        OnGameStatusChanged -= CheckGameStatus;
    }

    private void SetupGame()
    {
        NowGameStatus = GameStatus.Null;
        OnGameStatusChanged += CheckGameStatus;
        
        ClearStages.Add(1,false);
        ClearStages.Add(2,false);
        ClearStages.Add(3,false);
    }

    private void StartOpening()
    {
        NowGameStatus = GameStatus.Opening;
    }

    public void ClearStage(int nazoNum)
    {
        ClearStages[nazoNum] = true;
        Debug.Log("Clear: " + nazoNum);
    }

    private void CheckGameStatus(GameStatus gameStatus)
    {
        int allClearStages = 0;
        foreach (var stage in ClearStages)
        {
            if (stage.Value)
            {
                allClearStages++;
            }
        }

        if (gameStatus == GameStatus.MainStage)
        {
            if (allClearStages>=3)
            {
                NowGameStatus = GameStatus.LastMission;
            }
        }
    }
}
