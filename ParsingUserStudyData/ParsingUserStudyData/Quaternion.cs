using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsingUserStudyData
{
    class Quaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public Vector3 ConvertToEular() {
            Vector3 eular;
	        double test = x * y + z * w;
	        if (test > 0.499) { // singularity at north pole
		        eular.x = 2 * (float) Math.Atan2(x, w);
		        eular.y = (float) Math.PI/2;
		        eular.z = 0;
		        return eular;
	        }
	        if (test < -0.499) { // singularity at south pole
		       eular.x = -2 * (float) Math.Atan2(x, w);
		       eular.y = - (float) Math.PI/2;
		       eular.z = 0;
		       return eular;
	        }
            double sqx = x*x;
            double sqy = y*y;
            double sqz = z*z;
            eular.x = (float) Math.Atan2(2*y*w-2*x*z , 1 - 2*sqy - 2*sqz);
	        eular.y = (float) Math.Asin(2*test);
	        eular.z = (float) Math.Atan2(2*x*w-2*y*z , 1 - 2*sqx - 2*sqz);
            return eular;
        }
    };
}
