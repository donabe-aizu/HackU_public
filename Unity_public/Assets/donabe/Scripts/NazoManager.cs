using System;
using UniRx;
using UnityEngine;

/// <summary>
/// 各謎解きシーンに一つ
/// </summary>
public class NazoManager : MonoBehaviour
{
    public int nazoNumber;

    private void Start()
    {
        NetworkManager.instance.isCorrectAnswer.Subscribe(x =>
        {
            if (x.Item1 == nazoNumber)
            {
                Answer(x.Item2);
            }
        }).AddTo(this);
    }

    public void Answer(bool isCorrect)
    {
        if (isCorrect)
        {
            GameManager.instance.ClearStage(nazoNumber);
        }
    }
}