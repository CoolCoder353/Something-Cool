using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TIM
{

    public class LogchainElement : MonoBehaviour
    {
        [Header("Content"), SerializeField] private Transform _content;
        [Header("Dotted Line Prefab"), SerializeField] private GameObject _dottedLinePrefab;
        [Header("Element Prefab"), SerializeField] private LogchainElementMessageEl _elPrefab;
        [Header("Title"), SerializeField] private TMP_Text _title;
        [Header("Highlight Panel"), SerializeField] private GameObject _highlightPanel;
        [Header("Collapse Toggle")] public ConsoleToggle CollapseToggle;

        public Logchain Logchain { private set; get; }

        private List<LogchainElementMessageEl> _messageElements = new List<LogchainElementMessageEl>();
        private List<GameObject> _dottedLines = new List<GameObject>();

        public void Init(Logchain logchain)
        {
            Logchain = logchain;
            Logchain.LogchainElement = this;
            _title.text = logchain.Title;
        }

        public void TryAddMessage(ConsoleMessage consoleMessage)
        {
            if (CollapseToggle.Selected && consoleMessage.OriginalMessage != null)
            {
                var el = GetElement(consoleMessage.OriginalMessage);
                el.RefreshCount();
            }
            else
            {
                SpawnNewMessageElement(consoleMessage);
            }
        }

        private void SpawnNewMessageElement(ConsoleMessage consoleMessage)
        {
            if (_messageElements.Count > 0)
            {
                var line = Instantiate(_dottedLinePrefab, _content);
                _dottedLines.Add(line);
            }
            var el = Instantiate(_elPrefab, _content);
            el.Init(consoleMessage, this);
            _messageElements.Add(el);
        }

        public void Repaint()
        {
            for (int i = _messageElements.Count - 1; i >= 0; i--)
            {
                Destroy(_messageElements[i].gameObject);
            }

            for (int i = _dottedLines.Count - 1; i >= 0; i--)
            {
                Destroy(_dottedLines[i]);
            }

            _messageElements.Clear();
            _dottedLines.Clear();

            foreach (var consoleMessage in Logchain.ConsoleMessages)
            {
                if (CollapseToggle.Selected)
                {
                    if (consoleMessage.OriginalMessage == null)
                        SpawnNewMessageElement(consoleMessage);
                }
                else
                {
                    SpawnNewMessageElement(consoleMessage);
                }
            }
        }

        public void SetHighlight(bool highlight)
        {
            _highlightPanel.SetActive(highlight);
        }

        private LogchainElementMessageEl GetElement(ConsoleMessage consoleMessage)
        {
            foreach (LogchainElementMessageEl el in _messageElements)
            {
                if (el.ConsoleMessage == consoleMessage)
                    return el;
            }

            return null;
        }
    }
}