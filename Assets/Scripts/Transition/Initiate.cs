using UnityEngine;
using UnityEngine.UI;

public static class Initiate
{
    static bool areWeFading = false;

    //Create Fader object and assing the fade scripts and assign all the variables
    public static void Fade(string scene, Color col, float multiplier)
    {
        if (areWeFading)
            return;

        GameObject init = new GameObject();
        init.name = "Fader";
        Canvas myCanvas = init.AddComponent<Canvas>();
        myCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        myCanvas.sortingOrder = 1;
        init.AddComponent<Fader>();
        init.AddComponent<CanvasGroup>();
        init.AddComponent<Image>();
        //Put this into function in fader class instead of using variables here
        Fader scr = init.GetComponent<Fader>();
        scr.FadeDamp = multiplier;
        scr.FadeScene = scene;
        scr.FadeColour = col;
        areWeFading = true;
        scr.InitiateFader();
        
    }

    public static void DoneFading() {
        areWeFading = false;
    }
}
