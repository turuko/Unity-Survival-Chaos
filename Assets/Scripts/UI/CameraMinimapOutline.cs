using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.UI
{
    public class CameraMinimapOutline : MonoBehaviour
    {

        public Material cameraBoxMaterial;

        public Camera minimap;

        public float lineWidth;

        public Collider mapCollider;

        private Vector3 GetCameraFrustumPoint(Vector3 position)
        {
            var positionRay = Camera.main.ScreenPointToRay(position);
            RaycastHit hit;
            Vector3 result = mapCollider.Raycast(positionRay, out hit, Camera.main.transform.position.y * 2)
                ? hit.point
                : new Vector3();

            return result;

        }
        
        private Vector3 ClampViewportPoint(Vector3 viewportPoint)
        {
            float minX = 0 + lineWidth;
            float maxX = 1 - lineWidth;
            float minY = 0 + lineWidth;
            float maxY = 1 - lineWidth;

            float clampedX = Mathf.Clamp(viewportPoint.x, minX, maxX);
            float clampedY = Mathf.Clamp(viewportPoint.y, minY, maxY);

            return new Vector3(clampedX, clampedY, viewportPoint.z);
        }

        public void OnPostRender()
        {
            Vector3 minMinViewportPoint = minimap.WorldToViewportPoint(GetCameraFrustumPoint(new Vector3(0f, 0f)));
            Vector3 maxMinViewportPoint =
                minimap.WorldToViewportPoint(GetCameraFrustumPoint(new Vector3(Screen.width, 0f)));
            Vector3 minMaxViewportPoint =
                minimap.WorldToViewportPoint(GetCameraFrustumPoint(new Vector3(0f, Screen.height)));
            Vector3 maxMaxViewportPoint =
                minimap.WorldToViewportPoint(GetCameraFrustumPoint(new Vector3(Screen.width, Screen.height)));

            minMinViewportPoint = ClampViewportPoint(minMinViewportPoint);
            maxMinViewportPoint = ClampViewportPoint(maxMinViewportPoint);
            minMaxViewportPoint = ClampViewportPoint(minMaxViewportPoint);
            maxMaxViewportPoint = ClampViewportPoint(maxMaxViewportPoint);
            
            GL.PushMatrix();
            {
                cameraBoxMaterial.SetPass(0);
                GL.LoadOrtho();

                GL.Begin(GL.QUADS);
                GL.Color(Color.white);
                {

                    GL.Vertex(new Vector3(minMinViewportPoint.x, minMinViewportPoint.y + lineWidth, 0));
                    GL.Vertex(new Vector3(minMinViewportPoint.x, minMinViewportPoint.y - lineWidth, 0));
                    GL.Vertex(new Vector3(maxMinViewportPoint.x, maxMinViewportPoint.y - lineWidth, 0));
                    GL.Vertex(new Vector3(maxMinViewportPoint.x, maxMinViewportPoint.y + lineWidth, 0));


                    GL.Vertex(new Vector3(minMinViewportPoint.x + lineWidth, minMinViewportPoint.y, 0));
                    GL.Vertex(new Vector3(minMinViewportPoint.x - lineWidth, minMinViewportPoint.y, 0));
                    GL.Vertex(new Vector3(minMaxViewportPoint.x - lineWidth, minMaxViewportPoint.y, 0));
                    GL.Vertex(new Vector3(minMaxViewportPoint.x + lineWidth, minMaxViewportPoint.y, 0));



                    GL.Vertex(new Vector3(minMaxViewportPoint.x, minMaxViewportPoint.y + lineWidth, 0));
                    GL.Vertex(new Vector3(minMaxViewportPoint.x, minMaxViewportPoint.y - lineWidth, 0));
                    GL.Vertex(new Vector3(maxMaxViewportPoint.x, maxMaxViewportPoint.y - lineWidth, 0));
                    GL.Vertex(new Vector3(maxMaxViewportPoint.x, maxMaxViewportPoint.y + lineWidth, 0));

                    GL.Vertex(new Vector3(maxMinViewportPoint.x + lineWidth, maxMinViewportPoint.y, 0));
                    GL.Vertex(new Vector3(maxMinViewportPoint.x - lineWidth, maxMinViewportPoint.y, 0));
                    GL.Vertex(new Vector3(maxMaxViewportPoint.x - lineWidth, maxMaxViewportPoint.y, 0));
                    GL.Vertex(new Vector3(maxMaxViewportPoint.x + lineWidth, maxMaxViewportPoint.y, 0));

                }
                GL.End();
            }
            GL.PopMatrix();
        }

    }
}