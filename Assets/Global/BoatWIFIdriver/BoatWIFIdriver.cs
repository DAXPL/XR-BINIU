using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoatWIFIdriver : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private GameObject connectionPanel;

    Vector2 movementVector = Vector2.zero;
    private ClientWebSocket ws; // WebSocket client
    public string serverAddress = "ws://192.168.137.134:81"; // Adres ESP32 (IP i port)
    private CancellationTokenSource cts;

    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    public void Update()
    {
        movementVector = moveAction.action.ReadValue<Vector2>();
    }

    void OnApplicationQuit()
    {
        if (ws != null && ws.State == WebSocketState.Open)
        {
            ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Application Quit", CancellationToken.None).Wait();
        }
    }

    public void UpdateIP(string newIp)
    {
        serverAddress = $"ws://{newIp}:81";
    }

    [ContextMenu("Connect")]
    public void Connect()
    {
        ws = new ClientWebSocket();
        cts = new CancellationTokenSource();
        ConnectAsync().ConfigureAwait(false); 
    }

    private async void ReceiveMessagesAsync()
    {
        byte[] buffer = new byte[1024];
        while (ws.State == WebSocketState.Open)
        {
            try
            {
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", cts.Token);
                    Debug.Log("Po³¹czenie zamkniête.");
                    if (connectionPanel) connectionPanel.SetActive(true);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("B³¹d podczas odbierania: " + e.Message);
            }
        }
    }

    private async Task ConnectAsync()
    {
        try
        {
            await ws.ConnectAsync(new Uri(serverAddress), cts.Token);
            Debug.Log("Po³¹czenie z ESP32 otwarte!");
            if (connectionPanel) connectionPanel.SetActive(false);

            // Start listening for messages from WebSocket
            ReceiveMessagesAsync();
            ValueLoopAsync();
        }
        catch (Exception e)
        {
            Debug.LogError("B³¹d po³¹czenia: " + e.Message);
        }
    }

    private async Task ValueLoopAsync()
    {
        Vector2 prevInput = Vector2.zero;
        while (ws.State == WebSocketState.Open)
        {
            float magnitude = (prevInput - movementVector).magnitude;
            if (magnitude >= 0.1f)
            {
                prevInput = movementVector;
                await SendData(movementVector.x, movementVector.y);
                await Task.Delay(100);
            }
        }
    }

    private async Task SendData(float value1, float value2)
    {
        if (ws == null || ws.State != WebSocketState.Open)
            return;

        LedData data = new LedData { led1 = Mathf.Clamp01(value1), led2 = Mathf.Clamp01(-value1), led3 = Mathf.Clamp01(value2) };
        string jsonMessage = JsonUtility.ToJson(data);
        byte[] buffer = Encoding.UTF8.GetBytes(jsonMessage);

        try
        {
            await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cts.Token);
        }
        catch (Exception e)
        {
            Debug.LogError("B³¹d wysy³ania danych: " + e.Message);
        }
    }
}
[System.Serializable]
public class LedData
{
    public float led1;
    public float led2;
    public float led3;
}