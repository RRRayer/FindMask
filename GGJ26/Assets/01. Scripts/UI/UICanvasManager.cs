using UnityEngine;

public class UICanvasManager : MonoBehaviour
{
    [SerializeField] private Canvas seekerCanvas;
    [SerializeField] private Canvas hiderCanvas;

    public void ResetForNewRound()
    {
        if (seekerCanvas != null)
        {
            seekerCanvas.enabled = false;
        }

        if (hiderCanvas != null)
        {
            hiderCanvas.enabled = true;
        }
    }

    public void EnableSeekerCanvas()
    {
        if (seekerCanvas != null)
        {
            seekerCanvas.enabled = true;
        }

        if (hiderCanvas != null)
        {
            hiderCanvas.enabled = true;
        }

        Debug.Log("Seeker canvas enabled");
    }

    public void EnableHiderCanvas()
    {
        if (seekerCanvas != null)
        {
            seekerCanvas.enabled = false;
        }

        if (hiderCanvas != null)
        {
            hiderCanvas.enabled = true;
        }

        Debug.Log("Hider canvas enabled");
    }
}
