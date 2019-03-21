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
    class DataSaver
    {

        public static string data_filename = "Assets/Resources/Data/car_data.csv";
        public static string map_filename = "map.json";

        public static void save(StringBuilder sb)
        {
            setup(data_filename, "Time Distance Angle");

            StreamWriter sw = new StreamWriter(data_filename, true);
            sw.Write(sb.ToString());
            sw.Close();
        }

        public static void save(float time, float distance, float angle)
        {
            setup(data_filename, "Time Distance Angle");
            StreamWriter sw = new StreamWriter(data_filename, true);
            sw.WriteLine(time.ToString() + " " + distance.ToString() + " " + angle.ToString());
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

        public static void saveMap(NodesMap map) {
            Debug.Log("saving map..");
            MemoryStream stream1 = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(NodesMap));
            ser.WriteObject(stream1, map);
            
            byte[] json = stream1.ToArray();
            stream1.Close();

            string res = Encoding.UTF8.GetString(json, 0, json.Length);
            string path = Application.dataPath + "/Resources/Data/" + map_filename;
            System.IO.File.WriteAllText(path, res);
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
    }
}
