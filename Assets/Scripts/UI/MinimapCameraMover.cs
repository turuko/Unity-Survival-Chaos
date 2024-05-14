using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DefaultNamespace.UI
{
    public class MinimapCameraMover : MonoBehaviour, IPointerClickHandler
    {
        public Camera mainCamera;
        public Camera minimapCamera;
        public RectTransform minimapRect;
        public Collider mapCollider;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(minimapRect, eventData.position,
                        eventData.pressEventCamera,
                        out var localPoint))
                {
                    Vector2 normalisedPoint = localPoint / minimapRect.rect.size;
                    var ray = minimapCamera.ViewportPointToRay(normalisedPoint);

                    if (mapCollider.Raycast(ray, out var hit, Mathf.Infinity))
                    {
                        float distance = 0;
                        var mainCameraRay = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
                        if (mapCollider.Raycast(mainCameraRay, out var mainCameraHit, Mathf.Infinity))
                        {
                            var distVector = mainCamera.transform.position - mainCameraHit.point;
                            distance = distVector.magnitude;
                        }

                        mainCamera.transform.position = hit.point + (-mainCamera.transform.forward * distance);
                    }
                    
                }
            }
        }
    }
}