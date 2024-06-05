using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TIM
{

    public class LogchainWindow : MonoBehaviour
    {
        public TMP_InputField Input_Search;

        [SerializeField] private Transform _content;
        [SerializeField] private LogchainElement _elementPrefab;

        private List<LogchainElement> _logchainElements = new List<LogchainElement>();
        private Coroutine _searchingCoroutine;

        public void AddLogchain(Logchain logchain)
        {
            var el = Instantiate(_elementPrefab, _content);
            el.Init(logchain);
            _logchainElements.Add(el);
        }

        public void OnInputSearchChanged()
        {
            if (_searchingCoroutine != null)
            {
                StopCoroutine(_searchingCoroutine);
                _searchingCoroutine = null;
            }
            _searchingCoroutine = StartCoroutine(SearchAsync(Input_Search.text.ToLower()));
        }

        public void SetOpened(bool opened)
        {
            gameObject.SetActive(opened);
        }

        private IEnumerator SearchAsync(string title)
        {
            bool isNullOrWhiteSpace = string.IsNullOrWhiteSpace(title);

            int loops = 0;
            foreach (LogchainElement logchainElement in _logchainElements)
            {
                if (isNullOrWhiteSpace)
                {
                    logchainElement.gameObject.SetActive(true);
                    logchainElement.SetHighlight(false);
                }
                else
                {
                    string lowTitle = logchainElement.Logchain.Title.ToLower();
                    bool contains = lowTitle.Contains(title);
                    logchainElement.gameObject.SetActive(contains);
                    logchainElement.SetHighlight(contains && lowTitle == title);
                }

                loops++;
                if (loops > 5)
                    yield return null;
            }
        }

        public void Clear()
        {
            if (_searchingCoroutine != null)
            {
                StopCoroutine(_searchingCoroutine);
                _searchingCoroutine = null;
            }

            for (int i = _logchainElements.Count - 1; i >= 0; i--)
            {
                Destroy(_logchainElements[i].gameObject);
            }

            _logchainElements.Clear();
        }
    }
}
