using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider))]
public class TeleportWorld : MonoBehaviour
{
    public int nextNazoNumber = 0;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        switch (nextNazoNumber)
        {
            case 0:
                GameManager.instance.NowGameStatus = GameStatus.MainStage;
                SceneManager.LoadScene("MainRoom");
                break;
            case 1:
                GameManager.instance.NowGameStatus = GameStatus.Nazo1;
                NetworkManager.instance.SendMessageToServer("1");
                SceneManager.LoadScene("nazo_1");
                break;
            case 2:
                GameManager.instance.NowGameStatus = GameStatus.Nazo2;
                NetworkManager.instance.SendMessageToServer("2");
                SceneManager.LoadScene("nazo_2");
                break;
            case 3:
                GameManager.instance.NowGameStatus = GameStatus.Nazo3;
                NetworkManager.instance.SendMessageToServer("3");
                SceneManager.LoadScene("nazo_3");
                break;
            default:
                break;
        }
    }
}