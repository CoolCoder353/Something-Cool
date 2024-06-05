using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TIM
{
#if UNITY_EDITOR
    [Icon("Assets/TIM/Console/Sprites/Console icon mini.png")]
#endif
    public class ConsoleMessageElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _sideImage;
        [SerializeField] private Image _bgImage;
        [SerializeField] private Image _logchainImage;
        [SerializeField] private TMP_Text _logchainText;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _stacktracePreviewText;
        [SerializeField] private TMP_Text _countText;
        [SerializeField] private Image _glareImage;
        [SerializeField] private VerticalLayoutGroup _verticalLayoutGroup;
        [SerializeField] private LayoutElement _layoutElement;
        public ConsoleMessage ConsoleMessage { private set; get; }
        private Coroutine _highlightCoroutine;
        private float _sideWidth;
        private bool _inited;
        
        private Color titleColor;
        private Color logchainColor;
        private Color stacktraceColor;
        
        public void Init(ConsoleMessage consoleMessage, bool animate)
        {
            ConsoleMessage = consoleMessage;
            Color c = ConsoleSetting.Instance.GetColor(consoleMessage.Type);
            _iconImage.sprite = ConsoleSetting.Instance.GetIcon(consoleMessage.Type);
            _iconImage.color = c;
                _sideImage.color = c;
            _logchainImage.color = c;
            _sideWidth = _sideImage.rectTransform.sizeDelta.x;

            c.a = 150 / 255f;
            _glareImage.color = c;
            c.a = 5 / 255f;
            _bgImage.color = c;
            _logchainText.gameObject.SetActive(!string.IsNullOrWhiteSpace(consoleMessage.Logchain));
            _logchainText.text = consoleMessage.Logchain;
            _titleText.text = consoleMessage.TitleCompact;
            _stacktracePreviewText.text = consoleMessage.StackTracePreview;
            
            titleColor = _titleText.color;
            logchainColor = _logchainText.color;
            stacktraceColor = _stacktracePreviewText.color;

            RefreshCount();
            RefreshCategoryActivity();

            _inited = true;
            
            if (animate && gameObject.activeInHierarchy && ConsoleConfigurator.Instance.AnimationToggle.Selected)
            {
                StartCoroutine(AnimateAsync());
            }
        }

        public void RefreshCount()
        {
            if (ConsoleConfigurator.Instance.CollapseToggle.Selected && ConsoleMessage.CopiesCount > 0)
            {
                _countText.gameObject.SetActive(true);
                _countText.text = ConsoleAlgorithms.GetCountString(ConsoleMessage.CopiesCount + 1);
            }
            else
            {
                _countText.gameObject.SetActive(false);
            }
        }

        public void RefreshCategoryActivity()
        {
            gameObject.SetActive(ConsoleConfigurator.Instance.IsCategoryActive(ConsoleMessage.Type));
        }

        private IEnumerator AnimateAsync()
        {
            var rectTransform = (RectTransform) transform;

            Color clearColor = Color.clear;

            _titleText.color = clearColor;
            _logchainText.color = clearColor;
            _stacktracePreviewText.color = clearColor;

            // Vertical Layout group
            _verticalLayoutGroup.enabled = false;
            float targetHeight = _logchainText.gameObject.activeSelf ? 100 : 70;
            var sizeDelta = rectTransform.sizeDelta;
            _layoutElement.enabled = true;

            float dur = 0.2f;
            for (float t = 0; t < dur; t += Time.unscaledDeltaTime)
            {
                sizeDelta.y = t / dur * targetHeight;
                rectTransform.sizeDelta = sizeDelta;
                _layoutElement.preferredHeight = sizeDelta.y;
                yield return null;
            }
            
            _layoutElement.enabled = false;
            sizeDelta.y = targetHeight;
            rectTransform.sizeDelta = sizeDelta;
            _verticalLayoutGroup.enabled = true;

            _glareImage.gameObject.SetActive(true);
            Vector2 finalPos = new Vector2(rectTransform.rect.width, 0);
            var glareTransform = _glareImage.rectTransform;
            glareTransform.anchorMin = Vector2.zero;
            glareTransform.anchorMax = Vector2.one;
            glareTransform.anchoredPosition = Vector2.zero;
            glareTransform.sizeDelta = Vector2.zero;
            glareTransform.anchorMin = Vector2.one / 2;
            glareTransform.anchorMax = Vector2.one / 2;
            glareTransform.sizeDelta = rectTransform.rect.size;
            glareTransform.anchoredPosition = -finalPos;
            float duration = 0.3f;
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                glareTransform.anchoredPosition = (t / duration * 2 - 1) * finalPos;
                _titleText.color = Color.Lerp(clearColor, titleColor, t/duration);
                _logchainText.color = Color.Lerp(clearColor, logchainColor, t/duration);
                _stacktracePreviewText.color = Color.Lerp(clearColor, stacktraceColor, t/duration);
                yield return new WaitForEndOfFrame();
            }

            _titleText.color = titleColor;
            _logchainText.color = logchainColor;
            _stacktracePreviewText.color = stacktraceColor;
            glareTransform.anchoredPosition = finalPos;
            _glareImage.gameObject.SetActive(false);
        }

        private IEnumerator AnimateHighlightAsync(bool highlight)
        {
            Color startColor = _bgImage.color;
            Color targetColor = ConsoleSetting.Instance.GetBgColor(ConsoleMessage.Type, highlight);
            var sizeDelta = _sideImage.rectTransform.sizeDelta;
            float sideStartWidth = sizeDelta.x;
            float sideTargetWidth = highlight ? 8 : 4;

            float dur = 0.2f;
            for (float t = 0; t < dur; t+= Time.unscaledDeltaTime)
            {
                _bgImage.color = Color.Lerp(startColor, targetColor, t / dur);
                sizeDelta.x = Mathf.Lerp(sideStartWidth, sideTargetWidth, t / dur);
                _sideImage.rectTransform.sizeDelta = sizeDelta;
                yield return null;
            }

            sizeDelta.x = sideTargetWidth;
            _sideImage.rectTransform.sizeDelta = sizeDelta;
            _bgImage.color = targetColor;
        }

        private void SetHighlightVisuals(bool highlight)
        {
            var sizeDelta = _sideImage.rectTransform.sizeDelta;
            sizeDelta.x = highlight ? _sideWidth * 2 : _sideWidth;
            _sideImage.rectTransform.sizeDelta = sizeDelta;
            Color targetColor = ConsoleSetting.Instance.GetBgColor(ConsoleMessage.Type, highlight);
            _bgImage.color = targetColor;
        }

        private void OnEnable()
        {
            if(!_inited)
                return;
            
            SetHighlightVisuals(false);
            _glareImage.gameObject.SetActive(false);
            _titleText.color = titleColor;
            _logchainText.color = logchainColor;
            _stacktracePreviewText.color = stacktraceColor;
            _verticalLayoutGroup.enabled = true;
            _layoutElement.enabled = false;
        }

        #region Overrides

        public void OnPointerEnter(PointerEventData eventData)
        {
            ConsoleAudio.PlayHighlightSound();
            if (_highlightCoroutine != null)
            {
                StopCoroutine(_highlightCoroutine);
                _highlightCoroutine = null;
            }
            if (ConsoleConfigurator.Instance.AnimationToggle.Selected)
            {
                _highlightCoroutine = StartCoroutine(AnimateHighlightAsync(true));
            }
            else
            {
                SetHighlightVisuals(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_highlightCoroutine != null)
            {
                StopCoroutine(_highlightCoroutine);
                _highlightCoroutine = null;
            }
            if (ConsoleConfigurator.Instance.AnimationToggle.Selected)
            {
                _highlightCoroutine = StartCoroutine(AnimateHighlightAsync(false));
            }
            else
            {
                SetHighlightVisuals(false);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            ConsoleAudio.PlayClickSound();
            if (_highlightCoroutine != null)
            {
                StopCoroutine(_highlightCoroutine);
                _highlightCoroutine = null;
            }
            SetHighlightVisuals(false);
            Console.Instance.OpenMessage(ConsoleMessage);
        }
        
        #endregion
    }
}