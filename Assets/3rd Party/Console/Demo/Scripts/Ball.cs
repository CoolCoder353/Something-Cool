using TIM;
using UnityEngine;

namespace Demo
{
    public class Ball : MonoBehaviour
    {
        public static bool EnableDebug;

        private static int _ballsSpawned;

        private string _logchain;

        private void Awake()
        {
            _logchain = "Ball " + _ballsSpawned++;
            if (EnableDebug)
                Console.Log("spawned", MessageType.Default, _logchain, true);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (EnableDebug)
                Console.Log("collision", MessageType.Default, _logchain, true);
        }

        private void Update()
        {
            if (transform.position.y < -30)
            {
                Destroy(gameObject);

                if (EnableDebug)
                    Console.Log("destroyed", MessageType.Default, _logchain, true);
            }
        }
    }
}