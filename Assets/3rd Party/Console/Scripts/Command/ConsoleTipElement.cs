
using TMPro;
using UnityEngine;
using NaughtyAttributes;
namespace TIM
{

    public class ConsoleTipElement : ConsoleButton
    {
        [BoxGroup("BoxGroup"), SerializeField] private TMP_Text _text;

        public CmdInputResult CmdInputResult { private set; get; }
        public int Index { private set; get; }
        public ConsoleTipMaster TipMaster { private set; get; }

        public void Init(CmdInputResult cmdInputResult, int index, ConsoleTipMaster tipMaster)
        {
            CmdInputResult = cmdInputResult;
            Index = index;
            TipMaster = tipMaster;
            _text.text = cmdInputResult.GetColoredTipText();
        }

        protected override void OnHighlight(bool highlight)
        {
            if (highlight)
                TipMaster.HighlightElement(Index);
        }

        protected override void OnClick()
        {
            Console.Instance.InputPanel.OnEnterPressed();
        }
    }
}