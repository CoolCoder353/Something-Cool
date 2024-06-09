using UnityEngine;
using NaughtyAttributes;

namespace Character
{
    [CreateAssetMenu]
    public class Character_Settings : ScriptableObject
    {
        [Foldout("Main")]
        public float speed;
        [Foldout("Main")]
        public float scrollSpeed;

        [Foldout("Movement")]
        public float shiftSpeedMultiplyer;

        [Foldout("Movement")]
        public Vector2 zoomScale;

        [Foldout("Movement"), Tooltip("The min and max values that the scroll amount should affect the speed of the camera")]
        public Vector2 zoomSpeedScale;

        [Foldout("Debug")]
        public bool debug;

    }


}