using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scrips.GenData
{
    class DataLoader
    {
        public static NodesMap fetchMap() {
            NodesMap deserializedMap = new NodesMap();

            string json = getJson();
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(deserializedMap.GetType());
            deserializedMap = ser.ReadObject(ms) as NodesMap;
            ms.Close();
            return deserializedMap;
        }

        public static NodesMap createMapFromData()
        {
            NodesMap map = new NodesMap();

            TextAsset txt = (TextAsset)Resources.Load("Data/" + DataSaver.map_data_filename, typeof(TextAsset));
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
                Debug.Log("i: " + i + ":" + j + ", endAngle: " + mapData[4]);
            }

            return map;

        }

        private static string getJson() {
            string path = "map.json";
            string filePath = "Data/" + path.Replace(".json", "");
            TextAsset targetFile = Resources.Load<TextAsset>(filePath);
            return targetFile.text;
        }
        
    }
}
