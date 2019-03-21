using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scrips.GenData
{
    class DataSaver
    {

        public static string data_filename = "Assets/Resources/Data/car_data.csv";

        public static void save(StringBuilder sb)
        {
            setup();

            StreamWriter sw = new StreamWriter(data_filename, true);
            sw.Write(sb.ToString());
            sw.Close();
        }

        public static void save(float time, float distance, float angle)
        {
            setup();
            StreamWriter sw = new StreamWriter(data_filename, true);
            sw.WriteLine(time.ToString() + " " + distance.ToString() + " " + angle.ToString());
            sw.Close();
        }

        private static void setup() {
            if (!File.Exists(data_filename))
            {
                File.Create(data_filename).Dispose();

                using (TextWriter tw = new StreamWriter(data_filename))
                {
                    tw.WriteLine("Time Distance Angle");
                }
            }
        }
    }
}
