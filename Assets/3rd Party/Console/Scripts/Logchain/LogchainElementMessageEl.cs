using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TIM
{
    public class LogchainElementMessageEl : MonoBehaviour
    {
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _countText;
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _downlineImage;
        [SerializeField] private Image _highlightImage;
        
        public ConsoleMessage ConsoleMessage { private set; get; }
        private LogchainElement _logchainElement;

        public void Init(ConsoleMessage consoleMessage, LogchainElement logchainElement)
        {
            ConsoleMessage = consoleMessage;
            _titleText.text = consoleMessage.TitleCompact;
            _iconImage.sprite = ConsoleSetting.Instance.GetIcon(consoleMessage.Type);
            _iconImage.color = ConsoleSetting.Instance.GetColor(consoleMessage.Type);
            _downlineImage.color = ConsoleSetting.Instance.GetColor(consoleMessage.Type);
            _highlightImage.color = ConsoleSetting.Instance.GetColor(consoleMessage.Type);
            _logchainElement = logchainElement;
            _countText.color = ConsoleSetting.Instance.GetColor(consoleMessage.Type);
            
            RefreshCount();
        }

        public void RefreshCount()
        {
            if (_logchainElement.CollapseToggle.Selected)
            {
                _countText.gameObject.SetActive(ConsoleMessage.CopiesCount > 0);
                _countText.text = ConsoleAlgorithms.GetCountString(ConsoleMessage.CopiesCount+1);
            }
            else
            {
                _countText.gameObject.SetActive(false);
            }
        }

        public void OnClick()
        {
            Console.Instance.MessageInspector.Open(ConsoleMessage);
        }
    }
}