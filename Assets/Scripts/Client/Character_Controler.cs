using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
namespace Character
{


    public class Character_Controler : MonoBehaviour
    {

        public Character_Settings settings;
        public Camera playerCamera;


        private Vector3 movement;
        private float scroll;


        [ClientCallback]
        // Update is called once per frame
        void Update()
        {

            float CameraSize = gameObject.transform.position.z;
            //    SHIFT     \\
            float shiftMultiply = 1;
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                shiftMultiply = settings.shiftSpeedMultiplyer;
            }
            //     SCROLL     \\
            scroll = settings.scrollSpeed * Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime;
            float percentageScroll = Mathf.Clamp(CameraSize / settings.zoomScale.y, settings.zoomSpeedScale.x, settings.zoomSpeedScale.y);

            //     MOVEMENT    \\
            movement.x = Input.GetAxis("Horizontal") * settings.speed * shiftMultiply * percentageScroll * Time.deltaTime;
            movement.y = Input.GetAxis("Vertical") * settings.speed * shiftMultiply * percentageScroll * Time.deltaTime;


            //     DEBUG     \\
            if (settings.debug)
            {
                Debug.Log("Movement: " + movement);
                Debug.Log("Scroll: " + scroll);
                Debug.Log("% Scroll: " + percentageScroll);
            }

        }

        [ClientCallback]
        private void LateUpdate()
        {
            float CameraSize = gameObject.transform.position.z;

            gameObject.transform.position += movement;
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, Mathf.Clamp(CameraSize - scroll, settings.zoomScale.x, settings.zoomScale.y));

        }
    }
}