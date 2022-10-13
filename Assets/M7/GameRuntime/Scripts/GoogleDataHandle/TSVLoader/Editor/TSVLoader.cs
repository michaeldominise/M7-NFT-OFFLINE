using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public static class TSVLoader
{
    static void Load_Base(string[] lines, Action<string> onLineConvertToJson)
    {
        List<string> headerList = null;
        for (var i = 0; i < lines.Length; i++)
        {
            var col = lines[i].Replace("/r", "").Replace("\\","/").Replace("//", "\\\\").Split('\t');
            if (i == 0)
                headerList = col.ToList();
            else
            {
                var jsonStringList = new List<string>();
                for (var x = 0; x < col.Length; x++)
                {
                    if (x >= headerList.Count)
                        break;
                    if (string.IsNullOrEmpty(headerList[x].Trim()))
                        continue;

                    jsonStringList.Add(string.Format("\"{0}\":\"{1}\"", headerList[x].Trim(), col[x].Replace("\"", "").Replace("\r", "").Trim()));
                }
                var jsonString = string.Format("{{{0}}}", jsonStringList.Aggregate((arg1, arg2) => string.Format("{0},{1}", arg1, arg2)));
                //DebugWriter.Log(jsonString, false);

                if (onLineConvertToJson != null)
                    onLineConvertToJson(jsonString);
            }
        }
    }

    public static List<T> LoadFile<T>(string filePath)
    {
        return LoadString<T>(File.ReadAllText(filePath));
    }

    public static List<T> LoadString<T>(string value)
    {
        var objArray = new List<T>();
        Load_Base(value.Split('\n'), (jsonString) =>
        {
            try
            {
                objArray.Add(JsonUtility.FromJson<T>(jsonString));
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("jsonError: {0}", jsonString));
            }
        });
        return objArray;
    }

    public static List<T> LoadToScriptableObjects<T>(string filePath) where T : ScriptableObject
    {
        var objArray = new List<T>();
        Load_Base(File.ReadAllLines(filePath), (jsonString) =>
        {
            var sriptableObject = ScriptableObject.CreateInstance<T>();
            JsonUtility.FromJsonOverwrite(jsonString, sriptableObject);
            objArray.Add(sriptableObject);
        });

        return objArray;
    }

    public static string ConvertToJson(string filePath)
    {
        var stringArray = new List<string>();
        Load_Base(File.ReadAllLines(filePath), (jsonString) => stringArray.Add(jsonString));

        var jsonArrString = string.Format("[\n{0}\n]", stringArray.Aggregate((arg1, arg2) => string.Format("{0},\n{1}", arg1, arg2)));
        return jsonArrString;
    }
}
