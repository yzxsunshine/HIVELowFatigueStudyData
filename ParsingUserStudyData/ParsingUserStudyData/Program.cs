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
            sw.WriteLine("SubjectID,Condition,TravelType,Level,Duration_1,Duration_2");
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
            sw.WriteLine("SubjectID,Condition,Level,#Colllision_1,#Colllision_2");
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
            string line = "SubjectID,Condition,Level,Pass";
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
            sw.WriteLine("SubjectID,Condition,TravelType,#Pass,Time");
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
            sw.WriteLine("SubjectID,Condition,TravelType,Level,ResponseTime_1,#InitIncorrectSwitch_1,#InitIncorrectWalking_1,#InitIncorrectSegway_1,#InitIncorrectSurfing_1,ResponseTime_2,#InitIncorrectSwitch_2,#InitIncorrectWalking_2,#InitIncorrectSegway_2,#InitIncorrectSurfing_2");
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
            sw.WriteLine("SubjectID,Condition,Level,DistanceToGoal_1,OverShot_1,SheerOff_1,DistanceToGoal_2,OverShot_2,SheerOff_2");
            for (int i = 0; i < subjects.Count; i++)
            {
                subjects[i].OutputSurfingLanding(sw);
            }
            sw.Flush();
            sw.Close();
        }

        static void Main(string[] args)
        {
            string rootPath = "../../../../VE_data";
            List<SubjectData> subjects = new List<SubjectData>();
            for (int i = 0; i < 32; i++)
            {
                Console.WriteLine(String.Format("Processing Subject {0}", i));
                string directoryPath = String.Format("{0}/subject_{1}", rootPath, i);
                string[] fileEntries = Directory.GetFiles(directoryPath);
                SubjectData subjectData = new SubjectData(i);
                if (subjectData.IsValid())
                {
                    foreach (string fileName in fileEntries)
                    {
                        subjectData.AddTrial(fileName);
                    }
                    subjects.Add(subjectData);
                }
            }
            OutputTimeTable(subjects);
            OutputCollisionTable(subjects);
            OutputTrainingTable(subjects);
            OutputHeatMap(subjects);
            OutputModeSwitch(subjects);
            OutputSurfingLanding(subjects);
        }
    }
}
