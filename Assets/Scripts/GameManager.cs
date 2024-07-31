using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Volume volume; // Reference to the Volume component
    public GameObject paperOverlay;
    private Material paperOverlayMaterial;
    private float originalPaperOverlayOpacity;
    private float targetOpacity; // The desired opacity (0.31 or 1.0)
    private float transitionDuration = 2.0f; // Duration of the transition in seconds
    public GameObject scoreMenu;
    void Start()
    {
        // Get the material from the Renderer component
        paperOverlayMaterial = paperOverlay.GetComponent<Renderer>().material;

        // Save the original opacity (alpha value)
        originalPaperOverlayOpacity = paperOverlayMaterial.color.a;
    }
    private IEnumerator DecreaseOpacity(float target)
    {
        float elapsedTime = 0.0f;
        Color startColor = paperOverlayMaterial.color;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / transitionDuration);
            Color newColor = Color.Lerp(startColor, new Color(startColor.r, startColor.g, startColor.b, target), t);
            paperOverlayMaterial.color = newColor;
            yield return null;
        }
    }
    private IEnumerator IncreaseOpacity(float target)
    {
        float elapsedTime = 0.0f;
        Color startColor = paperOverlayMaterial.color;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / transitionDuration);
            Color newColor = Color.Lerp(startColor, new Color(startColor.r, startColor.g, startColor.b, target), t);
            paperOverlayMaterial.color = newColor;
            yield return null;
        }
    }
    public void DecreaseOpacity()
    {
        targetOpacity = 0.31f;
        StartCoroutine(DecreaseOpacity(targetOpacity));
    }

    public void IncreaseOpacity()
    {
        targetOpacity = 1.0f;
        StartCoroutine(IncreaseOpacity(targetOpacity));
    }

    public void ScoreMenu()
    {
        scoreMenu.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
