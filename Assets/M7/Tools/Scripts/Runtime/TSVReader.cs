using System;
using UnityEngine;

public class TSVReader
{
    private static char[] LINE_SEPARATOR { get { char[] val = { '\n', '\r' }; return val; } }

    public static void Read(string tsv, System.Action<int, string> lineCallback)
    {
        string[] lines = tsv.Split(LINE_SEPARATOR);
        for (int lineNumber = 0; lineNumber < lines.Length; lineNumber++)
        {
            try
            {
                lineCallback?.Invoke(lineNumber, lines[lineNumber]);
            }
            catch(Exception)
            {
                Debug.LogError($"{lineNumber}: {lines[lineNumber]}");
            }
        }
    }
}
