using System;
using NaughtyAttributes;
using UnityEngine;

namespace TIM
{
    [System.Serializable]
    public class CmdFormulaPart
    {
        [BoxGroup("h"), Label("Type")]
        public CmdPartType Type;

        [BoxGroup("h"), Tooltip("ex: 'Aimbot' or 'InfiniteHealth'"), ShowIf("Type", CmdPartType.Sentence), Label("Sentence")]
        public string Sentence = "";

        [BoxGroup("h"), Tooltip("ex: 'Color' or 'Weapon'"), HideIf("Type", CmdPartType.Sentence), Label("Title")]
        public string Title = "";

        [Tooltip("ex: 'Red', 'Green', 'Blue' or 'AK-47', 'Deagle', 'AWP'"), ShowIf("Type", CmdPartType.Enum)]
        public string[] EnumVariants = { "Variant-1", "Variant-2" };

        [HideInInspector] public CmdFormula Formula;

        public string GetPreviewString(bool coloring, bool underline)
        {
            string s = GetString();
            if (underline)
                s = "<u>" + s + "</u>";

            return s;

            // functions:
            string GetString()
            {
                switch (Type)
                {
                    case CmdPartType.Sentence: return Sentence ?? "";
                    case CmdPartType.String: return $"[{GetColored("string")}: " + (Title ?? "") + "]";
                    case CmdPartType.Enum: return $"[{GetColored("enum")}: " + (Title ?? "") + "]";
                    case CmdPartType.Bool: return $"[{GetColored("bool")}: " + (Title ?? "") + " ]";
                    case CmdPartType.Integer: return $"[{GetColored("int")}: " + (Title ?? "") + "]";
                    case CmdPartType.Float: return $"[{GetColored("float")}: " + (Title ?? "") + "]";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            string GetColored(string str, int variation = 0)
            {
                if (!coloring)
                    return str;

                if (variation == 0) // type name
                    return "<color=#0aebf5>" + str + "</color>";

                return str;
            }
        }

        private bool ContainsSpace()
        {
            if (Formula == null)
                return false;

            if (Type == CmdPartType.Sentence)
            {
                return Sentence.Contains(Formula.GetSpaceString());
            }
            else if (Type == CmdPartType.Enum)
            {
                if (EnumVariants == null)
                    return false;

                foreach (string enumVariant in EnumVariants)
                {
                    if (enumVariant.Contains(Formula.GetSpaceString()))
                        return true;
                }
            }

            return false;
        }

        // NaughtyAttributes doesn't have an equivalent for InfoBox
        // Consider using Debug.Log or similar to display these messages
        // private string GetSentenceSpaceContainsError => Formula == null ? "" : "Sentence can not contain space symbol '" + Formula.GetSpaceString() + "' from Formula!";
        // private string GetEnumSpaceContainsError => Formula == null ? "" : "Enum elements can not contain space symbol '" + Formula.GetSpaceString() + "' from Formula!";

        public CmdFormulaPart() { }

        public CmdFormulaPart(string sentence)
        {
            Sentence = sentence;
        }

        public CmdFormulaPart(string title, string[] enumVariants)
        {
            Title = title;
            EnumVariants = enumVariants;
        }

        public CmdFormulaPart(CmdPartType partType, string title)
        {
            Type = partType;
            Title = title;
        }
    }
}