using System.Collections.Generic;
using UnityEngine;

public class RuntimeConsole : MonoBehaviour
{
    private readonly List<string> logs = new();
    private Vector2 scrollPos;

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        string color = type switch
        {
            LogType.Error => "red",
            LogType.Exception => "red",
            LogType.Warning => "yellow",
            _ => "white"
        };

        
        logs.Add($"<color={color}>{type}: {logString}\n</color>");

        if (logs.Count > 100)
            logs.RemoveAt(0);
    }

    private void OnGUI()
    {
        GUI.Box(new Rect(10, 10, Screen.width - 20, Screen.height - 20), "Runtime Console");

        GUILayout.BeginArea(new Rect(20, 40, Screen.width - 40, Screen.height - 50));

        scrollPos = GUILayout.BeginScrollView(scrollPos);

        foreach (var log in logs)
        {
            GUILayout.Label(log);
        }

        GUILayout.EndScrollView();

        GUILayout.EndArea();
    }
}
