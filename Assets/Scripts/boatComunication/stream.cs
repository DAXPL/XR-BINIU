using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class stream : MonoBehaviour
{
    public RawImage displayImage;
    public string camUrl = "http://192.168.1.10/capture"; // <- podmieñ na IP swojego ESP32-CAM
    public float refreshRate = 0.033f; // czas miêdzy klatkami (0.1s = 10 FPS)

    void Start()
    {
        StartCoroutine(StreamLoop());
    }

    IEnumerator StreamLoop()
    {
        while (true)
        {
            yield return StartCoroutine(GetSnapshot());
            yield return new WaitForSeconds(refreshRate);
        }
    }

    IEnumerator GetSnapshot()
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(camUrl))
        {
            yield return www.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (www.result != UnityWebRequest.Result.Success)
#else
            if (www.isNetworkError || www.isHttpError)
#endif
            {
                Debug.LogWarning("B³¹d pobierania: " + www.error);
            }
            else
            {
                Texture2D tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
                displayImage.texture = tex;
            }
        }
    }
}
