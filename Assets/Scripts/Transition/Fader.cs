using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    public bool Start { private get; set; }
    public float FadeDamp { private get; set; }
    public string FadeScene { private get; set; }
    public Color FadeColour { private get; set; }

    private float Alpha { get; set; }
    private bool IsFadeIn { get; set; }

    CanvasGroup myCanvas;
    Image bg;
    float lastTime = 0;
    bool startedLoading = false;
    //Set callback
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }
    //Remove callback
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    public void InitiateFader()
    {
        Start = true;
        DontDestroyOnLoad(gameObject);

        //Getting the visual elements
        if (transform.GetComponent<CanvasGroup>())
            myCanvas = transform.GetComponent<CanvasGroup>();

        if (transform.GetComponentInChildren<Image>())
        {
            bg = transform.GetComponent<Image>();
            bg.color = FadeColour;
        }

        myCanvas.alpha = 0.0f;
        StartCoroutine(FadeIt());
    }

    IEnumerator FadeIt()
    {

        while (!Start)
        {
            //waiting to start
            yield return null;
        }
        lastTime = Time.time;
        float coDelta = lastTime;
        bool hasFadedIn = false;

        while (!hasFadedIn)
        {
            coDelta = Time.time - lastTime;
            if (!IsFadeIn)
            {
                //Fade in
                Alpha = newAlpha(coDelta, 1, Alpha);
                if (Alpha == 1 && !startedLoading)
                {
                    startedLoading = true;
                    SceneManager.LoadScene(FadeScene);
                }

            }
            else
            {
                //Fade out
                Alpha = newAlpha(coDelta, 0, Alpha);
                if (Alpha == 0)
                {
                    hasFadedIn = true;
                }


            }
            lastTime = Time.time;
            myCanvas.alpha = Alpha;
            yield return null;
        }

        Initiate.DoneFading();
        Destroy(gameObject);
        yield return null;
    }


    float newAlpha(float delta, int to, float currAlpha)
    {

        switch (to)
        {
            case 0:
                currAlpha -= FadeDamp * delta;
                if (currAlpha <= 0)
                    currAlpha = 0;

                break;
            case 1:
                currAlpha += FadeDamp * delta;
                if (currAlpha >= 1)
                    currAlpha = 1;

                break;
        }

        return currAlpha;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeIt());
        //We can now fade in
        IsFadeIn = true;
    }
}
