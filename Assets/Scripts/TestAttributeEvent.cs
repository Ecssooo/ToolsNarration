using System;
using UnityEngine;
using UnityEngine.UI;

public class TestAttributeEvent : MonoBehaviour
{
    [SerializeField] private Image bgImage;
    [SerializeField] private Camera mainCamera;

    [DialogueEvent]
    public void ChangeBackgroundColor(string newColor)
    {
        if (bgImage != null)
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(newColor, out color))
            {
                bgImage.color = color;
            }
            else
            {
                Debug.LogWarning("Invalid color string: " + newColor);
            }
        }
    }

    [DialogueEvent]
    public void ChangeBackgroundColorToRed()
    {
        ChangeBackgroundColor("red");
    }

    [DialogueEvent]
    public void CameraShake()
    {
        if (mainCamera != null)
        {
            // Simple shake effect by changing the camera's position randomly
            Vector3 originalPosition = mainCamera.transform.position;
            float shakeAmount = 0.1f;
            int shakeCount = 10;

            for (int i = 0; i < shakeCount; i++)
            {
                float offsetX = UnityEngine.Random.Range(-shakeAmount, shakeAmount);
                float offsetY = UnityEngine.Random.Range(-shakeAmount, shakeAmount);
                mainCamera.transform.position = new Vector3(originalPosition.x + offsetX, originalPosition.y + offsetY, originalPosition.z);
            }

            mainCamera.transform.position = originalPosition; // Reset to original position
        }
    }

}
