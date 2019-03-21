using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scrips.GenData
{
    [DataContract]
    class NodesMap
    {
        [DataMember]
        public Dictionary<string, PairData> map { get; set; } = new Dictionary<string, PairData>();
        public NodesMap() {
            
        }

        public void add(int i, int j, float distance, float startAngle, float endAngle) {
            string p = i + ":" + j;
            PairData pd = new PairData(distance, startAngle, endAngle);
            map.Add(p, pd);
        }

        public float getStartAngle(int i, int j) {
            string p = i + ":" + j;
            return map[p].getStartAngle();
        }

        //used to set the currangle
        public float getEndAngle(int i, int j)
        {
            string p = i + ":" + j;
            return map[p].getEndAngle();

        }

        public float getDistance(int i, int j) {
            string p = i + ":" + j;
            return map[p].getDistance();
        }

        private const double a = 1.0, k = 1.1, d = 1.5, c = 1.0;
        private const double constD = 0.13, addConst = 1.29; 
        private double timeFormula(float angleRelStart, float distance) {
            Debug.Log("angle rel start: " + angleRelStart);
            double addon = Math.Sin(k * (angleRelStart - d)) + 1;
            double distCalc = (distance * constD) -addConst;
            return addon + distCalc;
        }

        public double getTime(float currAngle, int i, int j) {
            float relAngle = getAngleRelStart(currAngle, getStartAngle(i, j));
            return timeFormula(relAngle, getDistance(i, j));
        }

        public float getAngleRelStart(float currAngle, float startAngle) {
            if (currAngle < 0) {
                currAngle = 360 - currAngle;
            }

            if (startAngle < 0) {
                startAngle = 360 - startAngle;
            }

            Debug.Log("currangle: " + currAngle + ", startAngle: " + startAngle);

            return Math.Abs(startAngle - currAngle);
        }

        [DataContract]
        internal class PairData
        {
            [DataMember]
            public float distance { get; set; }
            [DataMember]
            public float startAngle { get; set; }
            [DataMember]
            public float endAngle { get; set; }

            public PairData(float d, float s, float e)
            {
                this.distance = d;
                this.startAngle = s;
                this.endAngle = e;
            }

            public float getDistance()
            {
                return distance;
            }

            public float getStartAngle()
            {
                return startAngle;
            }

            public float getEndAngle()
            {
                return endAngle;
            }
        }
    }
}
