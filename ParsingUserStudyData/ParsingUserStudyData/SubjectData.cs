using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ParsingUserStudyData
{
    class ModeSwitchData {
        public float switchTime;
        public int numWrongSwitch;
        public ModeSwitchData() {
            switchTime = -1.0f;
            numWrongSwitch = 0;
        }
    };

    class RawData {

    };

    class TrialData {
        float time;
        float startTime;
        float endTime;
        int numCollision;
        float avgDistance;
        float stdDistance;
        ModeSwitchData modeSwitchData;
        int trialType;   // 0 walking, 1 segway, 2 surfing

        public TrialData(int type) {
            trialType = type;
            time = -1.0f;
            numCollision = 0;
            avgDistance = 0;
            stdDistance = 0;
            modeSwitchData = new ModeSwitchData();
        }

        public void ParseFile(string filePath)
        {
            StreamReader sr = new StreamReader(filePath);
            string line;
            endTime = 0;
            List<float> distances = new List<float>();
            int numLine = 0;
            while ((line = sr.ReadLine()) != null)
            {
                if (line[0] == '#')
                    continue;
                string[] parse = line.Split(new char[] { '\t' });
                if (parse.Length < 18)
                {
                    Console.WriteLine("Wrong Stream!");
                    return;
                }
                float timeStamp = float.Parse(parse[0]);
                endTime = timeStamp;
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

                float distance = position.GetDistance(wayPtPos);
                distances.Add(distance);
                avgDistance += distance;
                numLine++;
                if (collision == "COLLISION_ENTER")
                {
                    numCollision++;
                }

                if (modeSwitch == "WAIT_SWITCH")
                {
                    modeSwitchData.switchTime = timeStamp;
                }

                else if (modeSwitch == "CORRECT_SWITCH")
                {
                    if (modeSwitchData.switchTime >= 0)
                    {
                        modeSwitchData.switchTime = timeStamp - modeSwitchData.switchTime;
                        startTime = timeStamp;
                    }
                }

                else if (modeSwitch == "INCORRECT_SWITCH")
                {
                    if (modeSwitchData.switchTime >= 0)
                    {
                        modeSwitchData.numWrongSwitch++;
                    }
                }

                if (reset == "RESET")
                {

                }

                if (wayptTriger == "TRIGGER_WAYPOINT")
                {

                }
            }
            time = endTime - startTime;
            avgDistance /= numLine;
        }
    };

    class ModeType <DataType> {
        List<DataType> trialData;
        int trialStartID;
        float trainingTime;
    };

    class SubjectData
    {
        int id;
        ModeType<TrialData> walkingTrials;
        ModeType<TrialData> segwayTrials;
        ModeType<TrialData> surfingTrials;
    }
}
