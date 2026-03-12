using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class DeathmatchLivesUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DeathmatchMatchController deathmatchController;
    [SerializeField] private Transform livesRoot;

    [Header("Settings")]
    [SerializeField] private int maxLives = 3;
    [SerializeField] private bool hideWhenNotDeathmatch = true;

    private readonly List<GameObject> lifeIcons = new List<GameObject>();
    private int lastAppliedLives = -1;

    private void Awake()
    {
        ResolveReferences();
        CacheLifeIcons();
    }

    private void OnEnable()
    {
        lastAppliedLives = -1;
        RefreshNow();
    }

    private void Update()
    {
        RefreshNow();
    }

    private void ResolveReferences()
    {
        if (deathmatchController == null)
        {
            deathmatchController = FindFirstObjectByType<DeathmatchMatchController>();
        }

        if (livesRoot != null)
        {
            return;
        }

        var allTransforms = FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < allTransforms.Length; i++)
        {
            var t = allTransforms[i];
            if (t == null || t.name != "Lifes")
            {
                continue;
            }

            livesRoot = t;
            break;
        }
    }

    private void CacheLifeIcons()
    {
        lifeIcons.Clear();
        if (livesRoot == null)
        {
            return;
        }

        for (int i = 0; i < livesRoot.childCount; i++)
        {
            var child = livesRoot.GetChild(i);
            if (child == null)
            {
                continue;
            }

            // Treat direct children as life icons; supports Image objects and wrappers.
            lifeIcons.Add(child.gameObject);
        }
    }

    private void RefreshNow()
    {
        if (livesRoot == null)
        {
            ResolveReferences();
            CacheLifeIcons();
            if (livesRoot == null)
            {
                return;
            }
        }

        if (hideWhenNotDeathmatch && GameModeRuntime.IsDeathmatch == false)
        {
            if (livesRoot.gameObject.activeSelf)
            {
                livesRoot.gameObject.SetActive(false);
            }

            return;
        }

        if (livesRoot.gameObject.activeSelf == false)
        {
            livesRoot.gameObject.SetActive(true);
        }

        int lives = maxLives;
        if (TryReadLocalLives(out var syncedLives))
        {
            lives = syncedLives;
        }

        lives = Mathf.Clamp(lives, 0, Mathf.Max(0, maxLives));
        if (lives == lastAppliedLives)
        {
            return;
        }

        lastAppliedLives = lives;
        ApplyLives(lives);
    }

    private bool TryReadLocalLives(out int lives)
    {
        lives = maxLives;
        if (deathmatchController == null)
        {
            return false;
        }

        if (deathmatchController.TryGetLocalPlayerLives(out lives))
        {
            return true;
        }

        NetworkRunner runner = deathmatchController.Runner;
        if (runner != null && runner.IsRunning)
        {
            return deathmatchController.TryGetLives(runner.LocalPlayer, out lives);
        }

        return false;
    }

    private void ApplyLives(int lives)
    {
        int iconCount = lifeIcons.Count;
        if (iconCount == 0)
        {
            return;
        }

        for (int i = 0; i < iconCount; i++)
        {
            bool visible = i < lives;
            GameObject icon = lifeIcons[i];
            if (icon != null && icon.activeSelf != visible)
            {
                icon.SetActive(visible);
            }
        }
    }
}
