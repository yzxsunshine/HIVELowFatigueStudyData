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
        public int numWrongSwitchWalking;
        public int numWrongSwitchSegway;
        public int numWrongSwitchSurfing;
        
        public int numWrongSwitchInit;
        public int numWrongSwitchWalkingInit;
        public int numWrongSwitchSegwayInit;
        public int numWrongSwitchSurfingInit;
        
        public ModeSwitchData()
        {
            switchTime = -1.0f;
            numWrongSwitch = 0;
            numWrongSwitchWalking = 0;
            numWrongSwitchSegway = 0;
            numWrongSwitchSurfing = 0;

            numWrongSwitchInit = 0;
            numWrongSwitchWalkingInit = 0;
            numWrongSwitchSegwayInit = 0;
            numWrongSwitchSurfingInit = 0;
        }
    };

    class RawData {
        public float timeStamp;
        public Vector3 position;
        public Vector3 orientation;
        public Vector3 headOrientation;
        public Vector3 wayPt;
        public float distance;
    };

    class TrialData {
        public int subjectID;
        public float time;
        public float startTime;
        public float endTime;
        public int numCollision;
        public float avgDistance;
        public float stdDistance;
        public ModeSwitchData modeSwitchData;
        public int controlType;
        public int travelType;   // 0 walking, 1 segway, 2 surfing
        public int level;
        public int pass;
        public List<RawData> dataList;

        public TrialData() {
            time = -1.0f;
            numCollision = 0;
            avgDistance = 0;
            stdDistance = 0;
            startTime = -1.0f;
            modeSwitchData = new ModeSwitchData();
            dataList = new List<RawData>();
            
        }

        public void ParseFileName(string filePath)
        {
            int index = filePath.LastIndexOf("\\");
            string fileName = filePath.Substring(index + 1);
            fileName = fileName.Substring(0, fileName.Length - 4);
            string[] seperators = new string[] { "_" };
            string[] tokens = fileName.Split(seperators, StringSplitOptions.None);
            subjectID = int.Parse(tokens[1]);
            controlType = int.Parse(tokens[3]);
            travelType = int.Parse(tokens[5]);
            level = int.Parse(tokens[4]);
            pass = int.Parse(tokens[6]);
        }

        public bool ParseFile(string filePath)
        {
            StreamReader sr = new StreamReader(filePath);
            string line;
            endTime = 0;
            dataList = new List<RawData>();
            int numLine = 0;
            ParseFileName(filePath);
            bool firstCorrectSwitch = true;
            bool waitSwitch = false;
            while ((line = sr.ReadLine()) != null)
            {
                if (line[0] == '#')
                    continue;
                string[] parse = line.Split(new char[] { '\t' });
                if (parse.Length < 18)
                {
                    //Console.WriteLine("Wrong Stream!");
                    if (numLine <= 0)
                        return false;
                    else
                        return true;
                }
                RawData data = new RawData();
                float timeStamp = float.Parse(parse[0]);
                data.timeStamp = timeStamp;
                endTime = timeStamp;
                string collision = parse[1];
                string modeSwitch = parse[2];
                string reset = parse[3];
                string wayptTriger = parse[4];

                Vector3 position = new Vector3();
                position.x = float.Parse(parse[5]);
                position.y = float.Parse(parse[6]);
                position.z = float.Parse(parse[7]);
                data.position = position;

                Quaternion quat = new Quaternion();
                quat.x = float.Parse(parse[8]);
                quat.y = float.Parse(parse[9]);
                quat.z = float.Parse(parse[10]);
                quat.w = float.Parse(parse[11]);
                data.orientation = quat.ConvertToEular();

                Vector3 headRot = new Vector3();
                headRot.x = float.Parse(parse[12]);
                headRot.y = float.Parse(parse[13]);
                headRot.z = float.Parse(parse[14]);
                data.headOrientation = headRot;

                Vector3 wayPtPos = new Vector3();
                wayPtPos.x = float.Parse(parse[15]);
                wayPtPos.y = float.Parse(parse[16]);
                wayPtPos.z = float.Parse(parse[17]);
                data.wayPt = wayPtPos;

                float distance = position.GetDistance(wayPtPos);
                data.distance = distance;
          
                numLine++;
                if (collision == "COLLISION_ENTER")
                {
                    numCollision++;
                }

                if (modeSwitch == "WAIT_SWITCH")
                {
                    modeSwitchData.switchTime = timeStamp;
                    firstCorrectSwitch = false;
                    waitSwitch = true;
                }

                else if (modeSwitch == "CORRECT_SWITCH")
                {
                    if (modeSwitchData.switchTime >= 0 && waitSwitch)
                    {
                        modeSwitchData.switchTime = timeStamp - modeSwitchData.switchTime;
                        startTime = timeStamp;
                        waitSwitch = false;
                    }
                    else if (firstCorrectSwitch && Math.Abs(position.x - wayPtPos.x) < 0.5 && Math.Abs(position.z - wayPtPos.z) < 0.5)
                    {
                        if (modeSwitchData.switchTime < 0.0f)
                            modeSwitchData.switchTime = 0.2f;
                        startTime = timeStamp;
                        firstCorrectSwitch = false;
                    }
                }

                else if (modeSwitch == "INCORRECT_SWITCH_TO_WALKING")
                {
                    if (modeSwitchData.switchTime >= 0 && travelType != 0)
                    {
                        if (startTime < 0)
                        {
                            modeSwitchData.numWrongSwitchInit++;
                            modeSwitchData.numWrongSwitchWalkingInit++;
                        }
                        else
                        {
                            modeSwitchData.numWrongSwitch++;
                            modeSwitchData.numWrongSwitchWalking++;
                        }
                    }
                    else if (travelType == 0)
                    {
                        modeSwitchData.switchTime = timeStamp;
                        waitSwitch = true;
                    }
                }
                else if (modeSwitch == "INCORRECT_SWITCH_TO_SEGWAY")
                {
                    if (modeSwitchData.switchTime >= 0 && travelType != 1)
                    {
                        if (startTime < 0)
                        {
                            modeSwitchData.numWrongSwitchInit++;
                            modeSwitchData.numWrongSwitchSegwayInit++;
                        }
                        else
                        {
                            modeSwitchData.numWrongSwitch++;
                            modeSwitchData.numWrongSwitchSegway++;
                        }
                    }
                    else if (travelType == 1)
                    {
                        modeSwitchData.switchTime = timeStamp;
                        waitSwitch = true;
                    }
                }
                else if (modeSwitch == "INCORRECT_SWITCH_TO_SURFING")
                {
                    if (modeSwitchData.switchTime >= 0 && travelType != 2)
                    {
                        if (startTime < 0)
                        {
                            modeSwitchData.numWrongSwitchInit++;
                            modeSwitchData.numWrongSwitchSurfingInit++;
                        }
                        else
                        {
                            modeSwitchData.numWrongSwitch++;
                            modeSwitchData.numWrongSwitchSurfing++;
                        }
                    }
                    else if (travelType == 2)
                    {
                        modeSwitchData.switchTime = timeStamp;
                        waitSwitch = true;
                    }
                }

                if (reset == "RESET")
                {

                }

                if (wayptTriger == "TRIGGER_WAYPOINT")
                {

                }
                if (modeSwitchData.switchTime >= 0)
                {
                    dataList.Add(data);
                }
            }
            time = endTime - startTime;
            return true;
        }
    };

    class SubjectData
    {
        public int subjectID;
        public List<TrialData> trials;
        public static bool[] validList = null;
        public SubjectData(int id)
        {
            subjectID = id;
            trials = new List<TrialData>();
            if (validList == null)
            {
                InitializeValidList();
            }
        }

        public static void InitializeValidList()
        {
            validList = new bool[32] {true, true, true, true, true, true, true, true 
                                         ,true, true, true, true, true, false, false, true
                                         ,true, true, true, true, true, false, true, true
                                         ,true, true, true, true, true, true, true, false};
        }

        public bool IsValid()
        {
            return validList[subjectID];
        }

        public void AddTrial(string fileName)
        {
            TrialData trialData = new TrialData();
            if (trialData.ParseFile(fileName))
            {
                trials.Add(trialData);
            }
        }

        public void OutputStudyTime(StreamWriter sw)
        {
            //sw.WriteLine("SubjectID,Condition,TravelType,Level,Pass,Duration");
            int startIndex = 0;
            for (int i = 0; i < trials.Count; i++)
            {
                if (trials[i].level > 0)
                {
                    startIndex = i;
                    break;
                }
            }
            for (int i = startIndex; i < trials.Count; i+=2)
            {
                if (trials[i].level > 0)
                {
                    string line = string.Format("{0},{1},{2},{3},{4},{5}", trials[i].subjectID
                        , trials[i].controlType
                        , trials[i].travelType
                        , trials[i].level
                        , trials[i].time
                        , trials[i+1].time);
                    sw.WriteLine(line);
                }
            }
        }

        public void OutputSegwayCollision(StreamWriter sw)
        {
            //sw.WriteLine("SubjectID,Condition,Level,Pass,#Colllision");
            int startIndex = 0;
            for (int i = 0; i < trials.Count; i++)
            {
                if (trials[i].level > 0)
                {
                    startIndex = i;
                    break;
                }
            }
            for (int i = startIndex; i < trials.Count; i += 2)
            {
                if (trials[i].level > 0 && trials[i].travelType == 1)
                {
                    string line = string.Format("{0},{1},{2},{3},{4}", trials[i].subjectID
                        , trials[i].controlType
                        , trials[i].level
                        , trials[i].numCollision
                        , trials[i+1].numCollision);
                    sw.WriteLine(line);
                }
            }
        }

        public void OutputTrainingData(StreamWriter sw)
        {
            //sw.WriteLine("SubjectID,Condition,TravelType,#Pass,Time");
            int[] numPass = new int[3] { 0, 0, 0 };
            float[] startTime = new float[3] { 999, 999, 999 };
            float[] endTime = new float[3] { 0, 0, 0 };
            for (int i = 0; i < trials.Count; i++)
            {
                if (trials[i].level == 0)
                {
                    numPass[trials[i].travelType]++;
                    if (startTime[trials[i].travelType] > trials[i].startTime)
                    {
                        startTime[trials[i].travelType] = trials[i].startTime;
                    }
                    if (endTime[trials[i].travelType] < trials[i].endTime)
                    {
                        endTime[trials[i].travelType] = trials[i].endTime;
                    }
                }
            }
            for (int i = 0; i < 3; i++)
            {
                string line = string.Format("{0},{1},{2},{3},{4}", trials[0].subjectID
                        , trials[0].controlType
                        , i
                        , numPass[i]
                        , endTime[i] - startTime[i]);
                sw.WriteLine(line);
            }
        }

        public void OutputHeatMap(StreamWriter sw)
        {
            //
            for (int i = 0; i < trials.Count; i++)
            {
                if (trials[i].level > 0 && trials[i].travelType == 0)
                {
                    int[] buckets = Enumerable.Repeat<int>(0, 36).ToArray<int>();

                    for (int j = 0; j < trials[i].dataList.Count; j++)
                    {
                        float angle = trials[i].dataList[j].orientation.y;
                        while (angle < 0)
                        {
                            angle += 2 * (float)Math.PI;
                        }
                        int angleInt = (int)(angle * 180 / Math.PI / 10);
                        buckets[angleInt]++;
                    }
                    string line = string.Format("{0},{1},{2},{3}", trials[i].subjectID
                        , trials[i].controlType
                        , trials[i].level
                        , trials[i].pass);
                    float oneOverTotal = 1.0f / trials[i].dataList.Count;
                    for (int j = 0; j < 36; j++)
                    {
                        line += "," + (buckets[j] * oneOverTotal);
                    }
                    sw.WriteLine(line);
                }
            }
        }

        public void OutputStudyModeSwitch(StreamWriter sw)
        {
            //sw.WriteLine("SubjectID,Condition,TravelType,Level,Pass,WrongSwitch,WrongWalking,WrongSegway,WrongSurfing");
            int startIndex = 0;
            for (int i = 0; i < trials.Count; i++)
            {
                if (trials[i].level > 0)
                {
                    startIndex = i;
                    break;
                }
            }
            for (int i = startIndex; i < trials.Count; i += 2)
            {
                if (trials[i].level > 0)
                {
                    string line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13}", trials[i].subjectID
                        , trials[i].controlType
                        , trials[i].travelType
                        , trials[i].level
                        , trials[i].modeSwitchData.switchTime
                        , trials[i].modeSwitchData.numWrongSwitchInit
                        , trials[i].modeSwitchData.numWrongSwitchWalkingInit
                        , trials[i].modeSwitchData.numWrongSwitchSegwayInit
                        , trials[i].modeSwitchData.numWrongSwitchSurfingInit
                        , trials[i+1].modeSwitchData.switchTime
                        , trials[i+1].modeSwitchData.numWrongSwitchInit
                        , trials[i+1].modeSwitchData.numWrongSwitchWalkingInit
                        , trials[i+1].modeSwitchData.numWrongSwitchSegwayInit
                        , trials[i+1].modeSwitchData.numWrongSwitchSurfingInit);
                    sw.WriteLine(line);
                }
            }
        }

        public void OutputSurfingLanding(StreamWriter sw)
        {
            //sw.WriteLine("SubjectID,Condition,Level,Pass,DistanceToGoal,OverShot,SheerOff");
            int startIndex = 0;
            for (int i = 0; i < trials.Count; i++)
            {
                if (trials[i].level > 0)
                {
                    startIndex = i;
                    break;
                }
            }
            for (int i = startIndex; i < trials.Count; i += 2)
            {
                if (trials[i].level > 0 && trials[i].travelType == 2)
                {
                    int pathLength = trials[i].dataList.Count;
                    float distance_1 = trials[i].dataList[pathLength - 1].distance;
                    Vector3 diff_1 = trials[i].dataList[pathLength - 1].position.Subtract(trials[i].dataList[pathLength - 1].wayPt);
                    Vector3 direction_1 = trials[i].dataList[pathLength - 1].wayPt.Subtract(trials[i].dataList[pathLength/2].wayPt);
                    float sheer_1 = 0.0f;
                    if (direction_1.x > direction_1.z)
                    {
                        sheer_1 = diff_1.x;
                    }
                    else
                    {
                        sheer_1 = diff_1.z;
                    }

                    pathLength = trials[i+1].dataList.Count;
                    float distance_2 = trials[i+1].dataList[pathLength - 1].distance;
                    Vector3 diff_2 = trials[i + 1].dataList[pathLength - 1].position.Subtract(trials[i + 1].dataList[pathLength - 1].wayPt);
                    Vector3 direction_2 = trials[i + 1].dataList[pathLength - 1].wayPt.Subtract(trials[i + 1].dataList[pathLength / 2].wayPt);
                    float sheer_2 = 0.0f;
                    if (direction_2.x > direction_2.z)
                    {
                        sheer_2 = diff_2.x;
                    }
                    else
                    {
                        sheer_2 = diff_2.z;
                    }
                    string line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", trials[i].subjectID
                        , trials[i].controlType
                        , trials[i].level
                        , distance_1
                        , diff_1.y
                        , sheer_1
                        , distance_2
                        , diff_2.y
                        , sheer_2);
                    sw.WriteLine(line);
                }
            }
        }
    }
}
