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

        public Vector3()
        {
            x = y = z = 0;
        }

        public Vector3(float xx, float yy, float zz)
        {
            x = xx;
            y = yy;
            z = zz;
        }

        public float GetDistance (Vector3 other) {
            float distance = 0.0f;
            distance = (float) Math.Sqrt((x - other.x) * (x - other.x) + (y - other.y) * (y - other.y) + (z - other.z) * (z - other.z));
            return distance;
        }

        public Vector3 Subtract(Vector3 other) {
            return new Vector3(x - other.x, y - other.y, z - other.z);
        }

        public float GetMagnitude()
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }
    };
}
