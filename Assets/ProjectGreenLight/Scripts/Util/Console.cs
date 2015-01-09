using UnityEngine;

class Console : MonoBehaviour
{
    private static string logText = "";

    public static void Log(string newLogText)
    {
        logText += ("\n"+newLogText);
        Debug.Log("\n"+newLogText);
    }

    void OnGUI()
    {
        GUI.TextArea(new Rect(400, 100, 300, 600), logText);
    }
}
