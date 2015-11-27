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
        public List<Vector3> wayPoints;
        public List<int> wayPointIDs;

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
            wayPoints = new List<Vector3>();
            wayPointIDs = new List<int>();
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
                    if (dataList.Count > 0)
                    {
                        wayPointIDs.Add(dataList.Count);
                        wayPoints.Add(wayPtPos);
                    }
                }

                if (modeSwitchData.switchTime >= 0)
                {
                    if (dataList.Count == 0) 
                    {
                        wayPointIDs.Add(0);
                        wayPoints.Add(position);
                    }
                    dataList.Add(data);
                }
            }
            if (wayPointIDs.Count > 0)
            {
                wayPointIDs.Add(dataList.Count - 1);
                wayPoints.Add(dataList[dataList.Count - 1].wayPt);
            }
            time = endTime - startTime;
            return true;
        }
    };

    class SubjectData
    {
        public static int[,] lattinSquare = new int[9, 9] {{0,8,1,7,2,6,3,5,4},
														{1,0,2,8,3,7,4,6,5},
														{2,1,3,0,4,8,5,7,6},
														{3,2,4,1,5,0,6,8,7},
														{4,3,5,2,6,1,7,0,8},
														{5,4,6,3,7,2,8,1,0},
														{6,5,7,4,8,3,0,2,1},
														{7,6,8,5,0,4,1,3,2},
														{8,7,0,6,1,5,2,4,3}};
        public int subjectID;
        public int gender;
        public int age;
        public int gamingexp;
        public int fpsexp;
        public int vrexp;
        public int mazeexp;
        public List<TrialData> trainingTrials;
        public TrialData[, ] trials;
        public static bool[] validList = null;
        public SubjectData(int id, int genderParam, int ageParam, int gaming, int fps, int vr, int maze)
        {
            subjectID = id;
            gender = genderParam;
            age = ageParam;
            gamingexp = gaming;
            fpsexp = fps;
            vrexp = vr;
            mazeexp = maze;
            trainingTrials = new List<TrialData>();
            trials = new TrialData[9, 2];
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
                if (trialData.level == 0)
                {
                    trainingTrials.Add(trialData);
                }
                else
                {
                    int id = trialData.travelType * 3 + trialData.level;
                    trials[id-1, trialData.pass] = trialData;
                }
                
            }
        }

        public void OutputStudyTime(StreamWriter sw)
        {
            //sw.WriteLine("SubjectID,Condition,TravelType,Level,Pass,Duration");
            StringBuilder line = new StringBuilder();
            line.AppendFormat("{0},{1}", trials[0, 0].subjectID, trials[0, 0].controlType);
            line.AppendFormat(",{0},{1},{2},{3},{4},{5}", gender, age, gamingexp, fpsexp, vrexp, mazeexp);
            float[, ] avgTime = new float[3, 2] {{0, 0}, {0, 0}, {0, 0}};
            for (int i = 0; i < 9; i++) {
                line.AppendFormat(",{0},{1}", trials[i, 0].time, trials[i, 1].time);
                avgTime[i / 3, 0] += trials[i, 0].time;
                avgTime[i / 3, 1] += trials[i, 1].time;
            }
            for (int i = 0; i < 3; i++)
            {
                line.AppendFormat(",{0},{1}", avgTime[i, 0] / 3, avgTime[i, 1] / 3);
            }
            sw.WriteLine(line);
        }

        public void OutputSegwayCollision(StreamWriter sw)
        {
            //sw.WriteLine("SubjectID,Condition,Level,Pass,#Colllision");
            StringBuilder line = new StringBuilder();
            line.AppendFormat("{0},{1}", trials[0, 0].subjectID, trials[0, 0].controlType);
            line.AppendFormat(",{0},{1},{2},{3},{4},{5}", gender, age, gamingexp, fpsexp, vrexp, mazeexp);
            float[] avgCollision = new float[2] { 0, 0 };
            for (int i = 3; i < 6; i++)
            {
                if (trials[i, 0].travelType == 1)
                {
                    line.AppendFormat(",{0},{1}", trials[i, 0].numCollision, trials[i, 1].numCollision);
                    avgCollision[0] += trials[i, 0].numCollision;
                    avgCollision[1] += trials[i, 1].numCollision;
                }
            }
            line.AppendFormat(",{0},{1}", avgCollision[0] / 3, avgCollision[1] / 3);
            sw.WriteLine(line);
        }

        public void OutputTrainingData(StreamWriter sw)
        {
            //sw.WriteLine("SubjectID,Condition,TravelType,#Pass,Time");
            int[] numPass = new int[3] { 0, 0, 0 };
            float[] startTime = new float[3] { 999, 999, 999 };
            float[] endTime = new float[3] { 0, 0, 0 };
            for (int i = 0; i < trainingTrials.Count; i++)
            {
                numPass[trainingTrials[i].travelType]++;
                if (startTime[trainingTrials[i].travelType] > trainingTrials[i].startTime)
                {
                    startTime[trainingTrials[i].travelType] = trainingTrials[i].startTime;
                }
                if (endTime[trainingTrials[i].travelType] < trainingTrials[i].endTime)
                {
                    endTime[trainingTrials[i].travelType] = trainingTrials[i].endTime;
                }
            }
            StringBuilder line = new StringBuilder();
            line.AppendFormat("{0},{1}", trainingTrials[0].subjectID, trainingTrials[0].controlType);
            line.AppendFormat(",{0},{1},{2},{3},{4},{5}", gender, age, gamingexp, fpsexp, vrexp, mazeexp);
            float totalPass = 0;
            float totalTime = 0;
            for (int i = 0; i < 3; i++)
            {
                line.AppendFormat(",{0},{1}", numPass[i], endTime[i] - startTime[i]);
                totalPass += numPass[i];
                totalTime += endTime[i] - startTime[i];
            }
            line.AppendFormat(",{0},{1}", totalPass, totalTime);
            sw.WriteLine(line);
        }

        public void OutputHeatMap(StreamWriter sw)
        {
            int[] buckets = Enumerable.Repeat<int>(0, 36).ToArray<int>();
            int totalCount = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 2; j++)
                {   
                    for (int k = 0; k < trials[i, j].dataList.Count; k++)
                    {
                        float angle = trials[i, j].dataList[k].orientation.y;
                        while (angle < 0)
                        {
                            angle += 2 * (float)Math.PI;
                        }
                        int angleInt = (int)(angle * 180 / Math.PI / 10);
                        buckets[angleInt]++;
                    }
                    totalCount += trials[i, j].dataList.Count;
                }
            }
            string line = string.Format("{0},{1}", trials[0, 0].subjectID, trials[0, 0].controlType);
            float oneOverTotal = 1.0f / totalCount;
            for (int j = 0; j < 36; j++)
            {
                line += "," + (buckets[j] * oneOverTotal);
            }
            sw.WriteLine(line);
        }

        public void OutputStudyModeSwitch(StreamWriter sw)
        {
            //sw.WriteLine("SubjectID,Condition,TravelType,Level,Pass,WrongSwitch,WrongWalking,WrongSegway,WrongSurfing");
            StringBuilder line = new StringBuilder();
            line.AppendFormat("{0},{1}", trials[0, 0].subjectID, trials[0, 0].controlType);
            line.AppendFormat(",{0},{1},{2},{3},{4},{5}", gender, age, gamingexp, fpsexp, vrexp, mazeexp);
            float[,] avgTime = new float[3, 2] { { 0, 0 }, { 0, 0 }, { 0, 0 } };
            for (int i = 0; i < 9; i++)
            {
                line.AppendFormat(",{0},{1}", trials[i, 0].modeSwitchData.switchTime, trials[i, 1].modeSwitchData.switchTime);
                avgTime[i / 3, 0] += trials[i, 0].modeSwitchData.switchTime;
                avgTime[i / 3, 1] += trials[i, 1].modeSwitchData.switchTime;
            }
            float[] totalTime = new float[2] {0, 0};
            for (int i = 0; i < 3; i++)
            {
                totalTime[0] += avgTime[i, 0];
                totalTime[1] += avgTime[i, 1];
                line.AppendFormat(",{0},{1}", avgTime[i, 0] / 3, avgTime[i, 1] / 3);
            }
            line.AppendFormat(",{0},{1}", totalTime[0] / 9, totalTime[1] / 9);
            sw.WriteLine(line);
        }

        public void OutputSurfingLanding(StreamWriter sw)
        {
            //sw.WriteLine("SubjectID,Condition,Level,Pass,DistanceToGoal,OverShot,SheerOff");
            StringBuilder line = new StringBuilder();
            line.AppendFormat("{0},{1}", trials[0, 0].subjectID, trials[0, 0].controlType);
            line.AppendFormat(",{0},{1},{2},{3},{4},{5}", gender, age, gamingexp, fpsexp, vrexp, mazeexp);
            float[] avgDistance = new float[2] { 0, 0 };
            int lattinID = (subjectID / 2) % 9;
            int[] lattinSequence = new int[3];
            int counter = 0;
            for (int i = 0; i < 9; i++)
            {
                if (lattinSquare[lattinID, i] > 5) {
                    lattinSequence[counter++] = lattinSquare[lattinID, i];
                }
            }
            for (int i = 0; i < 3; i++)
            {
                if (trials[lattinSequence[i], 0].travelType == 2)
                {
                    int pathLength = trials[lattinSequence[i], 0].dataList.Count;
                    float distance_1 = trials[lattinSequence[i], 0].dataList[pathLength - 1].distance;
                    Vector3 diff_1 = trials[lattinSequence[i], 0].dataList[pathLength - 1].position.Subtract(trials[lattinSequence[i], 0].dataList[pathLength - 1].wayPt);
                    Vector3 direction_1 = trials[lattinSequence[i], 0].dataList[pathLength - 1].wayPt.Subtract(trials[lattinSequence[i], 0].dataList[pathLength / 2].wayPt);
                    float sheer_1 = 0.0f;
                    if (direction_1.x > direction_1.z)
                    {
                        sheer_1 = diff_1.x;
                    }
                    else
                    {
                        sheer_1 = diff_1.z;
                    }

                    line.AppendFormat(",{0}", distance_1);
                    avgDistance[0] += distance_1;
                }
            }

            counter = 0;
            for (int i = 0; i < 9; i++)
            {
                if (lattinSquare[(lattinID + 4) % 9, i] > 5)
                {
                    lattinSequence[counter++] = lattinSquare[(lattinID + 4) % 9, i];
                }
            }
            for (int i = 0; i < 3; i++)
            {
                if (trials[lattinSequence[i], 0].travelType == 2)
                {
                    int pathLength = trials[lattinSequence[i], 1].dataList.Count;
                    float distance_2 = trials[lattinSequence[i], 1].dataList[pathLength - 1].distance;
                    Vector3 diff_2 = trials[lattinSequence[i], 1].dataList[pathLength - 1].position.Subtract(trials[lattinSequence[i], 1].dataList[pathLength - 1].wayPt);
                    Vector3 direction_2 = trials[lattinSequence[i], 1].dataList[pathLength - 1].wayPt.Subtract(trials[lattinSequence[i], 1].dataList[pathLength / 2].wayPt);
                    float sheer_2 = 0.0f;
                    if (direction_2.x > direction_2.z)
                    {
                        sheer_2 = diff_2.x;
                    }
                    else
                    {
                        sheer_2 = diff_2.z;
                    }
                    line.AppendFormat(",{0}", distance_2);
                    avgDistance[1] += distance_2;
                }
            }
            line.AppendFormat(",{0},{1}", avgDistance[0] / 3, avgDistance[1] / 3);
            sw.WriteLine(line);
        }

        private float GetXZDistance (Vector3 startPt, Vector3 endPt, Vector3 pt, float Magnitude)
        {
            float distance = 0.0f;
            if (Magnitude <= 0.0f)
            {
                Magnitude = endPt.Subtract(startPt).GetMagnitude();
            }
            distance = (float) Math.Abs((endPt.z - startPt.z) * pt.x - (endPt.x - startPt.x) * pt.z + endPt.x * startPt.z - endPt.z * startPt.x) / Magnitude;
            return distance;
        }

        public void OutputWalkingPathDistance(StreamWriter sw)
        {
            StringBuilder line = new StringBuilder();
            line.AppendFormat("{0},{1}", trials[0, 0].subjectID, trials[0, 0].controlType);
            line.AppendFormat(",{0},{1},{2},{3},{4},{5}", gender, age, gamingexp, fpsexp, vrexp, mazeexp);
            float[] avgTotalDistance = {0, 0};
            float[] avgTotalTriggerDistance = { 0, 0 };
                 
            for (int i = 0; i < 3; i++) // only walking trials
            {
                for (int j = 0; j < 2; j++)
                {
                    float avgDistance = 0;
                    float avgTriggerDistance = 0;
                    int wayPtNum = trials[i, j].wayPoints.Count - 1;
                    int pathLength = trials[i, j].dataList.Count;
                    for (int k = 0; k < trials[i, j].wayPoints.Count - 1; k++) 
                    {
                        int startID = trials[i, j].wayPointIDs[k];
                        int endID = trials[i, j].wayPointIDs[k + 1];
                        Vector3 startPt = trials[i, j].wayPoints[k];
                        Vector3 endPt = trials[i, j].wayPoints[k + 1];
                        float magnitude = endPt.Subtract(startPt).GetMagnitude();
                        if (magnitude < 0.001f)
                        {
                            wayPtNum--;
                            pathLength -= endID - startID;
                            continue;
                        }
                        int l = startID;
                        for (; l < endID; l++)
                        {
                            float distance = GetXZDistance(startPt, endPt, trials[i, j].dataList[l].position, magnitude);
                            avgDistance += distance;
                        }
                        Vector3 curPos = trials[i, j].dataList[l].position;
                        curPos.y = endPt.y;
                        avgTriggerDistance += endPt.GetDistance(curPos);    // only count x and z distance
                    }
                    avgTriggerDistance /= wayPtNum;
                    avgDistance /= pathLength;
                    avgTotalDistance[j] += avgDistance;
                    avgTotalTriggerDistance[j] += avgTriggerDistance;
                    line.AppendFormat(",{0},{1}", avgTriggerDistance, avgDistance);
                }
            }
            line.AppendFormat(",{0},{1}", avgTotalTriggerDistance[0] / 3, avgTotalDistance[0] / 3);
            line.AppendFormat(",{0},{1}", avgTotalTriggerDistance[1] / 3, avgTotalDistance[1] / 3);
            sw.WriteLine(line);
        }

        public void OutputWalkingRotation(StreamWriter sw)
        {
            StringBuilder line = new StringBuilder();
            line.AppendFormat("{0},{1}", trials[0, 0].subjectID, trials[0, 0].controlType);
            line.AppendFormat(",{0},{1},{2},{3},{4},{5}", gender, age, gamingexp, fpsexp, vrexp, mazeexp);
            float[] avgTotalRotation = { 0, 0 };
            float[] avgTotalOverShot = { 0, 0 };

            for (int i = 0; i < 3; i++) // only walking trials
            {
                for (int j = 0; j < 2; j++)
                {
                    float avgRotation = 0.0f;
                    float overshot = 0;
                    float prevAngle = 0.0f;
                    for (int k = 1; k < trials[i, j].dataList.Count; k++)
                    {
                        float angle = trials[i, j].dataList[k].orientation.y - trials[i, j].dataList[k-1].orientation.y;
                        avgRotation += (float)Math.Abs(angle);
                        if (prevAngle * angle < 0.0f && Math.Abs(angle - prevAngle) > 0.1)
                        {
                            overshot += 1;
                        }
                        prevAngle = angle;
                    }
                    avgRotation /= trials[i, j].dataList.Count - 1;
                    overshot /= trials[i, j].wayPoints.Count - 1;
                    line.AppendFormat(",{0},{1}", avgRotation, overshot);
                    avgTotalRotation[j] += avgRotation;
                    avgTotalOverShot[j] += overshot;
                }
            }
            line.AppendFormat(",{0},{1}", avgTotalRotation[0] / 3, avgTotalOverShot[0] / 3);
            line.AppendFormat(",{0},{1}", avgTotalRotation[1] / 3, avgTotalOverShot[1] / 3);
            sw.WriteLine(line);
        }

        public void OutputSegwayOvershot(StreamWriter sw)
        {
            StringBuilder line = new StringBuilder();
            line.AppendFormat("{0},{1}", trials[0, 0].subjectID, trials[0, 0].controlType);
            line.AppendFormat(",{0},{1},{2},{3},{4},{5}", gender, age, gamingexp, fpsexp, vrexp, mazeexp);
            float[] avgTotalRotation = { 0, 0 };
            float[] avgTotalOverShot = { 0, 0 };

            for (int i = 3; i < 6; i++) // only segway trials
            {
                for (int j = 0; j < 2; j++)
                {
                    float avgRotation = 0.0f;
                    float overshot = 0;
                    Vector3 prevVel = new Vector3(0, 0, 0);
                    for (int k = 1; k < trials[i, j].dataList.Count; k++)
                    {
                        float angle = trials[i, j].dataList[k].orientation.y - trials[i, j].dataList[k - 1].orientation.y;
                        avgRotation += (float)Math.Abs(angle);
                        Vector3 vel = trials[i, j].dataList[k].position.Subtract(trials[i, j].dataList[k - 1].position);
                        if (vel.x * prevVel.x + vel.z * prevVel.z < 0 && vel.GetDistance(prevVel) > 0.1f)
                        {
                            overshot++;
                        }
                        prevVel = vel;
                    }
                    avgRotation /= trials[i, j].dataList.Count - 1;
                    overshot /= trials[i, j].wayPoints.Count - 1;
                    overshot /= 2;  // back and forth
                    line.AppendFormat(",{0},{1}", avgRotation, overshot);
                    avgTotalRotation[j] += avgRotation;
                    avgTotalOverShot[j] += overshot;
                }
            }
            line.AppendFormat(",{0},{1}", avgTotalRotation[0] / 3, avgTotalOverShot[0] / 3);
            line.AppendFormat(",{0},{1}", avgTotalRotation[1] / 3, avgTotalOverShot[1] / 3);
            sw.WriteLine(line);
        }

        public void OutputSurfingDistance(StreamWriter sw)
        {
            StringBuilder line = new StringBuilder();
            line.AppendFormat("{0},{1}", trials[0, 0].subjectID, trials[0, 0].controlType);
            line.AppendFormat(",{0},{1},{2},{3},{4},{5}", gender, age, gamingexp, fpsexp, vrexp, mazeexp);
            float[] avgTotalDistance = { 0, 0 };

            for (int i = 6; i < 9; i++) // only surfing trials
            {
                for (int j = 0; j < 2; j++)
                {
                    float avgDistance = 0.0f;
                    for (int k = 1; k < trials[i, j].dataList.Count; k++)
                    {
                        float distance = trials[i, j].dataList[k].position.GetDistance(trials[i, j].dataList[k].wayPt);
                        avgDistance += distance;
                    }
                    avgDistance /= trials[i, j].dataList.Count - 1;
                    line.AppendFormat(",{0}", avgDistance);
                    avgTotalDistance[j] += avgDistance;
                }
            }
            line.AppendFormat(",{0},{1}", avgTotalDistance[0] / 3, avgTotalDistance[1] / 3);
            sw.WriteLine(line);
        }
    }
}
