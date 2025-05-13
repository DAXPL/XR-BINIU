using System;
using System.Globalization;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

//install url https://github.com/jilleJr/Newtonsoft.Json-for-Unity.git#upm bo przy tym zwykłym nie wiem ale wywala mi błąd 
public class esp32Controller : MonoBehaviour
{
    private ClientWebSocket ws;
    private CancellationTokenSource cts;
    private Uri uri = new Uri("ws://192.168.1.3/"); // ESP32
    
    private float timeSinceLastSend = 0f;

    public float temperature;
    public float humidity;
    public bool lightT;
    public DistanceData distance = new DistanceData();
    //public float pressure;
    //public float altitude;
    //public AccelData accel = new AccelData();
    public float pressure;
    public float alX;
    public float alY;
    public float alZ;


    async void Start()
    {
        ws = new ClientWebSocket();
        cts = new CancellationTokenSource();
        
        
        try
        {
            await ws.ConnectAsync(uri, cts.Token);
            Debug.Log("Połączono z ESP32!");
            _ = ReceiveLoop(); // START NASLUCHIWANIA 
        }
        catch (Exception ex)
        {
            Debug.LogError("Błąd połączenia: " + ex.Message);
        }
    }

    void Update()
    {
        timeSinceLastSend += Time.deltaTime;


        //dane
        Debug.Log($"temp: {temperature:F1}°C | wilgotność: {humidity:F1}%");
        Debug.Log($"Jasność: {(lightT ? "Jasno" : "Ciemno")}");
        Debug.Log($"Odległości [cm] => Front: {distance.front}, Left: {distance.left}, Right: {distance.right}");
        //Debug.Log($"Ciśnienie: {pressure:F1} Pa | Wysokość: {altitude:F1} m");
        //Debug.Log($"Akcelerometr X:{accel.x:F2}, Y:{accel.y:F2}, Z:{accel.z:F2}");
        Debug.Log($"Ciśnienie: {pressure:F1} Pa");
        Debug.Log($"Akcelerometr X:{alX:F2}, Y:{alY:F2}, Z:{alZ:F2}");


    }

    private async void Send(string message)
    {
        if (ws != null && ws.State == WebSocketState.Open)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cts.Token);
        }
    }

    private async Task ReceiveLoop()
    {
        var buffer = new byte[1024];

        while (ws.State == WebSocketState.Open)
        {
            var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                Debug.LogWarning("Zamknięto połączenie WebSocket");
                break;
            }

            string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            ProcessSensorData(message);
        }
    }
    private void ProcessSensorData(string json)
    {
        Debug.Log(json);
        try
        {
            SensorDataWrapper data = JsonConvert.DeserializeObject<SensorDataWrapper>(json);
            if (data != null && data.sensor != null)
            {
                temperature = data.sensor.temperature;
                humidity = data.sensor.humidity;
                lightT = data.sensor.lightT;
                distance = data.sensor.distance;
                pressure = data.sensor.pressure;
                alX = data.sensor.alX;
                alY = data.sensor.alY;
                alZ = data.sensor.alZ;
                //pressure = data.sensor.barometer.temperature;
                //altitude = data.sensor.barometer.pressure;
                //accel = data.sensor.acceleration;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Błąd JSON: " + e.Message + " | Otrzymano: " + json);
        }
    }


    //private void ProcessSensorData(string json)
    //{
    //    try
    //    {
    //        SensorDataWrapper data = JsonUtility.FromJson<SensorDataWrapper>(json);
    //        if (data != null && data.sensor != null)
    //        {
    //            temperature = data.sensor.temperature;
    //            humidity = data.sensor.humidity;
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogWarning("Błąd JSON: " + e.Message + " | Otrzymano: " + json);
    //    }
    //}

    private void OnApplicationQuit()
    {
        if (ws != null)
        {
            ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Bye", CancellationToken.None);
            ws.Dispose();
        }
    }

    [Serializable]
    public class SensorDataWrapper
    {
        public SensorData sensor;
    }

    [Serializable]
    public class SensorData
    {
        public float temperature;
        public float humidity;
        public bool lightT;
        public DistanceData distance;
        public float pressure;
        public float alX;
        public float alY;
        public float alZ;
        //public Barometer barometer;
        //public AccelData acceleration;

    }
    [Serializable]
    public class DistanceData
    {
        public int front;
        public int left;
        public int right;
    }
    [Serializable]
    public class AccelData
    {
        public float x;
        public float y;
        public float z;
    }
    [Serializable]
    public class Barometer
    {
        public float temperature;
        public float pressure;

    }

}
