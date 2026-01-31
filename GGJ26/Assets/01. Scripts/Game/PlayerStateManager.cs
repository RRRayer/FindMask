using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    private readonly Dictionary<string, PlayerState> players = new Dictionary<string, PlayerState>();
    private string localPlayerId;

    public event Action<PlayerState> OnPlayerStateChanged;

    public void RegisterPlayer(string playerId, bool isSeeker)
    {
        if (string.IsNullOrEmpty(playerId))
        {
            return;
        }

        PlayerState playerState;
        if (players.ContainsKey(playerId))
        {
            playerState = players[playerId];
            playerState.IsSeeker = isSeeker;
            Debug.Log($"[PlayerStateManager] Updating player {playerId}: IsSeeker = {isSeeker}");
        }
        else
        {
            playerState = new PlayerState(playerId, isSeeker);
            players[playerId] = playerState;
            Debug.Log($"[PlayerStateManager] Registering new player {playerId}: IsSeeker = {isSeeker}");
        }
        
        OnPlayerStateChanged?.Invoke(playerState);
    }

    public void SetLocalPlayer(string playerId)
    {
        localPlayerId = playerId;
        // When the local player is set, we might already have their state, so invoke the event.
        if (players.TryGetValue(playerId, out var playerState))
        {
            OnPlayerStateChanged?.Invoke(playerState);
        }
    }

    public void MarkDead(string playerId)
    {
        if (TryGetPlayer(playerId, out var state))
        {
            state.IsDead = true;
            OnPlayerStateChanged?.Invoke(state);
        }
    }

    public void AddElimination(string playerId, int amount = 1)
    {
        if (TryGetPlayer(playerId, out var state))
        {
            state.Eliminations += Mathf.Max(0, amount);
        }
    }

    public int GetAlivePlayerCount()
    {
        int count = 0;
        foreach (var entry in players.Values)
        {
            if (entry.IsDead == false)
            {
                count++;
            }
        }
        return count;
    }

    public int GetTotalPlayerCount()
    {
        return players.Count;
    }

    public PlayerState GetLastAlivePlayer()
    {
        if (GetAlivePlayerCount() != 1)
        {
            return null;
        }
        foreach (var entry in players.Values)
        {
            if (entry.IsDead == false)
            {
                return entry;
            }
        }
        return null;
    }

    public int GetAliveNonSeekersCount()
    {
        int count = 0;
        foreach (var entry in players.Values)
        {
            if (entry.IsSeeker == false && entry.IsDead == false)
            {
                count++;
            }
        }

        return count;
    }

    public int GetSeekersCount()
    {
        int count = 0;
        foreach (var entry in players.Values)
        {
            if (entry.IsSeeker)
            {
                count++;
            }
        }

        Debug.Log("Total Seekers Count: " + count);
        return count;
    }

    public int GetTotalNonSeekersCount()
    {
        int count = 0;
        foreach (var entry in players.Values)
        {
            if (entry.IsSeeker == false)
            {
                count++;
            }
        }
        Debug.Log("Total Non-Seekers Count: " + count);
        return count;
    }

    public bool AreAllNonSeekersDead()
    {
        // The game should end if there was at least one non-seeker to begin with, and now there are none left alive.
        return GetTotalNonSeekersCount() > 0 && GetAliveNonSeekersCount() == 0;
    }

    public bool TryGetLocalPlayer(out PlayerState state)
    {
        state = null;
        if (string.IsNullOrEmpty(localPlayerId))
        {
            return false;
        }

        return TryGetPlayer(localPlayerId, out state);
    }

    private bool TryGetPlayer(string playerId, out PlayerState state)
    {
        if (string.IsNullOrEmpty(playerId))
        {
            state = null;
            return false;
        }

        return players.TryGetValue(playerId, out state);
    }
}

