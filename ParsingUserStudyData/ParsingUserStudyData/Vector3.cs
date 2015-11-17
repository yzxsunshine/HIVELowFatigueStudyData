using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsingUserStudyData
{
    class Vector3
    {
        public float x;
        public float y;
        public float z;

        public float GetDistance (Vector3 other) {
            float distance = 0.0f;
            distance = (float) Math.Sqrt((x - other.x) * (x - other.x) + (y - other.y) * (y - other.y) + (z - other.z) * (z - other.z));
            return distance;
        }
    };
}
