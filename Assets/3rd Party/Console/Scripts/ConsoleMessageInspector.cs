using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
namespace TIM
{
    public class ConsoleMessageInspector : MonoBehaviour
    {
        [BoxGroup("Params"), SerializeField] private TMP_Text _titleText;
        [BoxGroup("Params"), SerializeField] private TMP_Text _stacktraceText;
        [BoxGroup("Params"), SerializeField] private GameObject _logchainButton;
        [BoxGroup("Params"), SerializeField] private TMP_Text _logchainText;
        [BoxGroup("Params"), SerializeField] private Image _bgImage;
        [BoxGroup("Params"), SerializeField] private Image _downlineImage;
        [BoxGroup("Params"), SerializeField] private CanvasGroup _canvasGroup;
        [BoxGroup("Params"), SerializeField] private Animation _animation;
        [BoxGroup("Params"), SerializeField] private Image _openLogchainBgImage;
        [BoxGroup("Params"), SerializeField] private Image _openLogchainHighlightImage;
        [BoxGroup("Params"), SerializeField] private Animation _copyButtonAnimation;
        [BoxGroup("Params"), SerializeField] private CanvasGroup _closeBtnCanvasGroup;
        [BoxGroup("Params"), SerializeField] private GameObject _expandButton;

        public ConsoleMessage ConsoleMessage { private set; get; }
        public bool Opened { private set; get; }



        public void Open(ConsoleMessage consoleMessage)
        {
            Opened = true;
            ConsoleMessage = consoleMessage;

            Color bgColor = ConsoleSetting.Instance.GetBgColor(consoleMessage.Type);
            _bgImage.color = bgColor;
            Color color = ConsoleSetting.Instance.GetColor(consoleMessage.Type);
            _downlineImage.color = color;
            _logchainText.text = consoleMessage.Logchain;
            _logchainButton.SetActive(!string.IsNullOrWhiteSpace(consoleMessage.Logchain));
            _titleText.text = consoleMessage.Title;
            _stacktraceText.text = ConsoleAlgorithms.GetColoredStacktrace(consoleMessage.StackTraceTruncated);
            _expandButton.SetActive(consoleMessage.StackTraceExpandable);
            _openLogchainBgImage.color = bgColor;
            _openLogchainHighlightImage.color = color;

            RefreshVisuals();

            gameObject.SetActive(true);

            if (ConsoleConfigurator.Instance.AnimationToggle.Selected)
            {
                _animation.Stop();
                _animation.Play("MessageInspector_open");
            }
        }

        public void Close()
        {
            Opened = false;
            RefreshVisuals();
            _stacktraceText.text = "";

            if (ConsoleConfigurator.Instance.AnimationToggle.Selected)
            {
                _animation.Stop();
                _animation.Play("MessageInspector_close");
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void CopyToClipboard()
        {
            _copyButtonAnimation.Stop();
            _copyButtonAnimation.Play();

            string buffer = "";
            buffer += ConsoleMessage.Title;
            if (!string.IsNullOrWhiteSpace(ConsoleMessage.Logchain))
            {
                buffer += "\n#logchain: '" + ConsoleMessage.Logchain + "'";
            }
            buffer += "\n\n";
            buffer += ConsoleMessage.StackTrace;

            GUIUtility.systemCopyBuffer = buffer;
        }

        public void Expand()
        {
            _stacktraceText.text = ConsoleAlgorithms.GetColoredStacktrace(ConsoleMessage.StackTrace);
            _expandButton.SetActive(false);
        }

        public void OpenLogchain()
        {
            Console.Instance.ConsoleConfigurator.LogchainToggle.Selected = true;
            Console.Instance.LogchainWindow.Input_Search.text = ConsoleMessage.Logchain;
        }

        private void OnEnable()
        {
            RefreshVisuals();
        }

        private void RefreshVisuals()
        {
            _closeBtnCanvasGroup.alpha = 1;
            transform.localPosition = Vector3.zero;
            _canvasGroup.alpha = Opened ? 1 : 0;
            _canvasGroup.interactable = Opened;
            _canvasGroup.blocksRaycasts = Opened;
        }
    }
}