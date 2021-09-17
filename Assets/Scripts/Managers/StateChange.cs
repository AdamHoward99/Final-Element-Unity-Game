using UnityEngine;

public class StateChange : MonoBehaviour
{
    public static string CurrentLevel { get; set; }
    public static int VictoriousPlayer { get; set; }

    public void MainMenu() => GameManager.OnChangeState("Menu");
    public void NatureLevel() => GameManager.OnChangeState("Nature Level");
    public void MechanicalLevel() => GameManager.OnChangeState("Mechanical Level");
    public void SpaceLevel() => GameManager.OnChangeState("Space Level");
    public void DungeonLevel() => GameManager.OnChangeState("Dungeon Level");
    public void PlayAgain() => GameManager.OnChangeState(CurrentLevel);
    public void QuitGame() => Application.Quit();
}