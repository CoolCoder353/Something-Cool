using System.Linq;
using UnityEngine;

namespace TIM
{
    /// <summary>
    /// Algorithms for Console's code.
    /// </summary>
    public static class ConsoleAlgorithms
    {
        /// <summary>
        /// if input value is less or equal 999 returns this value. If value is larger than 999, returns 999+
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string GetCountString(int count)
        {
            if (count > 999)
                return "999+";
            else
                return count.ToString();
        }
        
        /// <summary>
        /// You can highlight file paths (Folder/Script.cs) and code paths (TIM.Editor.ConsoleSettingEditor) with Rich text
        /// </summary>
        /// <param name="stacktrace">text which contains file paths and code paths. In console it was stacktrace</param>
        /// <returns></returns>
        public static string GetColoredStacktrace(string stacktrace)
        {
            string[] lines = stacktrace.Split('\n');

            string result = "";
            
            foreach (string line in lines)
            {
                string[] words = line.Split(' ');
                foreach (string word in words)
                {
                    result += GetColoredWord(word) + " ";
                }

                result += "\n\n";
            }

            return result;
            
            string GetColoredWord(string input)
            {
                string result;
                if(input.Contains('/') || input.Contains('\\'))
                {
                    result = $"<color=#{ColorUtility.ToHtmlStringRGB(ConsoleSetting.Instance.StacktraceFilePathColor)}>" + input + "</color>";
                }
                else if (IsCodePath())
                {
                    result = $"<color=#{ColorUtility.ToHtmlStringRGB(ConsoleSetting.Instance.StacktraceCodePathColor)}>" + input + "</color>";
                }
                else
                {
                    result = $"<color=#{ColorUtility.ToHtmlStringRGB(ConsoleSetting.Instance.StacktraceDefaultColor)}>" + input + "</color>";
                }

                return result;

                bool IsCodePath()
                {
                    int dotCount = input.Count(c => c == '.');
                    if (dotCount > 1)
                        return true;

                    if (dotCount > 0 && input[0] != '.' && input[^1] != '.')
                        return true;

                    return false;
                }
            }
        }

        public static string GetAutoCompleteHelpString(string current, string target)
        {
            if (current == null || string.IsNullOrEmpty(target) || current.Length > target.Length)
                return current;
            
            return current + target.Substring(current.Length, target.Length - current.Length);
        }
        
        /// <summary>
        /// returns true if the messages are equal
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool CompareMessages(ConsoleMessage a, ConsoleMessage b)
        {
            return a.Type == b.Type
                   && a.Title.Equals(b.Title)
                   && a.GetLogchainRefreshed.Equals(b.GetLogchainRefreshed)
                   && a.StackTraceCompact.Equals(b.StackTraceCompact);
        }
    }
}