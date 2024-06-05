using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace TIM
{
    [System.Serializable]
    public class CmdInputResult
    {
        [BoxGroup("BoxGroup")] public string InputString;
        public readonly List<CmdInputPart> Parts = new List<CmdInputPart>();
        [BoxGroup("formula")] public CmdFormula Formula;
        public bool Parsed = false;
        public bool PotentiallySuitable = false;
        public string AutoCompletedString;
        public string HelpString;



        public bool TryInit(string inputString, CmdFormula formula, bool autoComplete)
        {
            InputString = inputString;
            Formula = formula;
            PotentiallySuitable = false;
            AutoCompletedString = "";

            if (string.IsNullOrWhiteSpace(InputString) || Formula == null)
                return false;

            Parts.Clear();
            string space = Formula.GetSpaceString();
            int partIndex = 0;
            string partText = "";
            Parsed = true;
            for (int i = 0; i < inputString.Length; i++)
            {
                if (partIndex >= formula.Parts.Count)
                    return false;

                string c = inputString[i].ToString();
                if (c == space || i == inputString.Length - 1)
                {
                    if (c != space && i == inputString.Length - 1)
                        partText += c;
                    CmdInputPart inputPart = new CmdInputPart(partText, Formula.Parts[partIndex], autoComplete);

                    if (!inputPart.Parsed)
                    {
                        Parsed = false;
                        bool lastPartParsed = Parts.Count == 0 || Parts[^1].Parsed;
                        PotentiallySuitable = inputPart.PotentiallyOk && lastPartParsed;
                        if (!PotentiallySuitable)
                            return false;
                    }

                    Parts.Add(inputPart);
                    AutoCompletedString += inputPart.AutoCompletedString;
                    HelpString += inputPart.HelpString;
                    if (i != inputString.Length - 1)
                    {
                        AutoCompletedString += space;
                        HelpString += space;
                    }


                    partIndex++;
                    partText = "";
                }
                else
                {
                    partText += c;
                }
            }

            if (Parsed)
            {
                PotentiallySuitable = Parts.Count <= formula.Parts.Count;

                return Parts.Count == formula.Parts.Count;
            }
            else
                return false;
        }

        public void Execute()
        {
            if (!Parsed)
                throw new UnityException("Cmd is not parsed. How do you want to execute me, motherfucker?!");

            Console.RegisteredCommands[Formula]?.Invoke(this);
        }

        public string GetColoredTipText()
        {
            int currentPart = Parts.Count - 1;
            if (currentPart != Formula.Parts.Count - 1 && InputString[^1].ToString() == Formula.GetSpaceString())
                currentPart++;

            string result = "";
            for (int i = 0; i < Formula.Parts.Count; i++)
            {
                result += Formula.Parts[i].GetPreviewString(true, i == currentPart);
                if (i < Formula.Parts.Count - 1)
                {
                    result += Formula.GetSpaceString();
                }
            }

            return result;
        }
    }
}