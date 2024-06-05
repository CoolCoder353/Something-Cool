
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TIM
{

    public class ConsoleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {

        public bool Activity
        {
            get => _activity;
            set
            {
                _activity = value;
                if (DisabledPanel)
                    DisabledPanel.SetActive(!Activity);

                SetHighlight(_mouseEntered);
                SetPressed(false);
            }
        }

        [BoxGroup("Params")] public bool EnableAudio = true;
        [BoxGroup("Params")] public bool DisableHighlightOnStaticMouse = false;

        [BoxGroup("Links")] public GameObject HighlightPanel;
        [BoxGroup("Links")] public GameObject PressedPanel;
        [BoxGroup("Links")] public GameObject DisabledPanel;

        [BoxGroup("Events")] public UnityEvent ClickEvent = new UnityEvent();

        [SerializeField, HideInInspector] private bool _activity = true;

        public bool Highlighted { private set; get; }
        public bool Pressed { private set; get; }

        private bool _mouseEntered;

        #region Overrides

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (DisableHighlightOnStaticMouse && eventData.delta == Vector2.zero)
                return;

            _mouseEntered = true;
            SetHighlight(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _mouseEntered = false;
            SetHighlight(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left || !_mouseEntered)
                return;

            SetPressed(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            SetPressed(false);
        }

        private void OnEnable()
        {
            Highlighted = false;
            Pressed = false;
            if (HighlightPanel)
                HighlightPanel.SetActive(false);
            if (PressedPanel)
                PressedPanel.SetActive(false);
            if (DisabledPanel)
                DisabledPanel.SetActive(!Activity);
        }

        #endregion

        #region Public Methods

        public void SetHighlight(bool highlighted)
        {
            if (!Activity && highlighted)
                return;

            if (Highlighted != highlighted)
            {
                Highlighted = highlighted;

                if (EnableAudio && highlighted)
                    ConsoleAudio.PlayHighlightSound();

                OnHighlight(highlighted);
            }

            if (HighlightPanel)
                HighlightPanel.SetActive(highlighted);
        }

        public void SetPressed(bool pressed)
        {
            if (!Activity && pressed)
                return;

            if (Pressed != pressed)
            {
                Pressed = pressed;

                if (pressed)
                {
                    if (EnableAudio)
                        ConsoleAudio.PlayClickSound();
                    ClickEvent?.Invoke();
                    OnClick();
                }
            }

            if (PressedPanel)
                PressedPanel.SetActive(pressed);
        }

        #endregion

        protected virtual void OnHighlight(bool highlight) { }
        protected virtual void OnClick() { }
    }
}
