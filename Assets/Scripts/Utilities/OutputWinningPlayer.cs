using UnityEngine;
using UnityEngine.UI;

public class OutputWinningPlayer : MonoBehaviour
{
    public Text WinTextObj;
    private void Start() => OutputWinText();
    private void OutputWinText() => WinTextObj.text = $"Player {StateChange.VictoriousPlayer} Wins!!";
}
