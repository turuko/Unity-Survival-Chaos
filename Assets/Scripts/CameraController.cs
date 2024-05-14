using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DefaultNamespace
{
    
    //TODO: Figure out how to clamp the camera view to a bounds instead of clamping its position
    //TODO: which leads to incorrect limits at different zoom levels
    
    public class CameraController : MonoBehaviour
    {
        public float panSpeed = 20f;
        public float panBorderThickness = 20f;
        public float scrollSpeed = 20f;
        

        private void Update()
        {
            var pos = transform.position;
            if (Input.GetKey(KeyCode.W) || Input.mousePosition.y > Screen.height - panBorderThickness)
            {
                pos.z += panSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.S) || Input.mousePosition.y < panBorderThickness)
            {
                pos.z -= panSpeed * Time.deltaTime;
            }
            
            if (Input.GetKey(KeyCode.D) || Input.mousePosition.x > Screen.width - panBorderThickness)
            {
                pos.x += panSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.A) || Input.mousePosition.x < panBorderThickness)
            {
                pos.x -= panSpeed * Time.deltaTime;
            }
            
            pos += transform.forward * Input.GetAxis("Mouse ScrollWheel") * scrollSpeed * 100f * Time.deltaTime;
            
            transform.position = pos;
        }
    }
}