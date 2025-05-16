using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.MLAgents;

public class ScoreManager : MonoBehaviour
{
    Dictionary<string, int> score = new Dictionary<string, int>();
    public Dictionary<string, Vector3> resetPositions = new Dictionary<string, Vector3>();
    string lastHitBy = "Player1";
    bool hitGround = true;
    public ballScript ballScript;
    public TextMeshProUGUI scorePlayer;
    public TextMeshProUGUI scorePlayer1;

    // Reference to the ML agents
    public TennisAgent PlayerAgent;
    public TennisAgent Player1Agent;

    // Reference to the TennisAcademy
    private TennisAcademy academy;

    void Start()
    {
        // Find the TennisAcademy in the scene
        academy = FindObjectOfType<TennisAcademy>();
        if (academy == null)
        {
            Debug.LogError("TennisAcademy not found in scene! Cannot reset environment.");
        }
        else
        {
            // Manually call the custom initialize if override didn't work
            // If your Academy component does not have "Initialize Manually" checked, it might call its own Initialize.
            // You might need to test if this manual call is necessary or causes issues.
            // academy.CustomInitialize(); // Call if needed, test first without.
        }

        ballScript = GetComponent<ballScript>();
        scorePlayer.text = "0";
        scorePlayer1.text = "0";
        score["Player"] = 0;
        score["Player1"] = 0;
        resetPositions["Player"] = new Vector3(7.68f, 0.623f, 11.93f);
        resetPositions["Player1"] = new Vector3(21.51f, 0.623f, 14.4f);

        // Debug: Check agent references - moved here as agents might be initialized later
        // if (PlayerAgent == null) Debug.LogError("ScoreManager: PlayerAgent is not assigned!");
        // if (Player1Agent == null) Debug.LogError("ScoreManager: Player1Agent is not assigned!");
    }

    void Update()
    {
        scorePlayer.text = score["Player"].ToString();
        scorePlayer1.text = score["Player1"].ToString();
    }

    public void LastHitBy(string name)
    {
        Debug.Log($"{lastHitBy} a atins {hitGround} pamantu!");
        if (!hitGround)
        {
            score[lastHitBy]++;
            
            // Add rewards for scoring
            if (lastHitBy == "Player" && PlayerAgent != null)
            {
                PlayerAgent.AddScoreReward(1.0f);
                if (Player1Agent != null) Player1Agent.AddScoreReward(-1.0f);
            }
            else if (lastHitBy == "Player1" && Player1Agent != null)
            {
                Player1Agent.AddScoreReward(1.0f);
                if (PlayerAgent != null) PlayerAgent.AddScoreReward(-1.0f);
            }
            
            // Trigger episode end (point scored) and Academy reset
            EndEpisodeAndReset();
        }
        lastHitBy = name;
        hitGround = false;
    }

    public void HitGround()
    {
        hitGround = true;
        // Point ends when ball hits ground - Trigger episode end and Academy reset
        EndEpisodeAndReset();
    }

    public void WallHit()
    {
        if (hitGround)
        {
            score[lastHitBy]++;
            
            // Add rewards for scoring
            if (lastHitBy == "Player" && PlayerAgent != null)
            {
                PlayerAgent.AddScoreReward(1.0f);
                if (Player1Agent != null) Player1Agent.AddScoreReward(-1.0f);
            }
            else if (lastHitBy == "Player1" && Player1Agent != null)
            {
                Player1Agent.AddScoreReward(1.0f);
                if (PlayerAgent != null) PlayerAgent.AddScoreReward(-1.0f);
            }
            
            // Trigger episode end (point scored) and Academy reset
            EndEpisodeAndReset();
        }
        else
        {
            if (lastHitBy == "Player")
            {
                score["Player1"]++;
                if (Player1Agent != null) Player1Agent.AddScoreReward(1.0f);
                if (PlayerAgent != null) PlayerAgent.AddScoreReward(-1.0f);
                // Removed ball reset here, Academy will handle it
            }
            else
            {
                score["Player"]++;
                if (PlayerAgent != null) PlayerAgent.AddScoreReward(1.0f);
                if (Player1Agent != null) Player1Agent.AddScoreReward(-1.0f);
                // Removed ball reset here, Academy will handle it
            }
             // Point ends when ball hits wall after bounce - Trigger episode end and Academy reset
             EndEpisodeAndReset();
        }
    }

    // Helper method to trigger episode end and environment reset
    private void EndEpisodeAndReset()
    {
        // Agents need to call EndEpisode() to signal the end of their episode.
        // ML-Agents will then call OnEpisodeBegin() on all agents and the Academy.
        if (PlayerAgent != null) PlayerAgent.EndEpisode();
        if (Player1Agent != null) Player1Agent.EndEpisode();

        // The Academy's OnEpisodeBegin will be called automatically after agents EndEpisode.
        // If Academy.OnEpisodeBegin was not overridable, we would call CustomOnEpisodeBegin here.
        // Let's rely on the automatic call first.

        // Manual call if automatic doesn't work:
        // if (academy != null) academy.CustomOnEpisodeBegin();
    }
}
