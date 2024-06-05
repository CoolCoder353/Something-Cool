using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TIM
{

    public class ConsoleDownPanel : MonoBehaviour
    {
        [SerializeField] private Transform _content;
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private ConsoleButton _sendButton;

        private List<ConsoleTipElement> _consoleTipElements = new List<ConsoleTipElement>();

        public void OnConsoleOpened()
        {
            _inputField.text = "";

        }

        private void RefreshTips()
        {
            string myText = _inputField.text;
        }
    }
}
