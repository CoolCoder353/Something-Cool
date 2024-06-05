
using TMPro;
using UnityEngine;

namespace TIM
{

    public class CommandsListElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        public CmdFormula Formula { private set; get; }

        public void Init(CmdFormula formula)
        {
            Formula = formula;
            RefreshVisuals();
        }

        public void RefreshVisuals()
        {
            _text.text = Formula.GetPreview(true);
        }
    }
}