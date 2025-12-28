using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WebGLLogOverlay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private int maxLines = 20;
    private readonly Queue<string> lines = new();

    private void OnEnable() => Application.logMessageReceived += Handle;
    private void OnDisable() => Application.logMessageReceived -= Handle;

    private void Handle(string condition, string stackTrace, LogType type)
    {
        // 長すぎるのは切る
        if (condition.Length > 180) condition = condition.Substring(0, 180);

        lines.Enqueue(condition);
        while (lines.Count > maxLines) lines.Dequeue();

        text.text = string.Join("\n", lines);
    }
}
