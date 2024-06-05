using System;
using TMPro;
using UnityEngine;
using NaughtyAttributes;

namespace TIM
{
    public class ConsoleConfigurator : MonoBehaviour
    {
        [Foldout("Links")] public Console Console;
        [Foldout("Links")] public TMP_Text ErrorsCount;
        [Foldout("Links")] public TMP_Text WarningsCount;
        [Foldout("Links")] public TMP_Text NetworkCount;
        [Foldout("Links")] public TMP_Text DefaultCount;
        [Foldout("Links")] public ConsoleToggle ErrorToggle;
        [Foldout("Links")] public ConsoleToggle WarningToggle;
        [Foldout("Links")] public ConsoleToggle NetworkToggle;
        [Foldout("Links")] public ConsoleToggle DefaultToggle;
        [Foldout("Links")] public ConsoleToggle AnimationToggle;
        [Foldout("Links")] public ConsoleToggle HiddenToggle;
        [Foldout("Links")] public ConsoleToggle CollapseToggle;
        [Foldout("Links")] public ConsoleToggle LogchainToggle;

        public static ConsoleConfigurator Instance => Console.Instance.ConsoleConfigurator;

        public bool IsCategoryActive(MessageType messageType)
        {
            switch (messageType)
            {
                case MessageType.Default: return DefaultToggle.Selected;
                case MessageType.Error: return ErrorToggle.Selected;
                case MessageType.Warning: return WarningToggle.Selected;
                case MessageType.Network: return NetworkToggle.Selected;
                default:
                    throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null);
            }
        }

        public void OnMessageCountChanged()
        {
            ErrorsCount.text = ConsoleAlgorithms.GetCountString(Console.MessageCount_Error);
            WarningsCount.text = ConsoleAlgorithms.GetCountString(Console.MessageCount_Warning);
            NetworkCount.text = ConsoleAlgorithms.GetCountString(Console.MessageCount_Network);
            DefaultCount.text = ConsoleAlgorithms.GetCountString(Console.MessageCount_Default);
        }

        public void OnSelectedCategoriesChanged()
        {
            Console.MessagesContent.OnSelectedCategoriesChanged();
        }

        public void OnLogchainToggle()
        {
            Console.LogchainWindow.SetOpened(LogchainToggle.Selected);
        }

        public void OnCollapseToggle()
        {
            Console.MessagesContent.Repaint();
        }

        public void OnHiddenToggle()
        {
            Console.MessagesContent.Repaint();
        }

        public void OnAnimationsToggle()
        {

        }
    }
}