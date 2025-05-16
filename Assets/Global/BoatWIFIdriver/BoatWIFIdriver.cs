using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class BoatWIFIdriver : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject connectionPanel;
    [SerializeField] private string serverAddress = "192.168.0.39";
    private int port = 80;
    [SerializeField] private Breadboard breadboard;
    private ClientWebSocket ws;
    private CancellationTokenSource cts;
    
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

    private void ProcessSensorData(string json)
    {
        try
        {
            List<float?> values = JsonConvert.DeserializeObject<List<float?>>(json);
            if (values != null)
            {
                for (int i = 0;i < values.Count; i++)
                {
                    breadboard.SetSensorData(i, values[i] ?? 0f);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("B³¹d JSON: " + e.Message + " | Otrzymano: " + json);
        }
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
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                ProcessSensorData(message);
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
        while (ws.State == WebSocketState.Open)
        {
            if(breadboard == null)
            {
                await Task.Delay(1000);
                continue;
            }

            await SendData(new float[] { breadboard.GetMotorPWM(0), breadboard.GetMotorPWM(1) });
            await Task.Delay(100);
        }
    }

    private async Task SendData(float[] dataArray)
    {
        if (ws == null || ws.State != WebSocketState.Open)
            return;

        List<float?> values = new List<float?>(dataArray.Cast<float?>());
        string jsonMessage = JsonConvert.SerializeObject(values);
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
