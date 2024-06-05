using System.Collections.Generic;

namespace TIM
{
    public class Logchain
    {
        public string Title;
        public int Index;
        public LogchainElement LogchainElement;
        public List<ConsoleMessage> ConsoleMessages = new List<ConsoleMessage>();

        public Logchain(string title, int index)
        {
            Title = title;
            Index = index;
        }

        public void RegisterNewMessage(ConsoleMessage consoleMessage)
        {
            ConsoleMessages.Add(consoleMessage);
            LogchainElement.TryAddMessage(consoleMessage);
        }
    }
}