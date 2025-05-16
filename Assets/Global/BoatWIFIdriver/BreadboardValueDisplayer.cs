using TMPro;
using UnityEngine;

public class BreadboardValueDisplayer : MonoBehaviour
{
    [SerializeField] private Breadboard breadboard;
    [SerializeField] private string prefix = "";
    [SerializeField] private bool sensor = true;
    [SerializeField] private int index;
    private TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    private void FixedUpdate()
    {
        if (breadboard && text)
        {
            string val = sensor ? ((float)breadboard.GetSensorData(index)).ToString() : (breadboard.GetMotorPWM(index)).ToString();
            text.SetText($"{prefix} {val}");
        }
    }

}
