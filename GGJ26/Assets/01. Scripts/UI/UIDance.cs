using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDance : MonoBehaviour
{
    [SerializeField] private List<Animator> danceAnims;
    
    [Header("Listening to")]
    [SerializeField] private BoolEventChannelSO groupDanceActiveEvent;

    private void Awake()
    {
        if (danceAnims == null)
        {
            Log.E("Dance Animations not found");
        }
    }

    private void OnEnable()
    {
        groupDanceActiveEvent.OnEventRaised += DanceTime;
    }

    private void OnDisable()
    {
        groupDanceActiveEvent.OnEventRaised -= DanceTime;
    }

    private void DanceTime(bool isDance)
    {
        foreach (var danceAnim in danceAnims)
        {
            danceAnim.SetBool("isDancing", isDance);
        }
    }
}
