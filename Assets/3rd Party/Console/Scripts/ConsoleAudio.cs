using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TIM
{
    public class ConsoleAudio : MonoBehaviour
    {
        [SerializeField, Range(0, 1)] private float highlightVolume = 1;
        [SerializeField] private AudioClip highlightClip;
        [SerializeField, Range(0, 1)] private float clickVolume = 1;
        [SerializeField] private AudioClip clickClip;
        [SerializeField, Range(0, 1)] private float commandExecuteVolume = 1;
        [SerializeField] private AudioClip commandExecuteClip;
        [SerializeField, Range(0, 1)] private float commandFailVolume = 1;
        [SerializeField] private AudioClip commandFailClip;
        [SerializeField] private AudioSource audioSource;

        public static ConsoleAudio Instance { private set; get; }
        public static bool AudioEnabled => ConsoleSetting.Instance.AudioEnabled;

        private void Awake()
        {
            Instance = this;
        }

        public static void PlayHighlightSound()
        {
            if (Instance && AudioEnabled)
            {
                Instance.audioSource.pitch = Random.Range(0.7f, 1.3f);
                Instance.audioSource.PlayOneShot(Instance.highlightClip, Instance.highlightVolume);
            }
        }

        public static void PlayClickSound()
        {
            if (Instance && AudioEnabled)
            {
                Instance.audioSource.pitch = Random.Range(0.7f, 1.3f);
                Instance.audioSource.PlayOneShot(Instance.clickClip, Instance.clickVolume);
            }
        }

        public static void PlayCommandExecuteSound()
        {
            if (Instance && AudioEnabled)
            {
                Instance.audioSource.pitch = Random.Range(0.7f, 1.3f);
                Instance.audioSource.PlayOneShot(Instance.commandExecuteClip, Instance.commandExecuteVolume);
            }
        }

        public static void PlayCommandFailSound()
        {
            if (Instance && AudioEnabled)
            {
                Instance.audioSource.pitch = Random.Range(0.7f, 1.3f);
                Instance.audioSource.PlayOneShot(Instance.commandFailClip, Instance.commandFailVolume);
            }
        }
    }
}