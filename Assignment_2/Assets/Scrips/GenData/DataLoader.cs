using System;
using System.Collections.Generic;
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

        private static string getJson() {
            string path = "map.json";
            string filePath = "Data/" + path.Replace(".json", "");
            TextAsset targetFile = Resources.Load<TextAsset>(filePath);
            return targetFile.text;
        }
        
    }
}
