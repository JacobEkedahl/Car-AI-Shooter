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

        public static string data_filename = "Text/car_data";

        public static void save(StringBuilder sb) {
            //Write some text to the test.txt file
            StreamWriter writer = new StreamWriter(data_filename, true);
            writer.WriteLine(sb.ToString());
            writer.Close();
        }
    }
}
