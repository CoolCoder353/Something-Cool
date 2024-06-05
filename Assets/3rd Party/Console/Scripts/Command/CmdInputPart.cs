using System;
using NaughtyAttributes;

namespace TIM
{
    [System.Serializable]
    public class CmdInputPart
    {
        [Label("Type")] public CmdPartType Type;
        public string String;
        public int EnumVariant;
        public string EnumVariantString;
        public bool Bool;
        public int Integer;
        public float Float;
        public bool Parsed;
        public bool PotentiallyOk;

        public string AutoCompletedString;
        public string HelpString;

        object GetValue()
        {
            switch (Type)
            {
                case CmdPartType.String: return String;
                case CmdPartType.Enum: return EnumVariant;
                case CmdPartType.Bool: return Bool;
                case CmdPartType.Integer: return Integer;
                case CmdPartType.Float: return Float;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public CmdInputPart(string val, CmdFormulaPart formulaPart, bool autoComplete)
        {
            Type = formulaPart.Type;
            PotentiallyOk = false;
            Parsed = false;
            switch (Type)
            {
                case CmdPartType.Sentence:
                    AutoCompletedString = formulaPart.Sentence;
                    if (formulaPart.Sentence == val)
                    {
                        String = val;
                        PotentiallyOk = true;
                        Parsed = true;
                        break;
                    }
                    else if (formulaPart.Sentence.Length >= val.Length && formulaPart.Sentence.ToLower().Substring(0, val.Length) == val.ToLower())
                    {
                        String = formulaPart.Sentence;
                        PotentiallyOk = true;

                        if (autoComplete)
                        {
                            Parsed = true;
                            break;
                        }
                    }

                    Parsed = false;
                    break;

                case CmdPartType.String:
                    String = val;
                    AutoCompletedString = String;
                    PotentiallyOk = true;
                    Parsed = true;
                    break;
                case CmdPartType.Enum:
                    for (int i = 0; i < formulaPart.EnumVariants.Length; i++)
                    {
                        string enumVariant = formulaPart.EnumVariants[i];
                        if (val == enumVariant)
                        {
                            EnumVariant = i;
                            EnumVariantString = val;
                            AutoCompletedString = enumVariant;
                            Parsed = true;
                            break;
                        }
                        else if (!PotentiallyOk && enumVariant.Length >= val.Length && enumVariant.ToLower().Substring(0, val.Length) == val.ToLower())
                        {
                            EnumVariant = i;
                            EnumVariantString = enumVariant;
                            AutoCompletedString = enumVariant;
                            PotentiallyOk = true;
                            //break;
                        }
                    }

                    break;
                case CmdPartType.Bool:
                    if (val == "0")
                    {
                        Bool = false;
                        Parsed = true;
                        AutoCompletedString = "0";
                        break;
                    }
                    else if (val == "1")
                    {
                        Bool = true;
                        Parsed = true;
                        AutoCompletedString = "1";
                        break;
                    }
                    else if (val == "false")
                    {
                        Bool = false;
                        Parsed = true;
                        AutoCompletedString = "false";
                        break;
                    }
                    else if ("false".Contains(val))
                    {
                        Bool = false;
                        PotentiallyOk = true;
                        AutoCompletedString = "false";

                        if (autoComplete)
                        {
                            Parsed = true;
                            break;
                        }
                    }
                    else if (val == "true")
                    {
                        Bool = true;
                        Parsed = true;
                        AutoCompletedString = "true";
                        break;
                    }
                    else if ("true".Contains(val))
                    {
                        Bool = true;
                        PotentiallyOk = true;
                        AutoCompletedString = "true";

                        if (autoComplete)
                        {
                            Parsed = true;
                            break;
                        }
                    }

                    Parsed = false;
                    break;
                case CmdPartType.Integer:
                    AutoCompletedString = val;
                    if (int.TryParse(val, out Integer))
                    {
                        Parsed = true;
                        break;
                    }
                    else
                    {
                        Parsed = false;
                        break;
                    }
                case CmdPartType.Float:
                    AutoCompletedString = val;
                    if (float.TryParse(val, out Float))
                    {
                        Parsed = true;
                        break;
                    }
                    else
                    {
                        Parsed = false;
                        break;
                    }
                default:
                    Parsed = false;
                    throw new ArgumentOutOfRangeException();
            }
            HelpString = ConsoleAlgorithms.GetAutoCompleteHelpString(val, AutoCompletedString);
        }
    }
}