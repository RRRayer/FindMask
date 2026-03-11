using UnityEngine;

public class UIDead : MonoBehaviour
{
    [SerializeField] private Canvas CanvasDead;
    [SerializeField] private Canvas[] notDeadCanvas;

    public void HideDeadUI()
    {
        GameObject deadObject = GetDeadObject();
        if (deadObject != null)
        {
            deadObject.SetActive(false);
        }

        foreach (var c in notDeadCanvas)
        {
            if (c != null)
            {
                c.gameObject.SetActive(true);
            }
        }
    }

    public void ShowDeadUI()
    {
        Debug.Log("Showing Dead UI");
        foreach (var c in notDeadCanvas)
        {
            if (c != null)
            {
                c.gameObject.SetActive(false);
            }
        }

        GameObject deadObject = GetDeadObject();
        if (deadObject != null)
        {
            deadObject.SetActive(true);
        }
    }

    private GameObject GetDeadObject()
    {
        if (CanvasDead != null)
        {
            return CanvasDead.gameObject;
        }

        return gameObject;
    }
}
