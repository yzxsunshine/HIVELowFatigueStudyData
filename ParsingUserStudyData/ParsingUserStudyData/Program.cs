using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;

namespace ParsingUserStudyData
{
    class Program
    {
        public void ParseFile(string filePath)
        {
            StreamReader sr = new StreamReader(filePath);
			string line;
			while((line = sr.ReadLine()) != null) {
				if (line[0] == '#')
					continue;
				string[] parse = line.Split(new char[]{'\t'});
				if (parse.Length < 18) {
					Console.WriteLine("Wrong Stream!");
					return;
				}
				float timeStamp = float.Parse(parse[0]);
				string collision = parse[1];
				string modeSwitch = parse[2];
				string reset = parse[3];
				string wayptTriger = parse[4];

				Vector3 position = new Vector3();
				position.x = float.Parse(parse[5]);
				position.y = float.Parse(parse[6]);
				position.z = float.Parse(parse[7]);

				Quaternion quat = new Quaternion();
				quat.x = float.Parse(parse[8]);
				quat.y = float.Parse(parse[9]);
				quat.z = float.Parse(parse[10]);
				quat.w = float.Parse(parse[11]);

				Vector3 headRot = new Vector3();
				headRot.x = float.Parse(parse[12]);
				headRot.y = float.Parse(parse[13]);
				headRot.z = float.Parse(parse[14]);

				Vector3 wayPtPos = new Vector3();
				wayPtPos.x = float.Parse(parse[15]);
				wayPtPos.y = float.Parse(parse[16]);
				wayPtPos.z = float.Parse(parse[17]);

				if (collision == "COLLISION_ENTER") {

				}

				if (modeSwitch == "WAIT_SWITCH") {
						
				}
					
				else if (modeSwitch == "CORRECT_SWITCH") {
						
				}

				else if (modeSwitch == "INCORRECT_SWITCH") {
						
				}

				if (reset == "RESET") {
						
				}

				if (wayptTriger == "TRIGGER_WAYPOINT") {
						
				}
			}
        }

        static void Main(string[] args)
        {

        }
    }
}
