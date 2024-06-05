using System;
using System.IO;

namespace TIM
{
    public class ConsoleMessage
    {
        public string Title;
        public string TitleCompact;
        public MessageType Type;
        public string Logchain;
        public Logchain LogchainReference;
        public DateTime DateTime;
        public bool StackTraceExpandable;
        public string StackTrace;
        public string StackTraceTruncated;
        public string StackTraceCompact;
        public string StackTracePreview;
        public int Index;
        public bool Hidden;

        public string GetLogchainRefreshed => string.IsNullOrWhiteSpace(Logchain) ? "" : Logchain;
        
        public int CopiesCount = 0; // count of copies of this message
        public ConsoleMessage OriginalMessage = null; // the message of which this message is a copy

        public ConsoleMessage(string title, MessageType messageType, string stackTrace, int index, string logchain, bool hidden)
        {
            Title = title;
            Hidden = hidden;
            if (Title.Length > ConsoleSetting.Instance.MaxTitleLength)
            {
                Title = Title.Substring(0, ConsoleSetting.Instance.MaxTitleLength)+"...";
            }
            TitleCompact = Title;
            if (TitleCompact.Length > 120)
            {
                TitleCompact = TitleCompact.Substring(0, 120)+"...";
            }
            using (StringReader reader = new StringReader(TitleCompact))
            {
                TitleCompact = reader.ReadLine();
                if (!string.IsNullOrEmpty(reader.ReadLine()))
                    TitleCompact += "...";
            }
            
            Type = messageType;
            DateTime = DateTime.Now;
            StackTrace = stackTrace;
            if (StackTrace.Length > ConsoleSetting.Instance.MaxStacktraceLength)
            {
                StackTrace = StackTrace.Substring(0, ConsoleSetting.Instance.MaxStacktraceLength);
                StackTrace += "\n<...>\n";
            }
            
            StackTraceTruncated = stackTrace;
            if (StackTraceTruncated.Length > 1000)
            {
                StackTraceTruncated = StackTraceTruncated.Substring(0, 1000);
                StackTraceTruncated += "\n<...>\n";
                StackTraceExpandable = true;
            }
            else
            {
                StackTraceExpandable = false;
            }

            StackTraceCompact = StackTrace;
            if (StackTraceCompact.Length > 400)
            {
                StackTraceCompact = StackTraceCompact.Substring(0, 400);
            }

            StackTracePreview = StackTraceCompact;
            if (StackTracePreview.Length > 200)
            {
                StackTracePreview = StackTracePreview.Substring(0, 200);
            }

            using (StringReader reader = new StringReader(StackTracePreview))
            {
                if(messageType != MessageType.Error)
                    reader.ReadLine();
                StackTracePreview = reader.ReadLine();
            }
            
            Index = index;
            Logchain = logchain;
            if (!string.IsNullOrEmpty(Logchain) && Logchain.Length > 60)
                Logchain = Logchain.Substring(0, 100) + "...";
        }
    }
}