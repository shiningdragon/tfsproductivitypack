using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ShiningDragon.TFSProd.Common
{
    public class Utilities
    {
        public static bool Poll(Func<bool> breakCondition, int waitLoops, int waitTime)
        {
            bool result = false;
            while (waitLoops > 0 && !(result = breakCondition()))
            {
                --waitLoops;
                Thread.Sleep(waitTime);
            }
            return result;
        }

        public static void DispatcherPoll(Func<bool> condition, Action action, int waitLoops, int waitTime)
        {
            int i = 0;
            var timer = new DispatcherTimer()
            {
                Interval = new TimeSpan(waitTime),
                Tag = 0
            };
            timer.Tick += (sender, args) =>
            {
                if (i == waitLoops)
                {
                    timer.Stop();
                }
                else
                {
                    if (condition())
                    {
                        timer.Stop();
                        action();
                    }
                    else
                    {
                        ++i;
                    }
                }
            };
            timer.Start();
        }

        public static string ReplaceTFSServerPaths(string input, string pattern, string replacement)
        {
            string[] list = pattern.Split(new char[] { '/', '\\' });
            if (list[0] != "$")
            {
                throw new ApplicationException(string.Format("ReplaceTFSServerPaths: pattern {0} is not a valid tfs server path", pattern));
            }
            string regexPattern = "[$]";
            for (int i = 1; i < list.GetLength(0); ++i)
            {
                regexPattern += string.Format(@"(\\|/){0}", Regex.Escape(list[i]));
            }
            string output = Regex.Replace(input, regexPattern, replacement, RegexOptions.IgnoreCase);

            return output;
        }
    }
}
