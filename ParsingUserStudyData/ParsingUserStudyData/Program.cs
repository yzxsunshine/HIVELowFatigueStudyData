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
        static void OutputTimeTable(List<SubjectData> subjects)
        {
            StreamWriter sw = new StreamWriter("../../../../VE_data/performance_data/time.csv");
            string line = "SubjectID,Condition,Gender,Age,GamingExperience,FPSGameExperience,VRExperience,GoodAtMaze";
            line += ",WalkingEasyTime1,WalkingEasyTime2,WalkingMediumTime1,WalkingMediumTime2,WalkingHardTime1,WalkingHardTime2";
            line += ",SegwayEasyTime1,SegwayEasyTime2,SegwayMediumTime1,SegwayMediumTime2,SegwayHardTime1,SegwayHardTime2";
            line += ",SurfingEasyTime1,SurfingEasyTime2,SurfingMediumTime1,SurfingMediumTime2,SurfingHardTime1,SurfingHardTime2";
            line += ",WalkingTime1,WalkingTime2";
            line += ",SegwayTime1,SegwayTime2";
            line += ",SurfingTime1,SurfingTime2";
            sw.WriteLine(line);
            for (int i = 0; i < subjects.Count; i++)
            {
                subjects[i].OutputStudyTime(sw);
            }
            sw.Flush();
            sw.Close();
        }

        static void OutputCollisionTable(List<SubjectData> subjects)
        {
            StreamWriter sw = new StreamWriter("../../../../VE_data/performance_data/segway_collision.csv");
            string line = "SubjectID,Condition,Gender,Age,GamingExperience,FPSGameExperience,VRExperience,GoodAtMaze";
            line += ",#EasyColllision_1,#EasyColllision_2";
            line += ",#MediumColllision_1,#MediumColllision_2";
            line += ",#HardColllision_1,#HardColllision_2";
            line += ",#AvgColllision_1,#AvgHardColllision_2";
            sw.WriteLine(line);
            for (int i = 0; i < subjects.Count; i++)
            {
                subjects[i].OutputSegwayCollision(sw);
            }
            sw.Flush();
            sw.Close();
        }

        static void OutputHeatMap(List<SubjectData> subjects)
        {
            StreamWriter sw = new StreamWriter("../../../../VE_data/performance_data/heatmap.csv");
            string line = "SubjectID,Condition";
            for (int i = 0; i < 36; i++)
            {
                line += "," + i * 10;
            }
            sw.WriteLine(line);
            for (int i = 0; i < subjects.Count; i++)
            {
                subjects[i].OutputHeatMap(sw);
            }
            sw.Flush();
            sw.Close();
        }

        static void OutputTrainingTable(List<SubjectData> subjects)
        {
            StreamWriter sw = new StreamWriter("../../../../VE_data/performance_data/training.csv");
            string line = "SubjectID,Condition,Gender,Age,GamingExperience,FPSGameExperience,VRExperience,GoodAtMaze";
            line += ",WalkingTrial,WalkingTime,SegwayTrial,SegwayTime,SurfingTrial,SurfingTime";
            line += ",TotalTrials,TotalTime";
            sw.WriteLine(line);
            for (int i = 0; i < subjects.Count; i++)
            {
                subjects[i].OutputTrainingData(sw);
            }
            sw.Flush();
            sw.Close();
        }

        static void OutputModeSwitch(List<SubjectData> subjects)
        {
            StreamWriter sw = new StreamWriter("../../../../VE_data/performance_data/modeSwitch.csv");
            string line = "SubjectID,Condition,Gender,Age,GamingExperience,FPSGameExperience,VRExperience,GoodAtMaze";
            line += ",WalkingEasyResponseTime_1,WalkingEasyResponseTime_2";
            line += ",WalkingMediumResponseTime_1,WalkingMediumResponseTime_2";
            line += ",WalkingHardResponseTime_1,WalkingHardResponseTime_2";
            line += ",SegwayEasyResponseTime_1,SegwayEasyResponseTime_2";
            line += ",SegwayMediumResponseTime_1,SegwayMediumResponseTime_2";
            line += ",SegwayHardResponseTime_1,SegwayHardResponseTime_2";
            line += ",SurfingEasyResponseTime_1,SurfingEasyResponseTime_2";
            line += ",SurfingMediumResponseTime_1,SurfingMediumResponseTime_2";
            line += ",SurfingHardResponseTime_1,SurfingHardResponseTime_2";
            line += ",WalkingAvgResponseTime_1,WalkingAvgResponseTime_2";
            line += ",SegwayAvgResponseTime_1,SegwayAvgResponseTime_2";
            line += ",SurfingAvgResponseTime_1,SurfingAvgResponseTime_2";
            line += ",TotalAvgResponseTime_1,TotalAvgResponseTime_2";
            sw.WriteLine(line);
            for (int i = 0; i < subjects.Count; i++)
            {
                subjects[i].OutputStudyModeSwitch(sw);
            }
            sw.Flush();
            sw.Close();
        }

        static void OutputSurfingLanding(List<SubjectData> subjects)
        {
            StreamWriter sw = new StreamWriter("../../../../VE_data/performance_data/surfing_landing.csv");
            string line = "SubjectID,Condition,Gender,Age,GamingExperience,FPSGameExperience,VRExperience,GoodAtMaze";
            line += ",Surfing1_1,Surfing2_1,Surfing3_1";
            line += ",Surfing1_2,Surfing2_2,Surfing3_2";
            line += ",SurfingAvgDistance_1,SurfingAvgDistance_2";
            sw.WriteLine(line);
            
            for (int i = 0; i < subjects.Count; i++)
            {
                subjects[i].OutputSurfingLanding(sw);
            }
            sw.Flush();
            sw.Close();
        }

        static void OutputWalkingPathDistance(List<SubjectData> subjects)
        {
            StreamWriter sw = new StreamWriter("../../../../VE_data/performance_data/walking_path.csv");
            string line = "SubjectID,Condition,Gender,Age,GamingExperience,FPSGameExperience,VRExperience,GoodAtMaze";
            line += ",AverageDistanceToWayPointEasy_1,AverageDistanceToPathEasy_1";
            line += ",AverageDistanceToWayPointEasy_2,AverageDistanceToPathEasy_2";
            line += ",AverageDistanceToWayPointMedium_1,AverageDistanceToPathMedium_1";
            line += ",AverageDistanceToWayPointMedium_2,AverageDistanceToPathMedium_2";
            line += ",AverageDistanceToWayPointHard_1,AverageDistanceToPathHard_1";
            line += ",AverageDistanceToWayPointHard_2,AverageDistanceToPathHard_2";
            line += ",AverageDistanceToWayPointTotal_1,AverageDistanceToPathTotal_1";
            line += ",AverageDistanceToWayPointTotal_2,AverageDistanceToPathTotal_2";
            sw.WriteLine(line);

            for (int i = 0; i < subjects.Count; i++)
            {
                subjects[i].OutputWalkingPathDistance(sw);
            }
            sw.Flush();
            sw.Close();
        }

        static void OutputWalkingRotation(List<SubjectData> subjects)
        {
            StreamWriter sw = new StreamWriter("../../../../VE_data/performance_data/walking_rotation.csv");
            string line = "SubjectID,Condition,Gender,Age,GamingExperience,FPSGameExperience,VRExperience,GoodAtMaze";
            line += ",WalkingEasyRotation_1,WalkingEasyOverRotate_1";
            line += ",WalkingEasyRotation_2,WalkingEasyOverRotate_2";
            line += ",WalkingMediumRotation_1,WalkingMediumOverRotate_1";
            line += ",WalkingMediumRotation_2,WalkingMediumOverRotate_2";
            line += ",WalkingHardRotation_1,WalkingHardOverRotate_1";
            line += ",WalkingHardRotation_2,WalkingHardOverRotate_2";
            line += ",AverageRotation_1,AverageOverRotate_1";
            line += ",AverageRotation_2,AverageOverRotate_2";
            sw.WriteLine(line);

            for (int i = 0; i < subjects.Count; i++)
            {
                subjects[i].OutputWalkingRotation(sw);
            }
            sw.Flush();
            sw.Close();
        }

        static void OutputSegwayOvershot(List<SubjectData> subjects)
        {
            StreamWriter sw = new StreamWriter("../../../../VE_data/performance_data/segway_overshot.csv");
            string line = "SubjectID,Condition,Gender,Age,GamingExperience,FPSGameExperience,VRExperience,GoodAtMaze";
            line += ",SegwayEasyRotation_1,SegwayEasyOvershot_1";
            line += ",SegwayEasyRotation_2,SegwayEasyOvershot_2";
            line += ",SegwayMediumRotation_1,SegwayMediumOvershot_1";
            line += ",SegwayMediumRotation_2,SegwayMediumOvershot_2";
            line += ",SegwayHardRotation_1,SegwayHardOvershot_1";
            line += ",SegwayHardRotation_2,SegwayHardOvershot_2";
            line += ",AverageRotation_1,AverageOvershot_1";
            line += ",AverageRotation_2,AverageOvershot_2";
            sw.WriteLine(line);

            for (int i = 0; i < subjects.Count; i++)
            {
                subjects[i].OutputSegwayOvershot(sw);
            }
            sw.Flush();
            sw.Close();
        }

        static void OutputSurfingDistance(List<SubjectData> subjects)
        {
            StreamWriter sw = new StreamWriter("../../../../VE_data/performance_data/surfing_distance.csv");
            string line = "SubjectID,Condition,Gender,Age,GamingExperience,FPSGameExperience,VRExperience,GoodAtMaze";
            line += ",SurfingEasyDistance_1";
            line += ",SurfingEasyDistance_2";
            line += ",SurfingMediumDistance_1";
            line += ",SurfingMediumDistance_2";
            line += ",SurfingHardDistance_1";
            line += ",SurfingHardDistance_2";
            line += ",AverageDistance_1";
            line += ",AverageDistance_2";
            sw.WriteLine(line);

            for (int i = 0; i < subjects.Count; i++)
            {
                subjects[i].OutputSurfingDistance(sw);
            }
            sw.Flush();
            sw.Close();
        }

        static void Main(string[] args)
        {
            string rootPath = "../../../../VE_data";
            List<SubjectData> subjects = new List<SubjectData>();
            int[] gender = new int[] { 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 0, 1, 0, 0, 1, 1, 1, 1, 0, 1, 1, 0 };
            int[] age = new int[] { 35, 32, 18, 19, 20, 21, 22, 21, 20, 20, 20, 20, 21, 22, 18, 21, 22, 21, 19, 19, -1, 24, 18, 19, 18, 21, 19, 20, 21, 19, 19, 19 };
            int[] gamingexp = new int[] { 3, 6, 5, 2, 4, 5, 2, 3, 5, 5, 2, 4, 4, 1, 2, 1, 4, 4, 4, 3, 5, 1, 4, 5, 3, 2, 1, 1, 4, 1, 1, 1 };
            int[] fpsexp = new int[] { 3, 2, 4, 1, 4, 4, 1, 2, 4, 4, 2, 4, 2, 1, 2, 1, 4, 1, 3, 3, 5, 1, 4, 5, 2, 1, 1, 1, 4, 1, 1, 1 };
            int[] vrexp = new int[] { 1, 1, 4, 1, 1, 2, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 4, 3, 3, 1, 2, 1, 3, 1, 1, 1, 1, 1, 1, 1 };
            int[] mazeexp = new int[] { 4, 4, 5, 4, 4, 4, 2, 4, 4, 4, 3, 4, 4, 1, 3, 4, 3, 3, 2, 3, 2, 4, 4, 4, 4, 3, 2, 2, 4, 2, 5, 5 };
            for (int i = 0; i < 32; i++)
            {
                Console.WriteLine(String.Format("Processing Subject {0}", i));
                string directoryPath = String.Format("{0}/subject_{1}", rootPath, i);
                string[] fileEntries = Directory.GetFiles(directoryPath);
                SubjectData subjectData = new SubjectData(i, gender[i], age[i], gamingexp[i], fpsexp[i], vrexp[i], mazeexp[i]);
                if (subjectData.IsValid())
                {
                    foreach (string fileName in fileEntries)
                    {
                        subjectData.AddTrial(fileName);
                    }
                    subjects.Add(subjectData);
                }
            }
            //OutputTimeTable(subjects);
            //OutputCollisionTable(subjects);
            //OutputTrainingTable(subjects);
            OutputHeatMap(subjects);
            //OutputModeSwitch(subjects);
            //OutputSurfingLanding(subjects);
            //OutputWalkingPathDistance(subjects);
            //OutputWalkingRotation(subjects);
            //OutputSegwayOvershot(subjects);
            //OutputSurfingDistance(subjects);
        }
    }
}
