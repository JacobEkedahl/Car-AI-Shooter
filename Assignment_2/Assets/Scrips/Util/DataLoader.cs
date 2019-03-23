using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using UnityEngine;

public class DataLoader
{
    public static NodesMap fetchMap(string problem) {
        NodesMap deserializedMap = new NodesMap();

        string json = getJson(problem);
        MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
        DataContractJsonSerializer ser = new DataContractJsonSerializer(deserializedMap.GetType());
        deserializedMap = ser.ReadObject(ms) as NodesMap;
        ms.Close();
        return deserializedMap;
    }

    public static NodesMap createMapFromData(string problem)
    {
        NodesMap map = new NodesMap();

        TextAsset txt = (TextAsset)Resources.Load("Data/" + problem + DataSaver.map_data_filename, typeof(TextAsset));
        string content = txt.text;
        string[] data = content.Split('\n');
        foreach(string d in data)
        {
            if (d == "")
            {
                break;
            }
            string[] mapData = d.Split(' ');
            int i = Int32.Parse(mapData[0]);
            int j = Int32.Parse(mapData[1]);
            float distance = float.Parse(mapData[2]);
            float startAngle = float.Parse(mapData[3]);
            float endAngle = float.Parse(mapData[4]);
            map.add(i, j, distance, startAngle, endAngle);
          //  Debug.Log("i: " + i + ":" + j + ", endAngle: " + mapData[4]);
        }

        return map;

    }

    private static string getJson(string problem) {
        string filePath = "Data/" + problem + DataSaver.map_filename;
        TextAsset targetFile = Resources.Load<TextAsset>(filePath);
        return targetFile.text;
    }
        
}