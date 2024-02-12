using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveGames
{
    public class Viewpoint : MonoBehaviour
    {
        [Header("Viewpoint")]
        [SerializeField] string PointText = "Press E";
        [Space, SerializeField] Camera cam;
        [SerializeField] GameObject PlayerController;
        [SerializeField] Image ImagePrefab;
        [Space, SerializeField, Range(0.1f, 20)] float MaxViewRange = 8;
        [SerializeField, Range(0.1f, 20)] float MaxTextViewRange = 3;

        float Distance;
        Text ImageText;
        Image ImageUI;

        void Start()
        {
            if (cam == null)
            {
                Debug.LogError("Camera reference is not set in Viewpoint script!");
                return;
            }

            ImageUI = Instantiate(ImagePrefab, FindObjectOfType<Canvas>().transform).GetComponent<Image>();
            ImageText = ImageUI.GetComponentInChildren<Text>();
            ImageText.text = PointText;
        }

        void Update()
        {
            if (PlayerController == null)
            {
                Debug.LogError("PlayerController reference is not set in Viewpoint script!");
                return;
            }

            ImageUI.transform.position = cam.WorldToScreenPoint(calculateWorldPosition(transform.position, cam));

            Distance = Vector3.Distance(PlayerController.transform.position, transform.position);

            UpdateTextOpacity();
            UpdateImageOpacity();
        }

        private void UpdateTextOpacity()
        {
            float targetTextOpacity = Distance < MaxTextViewRange ? 1f : 0f;
            Color textOpacityColor = ImageText.color;
            textOpacityColor.a = Mathf.Lerp(textOpacityColor.a, targetTextOpacity, 10 * Time.deltaTime);
            ImageText.color = textOpacityColor;
        }

        private void UpdateImageOpacity()
        {
            float targetImageOpacity = Distance < MaxViewRange ? 1f : 0f;
            Color imageOpacityColor = ImageUI.color;
            imageOpacityColor.a = Mathf.Lerp(imageOpacityColor.a, targetImageOpacity, 10 * Time.deltaTime);
            ImageUI.color = imageOpacityColor;
        }

        private Vector3 calculateWorldPosition(Vector3 position, Camera camera)
        {
            Vector3 camNormal = camera.transform.forward;
            Vector3 vectorFromCam = position - camera.transform.position;
            float camNormDot = Vector3.Dot(camNormal, vectorFromCam.normalized);

            if (camNormDot <= 0f)
            {
                float camDot = Vector3.Dot(camNormal, vectorFromCam);
                Vector3 proj = (camNormal * camDot * 1.01f);
                position = camera.transform.position + (vectorFromCam - proj);
            }

            return position;
        }
    }
}
