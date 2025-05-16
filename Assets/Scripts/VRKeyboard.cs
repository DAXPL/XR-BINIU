using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VRKeyboard : MonoBehaviour
{
    [SerializeField] private TMP_InputField textInputBaseField;
    [SerializeField] private string[] keyboardMark;
    [SerializeField] private Button[] keyboardButtons;

    void Start()
    {
        for (int i = 0; i < keyboardButtons.Length && i<keyboardMark.Length; i++)
        {
            int index = i;
            if (keyboardMark[index] == "C")
            {
                keyboardButtons[index].onClick.AddListener(() => DelMark());
            }
            else
            {
                keyboardButtons[index].onClick.AddListener(() => PrintMark(keyboardMark[index]));
            }
            
            keyboardButtons[index].GetComponentInChildren<TextMeshProUGUI>().text = keyboardMark[index];
        }
    }
    public void PrintMark(string val)
    {
        textInputBaseField.text += val;
    }
    public void DelMark()
    {
        textInputBaseField.text = textInputBaseField.text.Remove(textInputBaseField.text.Length-1);
    }
}
