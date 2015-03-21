using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomInGameTime
{
    class Program
    {
        public class InGameDate{
            public int Days {get; set;}
            public int Hour {get; set;}
            public int Min {get; set;}

            public string ToString(bool ampm = false)
            {
                if (!ampm)
                    return string.Format("{0:00} {1:00}:{2:00}", this.Days, this.Hour, this.Min);
                string lblAmPm = "AM";
                if (this.Hour >= 12)
                    lblAmPm = "PM";
                int hr12 = this.Hour;
                if (this.Hour == 0)
                    hr12 = 12;
                if (this.Hour >= 13)
                    hr12 = this.Hour - 12;
                return string.Format("{0:00} {1:00}:{2:00} {3}", this.Days, hr12, this.Min, lblAmPm);
            }

#if DEBUG
            public double CorrespondingTimeStamp { get; set; }
#endif
        }

        public class InGameTime
        {
            /// <summary>
            /// Total of real seconds that represent a InGame Day
            /// </summary>
            private const float InGameDay_RealTimeSpan = 7200;
            
            private double TimeStamp;

            public InGameDate CurrentInGameDate
            {
                get
                {
                    InGameDate gameDate = new InGameDate();
                    gameDate.Days = (int)Math.Floor(this.TimeStamp / InGameDay_RealTimeSpan);
                    double secondsRemaining = this.TimeStamp % InGameDay_RealTimeSpan;
                    float lerp = MathS.SuperLerp(0.0f, 24.0f, 0.0f, InGameDay_RealTimeSpan, (float)secondsRemaining);
                    gameDate.Hour = (int)Math.Floor(lerp);
                    float hrFraction = lerp - gameDate.Hour;
                    gameDate.Min = (int)Math.Floor(hrFraction * 60.0f);
                    gameDate.CorrespondingTimeStamp = this.TimeStamp;
                    return gameDate;
                }
            }

            public InGameTime()
            {
                this.TimeStamp = CalculatesTimeStamp(DateTime.Now);
            }
#if DEBUG
            public InGameTime(DateTime customDate)
            {
                this.TimeStamp = CalculatesTimeStamp(customDate);
            }
#endif

            public void Update(float TimeDeltaTime)
            {
                this.TimeStamp += TimeDeltaTime;
            }

            //private static DateTime EPOCH = new DateTime(1970, 1, 1, 0, 0, 0);
            private static DateTime EPOCH = new DateTime(2015, 3, 21, 0, 0, 0); //lets use a more recent date for test purposes (might be useful to change it to product launch)
            public static double CalculatesTimeStamp(DateTime realTime)
            {
                TimeSpan span = realTime - EPOCH;
                return span.TotalSeconds;
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("CustomInGameTime Test");
            InGameTime inGameTimeEngine = new InGameTime(DateTime.Today);
            Console.Write("Using Today ({0}) inGame Time is ", DateTime.Today);
            Print(inGameTimeEngine.CurrentInGameDate);
            Console.WriteLine();
            Console.WriteLine("stamp\t  24hr           12hr  ");
            for (int i = 0; i < 1440; i++)
            {
                inGameTimeEngine.Update(60.0f);//adding 60 seconds to the timestamp
                Print(inGameTimeEngine.CurrentInGameDate);
            }
            Console.WriteLine("CustomInGameTime Test Finished");
            Console.ReadLine();
        }

        private static void Print(InGameDate gameDate)
        {
            Console.WriteLine("{0:00000}\t{1}  -->  {2}", gameDate.CorrespondingTimeStamp, gameDate.ToString(false), gameDate.ToString(true));
        }

        /// <summary>
        /// Source http://wiki.unity3d.com/index.php?title=SpeedLerp
        /// </summary>
        public static class MathS
        {
            public static float SuperLerp(float from, float to, float from2, float to2, float value)
            {
                if (from2 < to2)
                {
                    if (value < from2)
                        value = from2;
                    else if (value > to2)
                        value = to2;
                }
                else
                {
                    if (value < to2)
                        value = to2;
                    else if (value > from2)
                        value = from2;
                }
                return (to - from) * ((value - from2) / (to2 - from2)) + from;
            }
        }
    }
}