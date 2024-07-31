using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    GameManager gameManager;
    private Color originalColor; // Variable to store the original color
    private Volume volume; // Reference to the Volume component
    public float rShift, gShift, bShift;
    ColorAdjustments colorAdjustments;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }
    void Start()
    {
        volume = gameManager.volume;
        if (volume.profile.TryGet(out colorAdjustments))
        {
            // Save the original color
            originalColor = colorAdjustments.colorFilter.value;
            Debug.Log(originalColor);
        }
    }

    public void AdvanceTime()
    {
        // Set the new color to black
        colorAdjustments.colorFilter.value = new Color(originalColor.r - rShift, originalColor.g - gShift, originalColor.b - bShift);
        originalColor = colorAdjustments.colorFilter.value;
        Debug.Log(colorAdjustments.colorFilter.value);
    }
}

