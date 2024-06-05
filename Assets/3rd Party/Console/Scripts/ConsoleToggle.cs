using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using NaughtyAttributes;

namespace TIM
{

    public class ConsoleToggle : ConsoleButton
    {

        public bool Selected
        {
            get => _selected;
            set
            {
                bool changed = _selected != value;
                _selected = value;
                if (_iconImage)
                    _iconImage.color = _selected ? _iconColor : Color.black;
                if (_selectedPanel)
                    _selectedPanel.SetActive(_selected);

                if (changed)
                {
                    OnToggleEvent?.Invoke(_selected);
                }
            }
        }

        [SerializeField, HideInInspector] private bool _selected = true;

        [Foldout("Links"), SerializeField, OnValueChanged(nameof(GetImageColor))] private Image _iconImage;
        [Foldout("Links"), SerializeField] private Color _iconColor;
        [Foldout("Links"), SerializeField] private GameObject _selectedPanel;

        [Foldout("Events")] public UnityEvent<bool> OnToggleEvent = new UnityEvent<bool>();

        protected override void OnClick()
        {
            Selected = !Selected;
        }

        private void GetImageColor()
        {
            if (_iconImage)
                _iconColor = _iconImage.color;
        }
    }
}
