using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace TIM
{

    public class ConsoleMessagesContent : MonoBehaviour
    {
        public Console Console;
        public Transform Content;
        public VerticalLayoutGroup VerticalLayoutGroup;
        public ConsoleMessageElement ElementPrefab;

        private List<ConsoleMessageElement> _elementsList = new List<ConsoleMessageElement>();
        private bool Collapse => Console.ConsoleConfigurator.CollapseToggle.Selected;

        public void Repaint()
        {
            for (int i = _elementsList.Count - 1; i >= 0; i--)
            {
                if (_elementsList[i] != null)
                    Destroy(_elementsList[i].gameObject);
            }

            _elementsList.Clear();

            bool collapse = Collapse;
            bool hideHidden = Console.ConsoleConfigurator.HiddenToggle.Selected;

            foreach (ConsoleMessage consoleMessage in Console.MessagesList)
            {
                if (consoleMessage.Hidden && hideHidden)
                    continue;

                if (collapse)
                {
                    if (consoleMessage.OriginalMessage == null)
                        SpawnNewMessage(consoleMessage, false);
                }
                else
                {
                    SpawnNewMessage(consoleMessage, false);
                }
            }
        }

        public void OnSelectedCategoriesChanged()
        {
            foreach (ConsoleMessageElement element in _elementsList)
            {
                element.RefreshCategoryActivity();
            }
        }

        public void TryAddMessage(ConsoleMessage consoleMessage)
        {
            if (consoleMessage.Hidden && Console.ConsoleConfigurator.HiddenToggle.Selected)
                return;

            if (Collapse && consoleMessage.OriginalMessage != null)
            {
                var el = FindElementByMessageIndex(consoleMessage.OriginalMessage.Index);
                if (el)
                {
                    el.RefreshCount();
                }
                else
                {
                    SpawnNewMessage(consoleMessage.OriginalMessage, true);
                }
            }
            else
            {
                SpawnNewMessage(consoleMessage, true);
            }
        }

        private void SpawnNewMessage(ConsoleMessage consoleMessage, bool animate)
        {
            var el = Instantiate(ElementPrefab, Content);
            _elementsList.Add(el);
            el.Init(consoleMessage, animate);
        }

        private ConsoleMessageElement FindElementByMessageIndex(int messageIndex)
        {
            foreach (ConsoleMessageElement element in _elementsList)
            {
                if (element.ConsoleMessage.Index == messageIndex)
                    return element;
            }

            return null;
        }
    }
}