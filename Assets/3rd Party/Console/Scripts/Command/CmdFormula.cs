using System;
using System.Collections.Generic;
using NaughtyAttributes;

namespace TIM
{
    [System.Serializable]
    public class CmdFormula
    {

        public string Preview
        {
            get => GetPreview(false);
        }
        [BoxGroup, Label("Space Type")]
        public CmdSpaceType SpaceType;
        public List<CmdFormulaPart> Parts = new List<CmdFormulaPart>() { new CmdFormulaPart("Command"), new CmdFormulaPart(CmdPartType.Bool, "state") };

        public string GetSpaceString()
        {
            switch (SpaceType)
            {
                case CmdSpaceType.Space: return " ";
                case CmdSpaceType.Underline: return "_";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // NaughtyAttributes doesn't have an equivalent for OnInspectorInit
        // Consider moving this logic to a suitable place like Awake or Start if this is a MonoBehaviour
        private void Awake()
        {
            foreach (CmdFormulaPart part in Parts)
            {
                part.Formula = this;
            }
        }

        /// <summary>
        /// Get preview string of formula
        /// </summary>
        /// <param name="colored">If you need to have colored symbols with Rich text. You can set it to true.</param>
        /// <returns></returns>
        public string GetPreview(bool colored)
        {
            if (Parts == null || Parts.Count == 0)
                return "";

            string result = "";
            for (var i = 0; i < Parts.Count; i++)
            {
                result += Parts[i].GetPreviewString(colored, false);
                if (i < Parts.Count - 1)
                    result += GetSpaceString();
            }

            return result;
        }
    }
}