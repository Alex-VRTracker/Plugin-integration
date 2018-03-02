using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class VRTrackerScreenFader : MonoBehaviour
{

    public Image imageFaded;
    public float fadeSpeed = 1.5f;
    public bool sceneStarting = true;
    public bool fading = false;
    public float fadeDuration = 1f;
    private float startTime;

    void Awake()
    {
        imageFaded.rectTransform.localScale = new Vector2(Screen.width, Screen.height);
    }

    void Update()
    {
        // If the scene is starting...
        if (sceneStarting)
            // ... call the StartScene function.
            StartScene();
        if (fading)
            EndFadeRotation();
    }


    void FadeToClear()
    {
        // Lerp the colour of the image between itself and transparent.
        imageFaded.color = Color.Lerp(imageFaded.color, Color.clear, fadeSpeed * Time.deltaTime);
    }


    void FadeToBlack()
    {
        // Lerp the colour of the image between itself and black.
        Debug.Log("Fading duration " +  Time.deltaTime);
        Debug.Log("Fading a " + imageFaded.color.a);

        //imageFaded.color = Color.Lerp(imageFaded.color, Color.black, fadeSpeed * Time.deltaTime);

        Debug.Log("Fading after " + imageFaded.color.a);

    }


    void StartScene()
    {
        // Fade the texture to clear.
        FadeToClear();

        // If the texture is almost clear...
        if (imageFaded.color.a <= 0.05f)
        {
            // ... set the colour to clear and disable the RawImage.
            imageFaded.color = Color.clear;
            imageFaded.enabled = false;

            // The scene is no longer starting.
            sceneStarting = false;
        }
    }


    public IEnumerator EndSceneRoutine(int SceneNumber)
    {
        // Make sure the RawImage is enabled.
        imageFaded.enabled = true;
        do
        {
            // Start fading towards black.
            FadeToBlack();

            // If the screen is almost black...
            if (imageFaded.color.a >= 0.95f)
            {
                // ... reload the level
                SceneManager.LoadScene(SceneNumber);
                yield break;
            }
            /*else
            {
                yield return null;
            }*/
            //yield return WaitForSeconds(0.1f);
        } while (true);
    }

    public void EndScene(int SceneNumber)
    {
        sceneStarting = false;
        //startTime = Time.deltaTime;
        StartCoroutine("EndSceneRoutine", SceneNumber);
    }

    public IEnumerator RotationFadeRoutine()
    {
        // Make sure the RawImage is enabled.
        Debug.Log("Fading");
        imageFaded.enabled = true;
        do
        {
            // Start fading towards black.
            FadeToBlack();
            Debug.Log("Fading to black");

            // If the screen is almost black...
            if (imageFaded.color.a >= 0.95f && startTime < fadeDuration)
            {
                yield break;
            }
            else
            {
                yield return null;
            }
        } while (true);
    }

    public void FadeRotation()
    {
        sceneStarting = false;
        startTime = Time.deltaTime;
        StartCoroutine("RotationFadeRoutine");
    }

    void EndFadeRotation()
    {
        // Fade the texture to clear.
        FadeToClear();

        // If the texture is almost clear...
        if (imageFaded.color.a <= 0.05f)
        {
            // ... set the colour to clear and disable the RawImage.
            imageFaded.color = Color.clear;
            imageFaded.enabled = false;

            // The scene is no longer starting.
            fading = false;
        }
    }
}
