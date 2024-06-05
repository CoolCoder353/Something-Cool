using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TIM;
using TMPro;
using UnityEngine;

namespace Demo
{
    public class DemoManager : MonoBehaviour
    {
        [BoxGroup("0")] public CmdFormula SpawnPlayerFormula;
        [BoxGroup("1")] public CmdFormula ChangePlayerColorFormula;
        [BoxGroup("2")] public CmdFormula SpawnBallsFormula;
        [BoxGroup("3")] public CmdFormula SetBallsDebugFormula;

        [Header("Links")]
        public TMP_Text CommandsListText;
        public DemoPlayer PlayerPrefab;
        public GameObject BallPrefab;

        private DemoPlayer _demoPlayerInstance;

        private void Start()
        {
            // you can get formula's preview like this:
            CommandsListText.text += "\n" + SpawnPlayerFormula.GetPreview(true);
            CommandsListText.text += "\n" + ChangePlayerColorFormula.GetPreview(true);
            CommandsListText.text += "\n" + SpawnBallsFormula.GetPreview(true);
            CommandsListText.text += "\n" + SetBallsDebugFormula.GetPreview(true);

            // you can register your commands:
            Console.RegisterCommand(SpawnPlayerFormula, OnSpawnPlayerCommand);
            Console.RegisterCommand(ChangePlayerColorFormula, OnChangePlayerColorCommand);
            Console.RegisterCommand(SpawnBallsFormula, OnSpawnBallsFormula);
            Console.RegisterCommand(SetBallsDebugFormula, OnSetBallsDebug);

            // you can create new formula with code:
            CmdFormula formula = new CmdFormula()
            {
                Parts = new List<CmdFormulaPart>()
                {
                    new CmdFormulaPart("Download_image"),
                    new CmdFormulaPart(CmdPartType.String, "URL")
                },
                SpaceType = CmdSpaceType.Space
            };
            Console.RegisterCommand(formula, OnDownloadImageCommand);
        }

        private void OnSpawnPlayerCommand(CmdInputResult result)
        {
            if (_demoPlayerInstance)
                throw new UnityException("Player is already spawned!");

            _demoPlayerInstance = Instantiate(PlayerPrefab);
            print("Player spawned!");
        }

        private void OnChangePlayerColorCommand(CmdInputResult result)
        {
            if (!_demoPlayerInstance)
                throw new UnityException("Player is not spawned");

            _demoPlayerInstance.ChangeColor(result.Parts[1].EnumVariant);
        }

        private void OnSpawnBallsFormula(CmdInputResult result)
        {
            if (result.Parts[1].Integer > 50)
                throw new UnityException("Maximum 50 balls to spawn!");

            for (int i = 0; i < result.Parts[1].Integer; i++)
            {
                Instantiate(BallPrefab).transform.position += new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            }

            print("Balls sucessfully spawned!");
        }

        private void OnSetBallsDebug(CmdInputResult result)
        {
            Ball.EnableDebug = result.Parts[1].Bool;

            print("Balls debugging set to: " + Ball.EnableDebug);
        }

        private void OnDownloadImageCommand(CmdInputResult result)
        {
            StartCoroutine(DownloadImage(result.Parts[1].String));
        }

        // example from documentation:
        IEnumerator DownloadImage(string imageUrl)
        {
            yield return new WaitForSeconds(1);

            WWW www = new WWW(imageUrl);
            string logchain = "Image downloading";

            Console.Log("Web request sent", MessageType.Network, logchain, true);
            yield return www;

            if (www.error != null)
            {
                Console.Log(www.error, MessageType.Error, logchain); // we don't want to hide errors
            }
            else
            {
                Console.Log("data has been downloaded", MessageType.Network, logchain, true);
                Texture2D texture = www.texture;

                if (texture == null)
                {
                    Console.Log("Data doesn't contant a texture!", MessageType.Error,
                        logchain); // we don't want to hide errors
                }
                else
                {
                    Console.Log("Success!", MessageType.Default, logchain, true);
                }
            }
        }
    }
}