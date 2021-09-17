using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static void OnChangeState(string sceneName)
    {
        Time.timeScale = 1f;
        Initiate.Fade(sceneName, Color.black, 1f);
    }
}

