using System;
using System.Collections;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using System.Globalization;
public class esp32uMoterController : MonoBehaviour
{
    ClientWebSocket webSocket;
    Uri serverUri = new Uri("ws://192.168.1.3/ "); //ESP32
    CancellationTokenSource cancelToken = new CancellationTokenSource();

    async void Start()
    {
        webSocket = new ClientWebSocket();
        try
        {
            await webSocket.ConnectAsync(serverUri, cancelToken.Token);
            Debug.Log("Połączono z ESP32 przez WebSocket");

            // Rozpocznij pętlę wysyłania
            _ = StartSendLoop();
        }
        catch (Exception e)
        {
            Debug.LogError("Błąd połączenia: " + e.Message);
        }
    }

    async Task StartSendLoop()
    {
        while (webSocket.State == WebSocketState.Open)
        {
            float leftPower = Input.GetAxis("Vertical") + Input.GetAxis("Horizontal");
            float rightPower = Input.GetAxis("Vertical") - Input.GetAxis("Horizontal");

            leftPower = Mathf.Clamp(leftPower, -1f, 1f);
            rightPower = Mathf.Clamp(rightPower, -1f, 1f);

            string json = $"{{\"power\":{{\"left\":{leftPower.ToString("F2", CultureInfo.InvariantCulture)},\"right\":{rightPower.ToString("F2", CultureInfo.InvariantCulture)}}}}}";
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            var segment = new ArraySegment<byte>(bytes);
            Console.WriteLine(json);

            try
            {
                await webSocket.SendAsync(segment, WebSocketMessageType.Text, true, cancelToken.Token);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Błąd wysyłania: " + e.Message);
                break;
            }

            await Task.Delay(100); // zamiast yield return
        }
    }

    async void OnApplicationQuit()
    {
        if (webSocket != null && webSocket.State == WebSocketState.Open)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Zamykanie", CancellationToken.None);
            webSocket.Dispose();
            Debug.Log("Zamknięto połączenie WebSocket");
        }
    }
}
