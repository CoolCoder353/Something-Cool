using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace TIM
{

    public class ConsoleTipMaster : MonoBehaviour
    {
        [BoxGroup("BoxGroup"), SerializeField] private ConsoleInputPanel _consoleInputPanel;
        [BoxGroup("BoxGroup"), SerializeField] private Transform _content;
        [BoxGroup("BoxGroup"), SerializeField] private ConsoleTipElement _tipElementPrefab;

        private List<ConsoleTipElement> _tipElements = new List<ConsoleTipElement>();
        public CmdInputResult PerfectlySuitedResult { set; get; }
        public int HighlightedElement { private set; get; } = -1;
        public CmdInputResult GetHighlightedResult => HighlightedElement < 0 || HighlightedElement >= _tipElements.Count ? null : _tipElements[HighlightedElement].CmdInputResult;

        public void RefreshTips(string input)
        {
            List<CmdInputResult> foundResults = new List<CmdInputResult>();

            if (string.IsNullOrWhiteSpace(input))
            {
                HighlightedElement = -1;
            }
            else
            {
                // find formulas:
                var dictionary = Console.RegisteredCommands;
                PerfectlySuitedResult = null;
                foreach (var keyValuePair in dictionary)
                {
                    CmdInputResult cmdInputResult = new CmdInputResult();
                    if (cmdInputResult.TryInit(input, keyValuePair.Key, false))
                    {
                        PerfectlySuitedResult = cmdInputResult;
                    }
                    if (cmdInputResult.PotentiallySuitable && foundResults.Count < 5)
                        foundResults.Add(cmdInputResult);

                    if (foundResults.Count == 5 && PerfectlySuitedResult != null)
                        break;
                }
            }

            // refresh list:
            for (int i = _tipElements.Count - 1; i >= 0; i--)
            {
                Destroy(_tipElements[i].gameObject);
            }
            _tipElements.Clear();

            foreach (CmdInputResult cmdInputResult in foundResults)
            {
                var el = Instantiate(_tipElementPrefab, _content);
                el.Init(cmdInputResult, _tipElements.Count, this);
                _tipElements.Add(el);
                if (el.Index == HighlightedElement)
                    el.SetHighlight(true);
            }

            if (_tipElements.Count > 0)
            {
                if (HighlightedElement < 0)
                    HighlightElement(0);
                else if (HighlightedElement >= _tipElements.Count)
                    HighlightElement(_tipElements.Count - 1);
            }
        }

        public void HighlightElement(int index)
        {
            HighlightedElement = index;
            for (int i = 0; i < _tipElements.Count; i++)
            {
                _tipElements[i].SetHighlight(i == index);
            }

            _consoleInputPanel.RefreshHelpText();
        }

        public void HighlightNextElement()
        {
            HighlightedElement++;
            if (HighlightedElement >= _tipElements.Count)
                HighlightedElement = 0;

            HighlightElement(HighlightedElement);
        }

        public void HighlightPreviousElement()
        {
            HighlightedElement--;
            if (HighlightedElement < 0)
                HighlightedElement = _tipElements.Count - 1;

            HighlightElement(HighlightedElement);
        }
    }
}