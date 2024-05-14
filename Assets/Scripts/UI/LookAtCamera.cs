using System;
using UnityEngine;

namespace DefaultNamespace.UI
{
    public class LookAtCamera : MonoBehaviour
    {
        [SerializeField] private float constantScreenSize = 100f; // Set the desired constant size
        private Canvas canvas;
        private Camera mainCamera;
        
        private void Start()
        {
            canvas = GetComponent<Canvas>();
            mainCamera = Camera.main;

            // Make sure the Canvas is set to World Space
            if (canvas.renderMode != RenderMode.WorldSpace)
            {
                Debug.LogWarning("The Canvas render mode should be set to World Space for constant size functionality.");
            }
        }
        
        private void Update()
        {
            // Calculate the desired scale based on the constant screen size
            float distance = Vector3.Distance(transform.position, mainCamera.transform.position);
            float scaleFactor = Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad) * distance * 2 / constantScreenSize;

            // Set the scale of the Canvas
            canvas.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);

            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                Camera.main.transform.rotation * Vector3.up);
        }
    }
}