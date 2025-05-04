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
    [SerializeField] private InputActionReference gearUp;
    [SerializeField] private InputActionReference gearDown;
    [SerializeField] private GameObject connectionPanel;

    [SerializeField] private Vector2 movementVector = Vector2.zero;
    [SerializeField] private int gear = 5;
    private ClientWebSocket ws; // WebSocket client
    public string serverAddress = "ws://192.168.137.129"; // Adres ESP32 (IP i port)
    public int port = 80;
    private CancellationTokenSource cts;

    private void Start()
    {
        Application.targetFrameRate = 60;
        if (gearUp) gearUp.action.performed += ctx => { gear++; gear = (byte)Mathf.Clamp(gear, 0, 10); };
        if (gearDown) gearDown.action.performed += ctx => { gear--; gear = (byte)Mathf.Clamp(gear, 0, 10); };
    }

    public void Update()
    {
        Vector2 mov = moveAction.action.ReadValue<Vector2>();
        mov = new Vector2(MathF.Round(mov.x / 10.0f * gear, 2), MathF.Round(mov.y / 10.0f * gear, 2));
        movementVector = mov;
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
        serverAddress = $"{newIp}";
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
            string uri = $"ws://{serverAddress}:{port}";
            Debug.Log($"Connecting to {uri}");
            await ws.ConnectAsync(new Uri(uri), cts.Token);
            Debug.Log("Po³¹czenie z ESP32 otwarte!");
            if (connectionPanel) connectionPanel.SetActive(false);

            // Uruchom pêtle w tle
            _ = Task.Run(() => ReceiveMessagesAsync());
            _ = Task.Run(() => ValueLoopAsync());
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

    private async Task SendData(float forwardBackValue, float leftRightValue)
    {
        if (ws == null || ws.State != WebSocketState.Open)
            return;

        ControlWrapper data = new ControlWrapper
        {
            sterowanie = new MotorData
            {
                forwordBack = forwardBackValue,
                leftRight = leftRightValue
            }
        };

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
public class ControlWrapper
{
    public MotorData sterowanie;
}

[System.Serializable]
public class MotorData
{
    public float forwordBack;
    public float leftRight;
}