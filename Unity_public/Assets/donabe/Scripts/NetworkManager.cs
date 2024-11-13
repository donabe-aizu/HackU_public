using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    public string serverIP = "127.0.0.1"; // サーバーのIPアドレス
    public int port = 49152; // サーバーのポート番号
    
    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;
    private bool isConnected = false;

    public IObservable<bool> isShowHint => hintSubject;
    private Subject<bool> hintSubject = new Subject<bool>();
    
    public IObservable<(int,bool)> isCorrectAnswer => questionSubject;
    private Subject<(int,bool)> questionSubject = new Subject<(int,bool)>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this; //このインスタンスをstatic な instanceに登録
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); //２回目以降重複して作成してしまったgameObjectを削除
        }
    }

    private async void Start()
    {
        ConnectToServer();
        await UniTask.WaitForSeconds(3);
        SendMessageToServer("Test");
    }

    private void OnDestroy()
    {
        DisconnectFromServer();
    }

    // 接続の確立
    private async void ConnectToServer()
    {
        try
        {
            client = new TcpClient();
            await client.ConnectAsync(serverIP, port);
            stream = client.GetStream();
            isConnected = true;

            // 非同期で受信するスレッドを開始
            await UniTask.RunOnThreadPool(ReceiveData);

            Debug.Log("サーバーに接続しました。");
        }
        catch (Exception e)
        {
            Debug.LogError("サーバーへの接続に失敗しました: " + e.Message);
        }
    }

    // サーバーサイドへメッセージの送信
    public void SendMessageToServer(string message)
    {
        if (isConnected && stream != null)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);
                Debug.Log("メッセージを送信しました: " + message);
            }
            catch (Exception e)
            {
                Debug.LogError("メッセージ送信に失敗しました: " + e.Message);
            }
        }
    }

    // サーバーサイドからのメッセージの受信（非同期）
    private void ReceiveData()
    {
        while (isConnected)
        {
            try
            {
                if (stream != null && stream.DataAvailable)
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Debug.Log("サーバーからのメッセージ: " + message);
                        MessageParse(message);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("メッセージ受信に失敗しました: " + e.Message);
                isConnected = false;
            }
        }
    }

    // 接続の切断
    private void DisconnectFromServer()
    {
        isConnected = false;

        if (receiveThread != null && receiveThread.IsAlive)
        {
            receiveThread.Abort();
        }

        if (stream != null)
        {
            stream.Close();
            stream = null;
        }

        if (client != null)
        {
            client.Close();
            client = null;
        }

        Debug.Log("サーバーとの接続を切断しました。");
    }

    private void MessageParse(string message)
    {
        message = message.Replace("{", "").Replace("}", "").Replace("\"", "").Replace(" ", "");
        string[] parts = message.Split(':');
        
        string key = parts[0];
        string value = parts[1];

        switch (key)
        {
            case "fox":
                hintSubject.OnNext(true);
                break;
            case "question1":
                questionSubject.OnNext(value == "1" ? (1, true) : (1, false));
                break;
            case "question2":
                questionSubject.OnNext(value == "1" ? (2, true) : (2, false));
                break;
            case "question3":
                questionSubject.OnNext(value == "1" ? (3, true) : (3, false));
                break;
            case "question4":
                questionSubject.OnNext(value == "1" ? (4, true) : (4, false));
                break;
            default:
                break;
        }
    }
}
