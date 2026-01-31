using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UIResult : MonoBehaviour
{
    [Header("Listening on")]
    [SerializeField] private GameResultEventChannelSO onGameResult;

    [Header("UI")]
    [SerializeField] private Canvas ResultCanvas;
    [SerializeField] private TextMeshProUGUI txtResult;
    [SerializeField] private Canvas[] notResultCanvas;
    
    [Header("Game Data")]
    [SerializeField] private PlayerStateManager playerStateManager;


    private void OnEnable()
    {
        if (onGameResult != null)
        {
            onGameResult.OnEventRaised += OnGameResult;
        }
    }

    private void OnDisable()
    {
        if (onGameResult != null)
        {
            onGameResult.OnEventRaised -= OnGameResult;
        }
    }

    public void HideResult()
    {
        ResultCanvas.enabled = false;
        foreach (var c in notResultCanvas) c.enabled = true;
    }

    public void ShowResult()
    {
        Debug.Log("Showing Result UI");
        foreach (var c in notResultCanvas) c.enabled = false;
        ResultCanvas.enabled = true;
    }

    private void OnGameResult(GameResultData data)
    {
        ShowResult();
        if (txtResult != null)
        {
            txtResult.text = data.LocalPlayerWin ? "You Win!" : "You Lose!";
        }
    }
}
