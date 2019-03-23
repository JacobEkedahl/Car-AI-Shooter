using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DataSaver
{

    public static string prob1 = "Prob1/";
    public static string prob2 = "Prob2/";
    public static string prob3 = "Prob3/";

    private static string folder = Application.dataPath + "/Resources/Data/";
    public static string data_filename = "car_data";
    public static string map_filename = "map";
    public static string map_data_filename = "mapData";

    public static void save(StringBuilder sb)
    {
        setup(folder + data_filename + ".csv", "Time Distance Angle");

        StreamWriter sw = new StreamWriter(folder + data_filename + ".csv", true);
        sw.Write(sb.ToString());
        sw.Close();
    }

    public static void save(float time, float distance, float angle)
    {
        setup(folder + data_filename + ".csv", "Time Distance Angle");
        StreamWriter sw = new StreamWriter(folder + data_filename + ".csv", true);
        sw.WriteLine(time.ToString() + " " + distance.ToString() + " " + angle.ToString());
        sw.Close();
    }

    public static void saveMapData(int i, int j, float distance, float startAngle, float endAngle, string problem)
    {
        setup(folder + problem + map_data_filename + ".csv", "i j distance startAngle endAngle");
        StreamWriter sw = new StreamWriter(folder + problem + map_data_filename + ".csv", true);
        sw.WriteLine(i.ToString() + " " + j.ToString() + " " + distance.ToString() + " " + startAngle.ToString() + " " + endAngle);
        sw.Close();

    }
        
    private static void setup(string path, string firstLine) {
        if (!File.Exists(data_filename))
        {
            File.Create(data_filename).Dispose();

            using (TextWriter tw = new StreamWriter(data_filename))
            {
                tw.WriteLine(firstLine);
            }
        }
    }

    public static void saveMap(NodesMap map, string problem) {
        Debug.Log("saving map..");
        MemoryStream stream1 = new MemoryStream();
        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(NodesMap));
        ser.WriteObject(stream1, map);
            
        byte[] json = stream1.ToArray();
        stream1.Close();

        string res = Encoding.UTF8.GetString(json, 0, json.Length);
        string path = folder + problem + map_filename + ".json";
        System.IO.File.WriteAllText(path, res);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}