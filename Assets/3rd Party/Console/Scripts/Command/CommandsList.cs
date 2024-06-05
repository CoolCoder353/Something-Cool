using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TIM
{

    public class CommandsList : MonoBehaviour
    {
        [SerializeField] private Console _console;
        [SerializeField] private Transform _content;
        [SerializeField] private CommandsListElement _elementPrefab;
        [SerializeField] private Animation _animation;
        [SerializeField] private CanvasGroup _canvasGroup;

        private bool _opened;

        private Dictionary<CmdFormula, CommandsListElement> _dictionary = new Dictionary<CmdFormula, CommandsListElement>();

        public void AddNewCommand(CmdFormula formula)
        {
            var el = Instantiate(_elementPrefab, _content);
            el.Init(formula);
            _dictionary.Add(formula, el);
        }

        public void RemoveCommand(CmdFormula formula)
        {
            if (_dictionary.ContainsKey(formula))
            {
                var go = _dictionary[formula].gameObject;
                if (go)
                    Destroy(go);
                _dictionary.Remove(formula);
            }
        }

        public void OnConsoleOpened()
        {
            RefreshVisuals();
        }

        public void Open()
        {
            _opened = true;
            gameObject.SetActive(true);
            RefreshVisuals();

            if (_console.ConsoleConfigurator.AnimationToggle.Selected)
                _animation.Play("CommandsList_open");
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        private void RefreshVisuals()
        {
            _canvasGroup.alpha = 1;
            transform.localPosition = Vector3.zero;
            RefreshCommands();
        }

        public void RefreshCommands()
        {
            foreach (KeyValuePair<CmdFormula, CommandsListElement> keyValuePair in _dictionary)
            {
                keyValuePair.Value.RefreshVisuals();
            }
        }
    }
}
