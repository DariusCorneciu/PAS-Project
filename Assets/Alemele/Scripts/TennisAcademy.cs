using Unity.MLAgents;
using UnityEngine;

public class TennisAcademy : Academy
{
    [Header("Environment References")]
    [SerializeField] private GameObject ballGameObject;
    [SerializeField] private ScoreManager scoreManager;

    private Rigidbody ballRigidbody;

    // Initialize is called once at the start
    // Public method that can be called if needed, but Academy should handle init
    public void CustomInitialize()
    {
        Debug.Log("Academy Custom Initializing...");
        if (ballGameObject != null)
        {
            ballRigidbody = ballGameObject.GetComponent<Rigidbody>();
            if (ballRigidbody == null) Debug.LogError("Rigidbody not found on Ball GameObject assigned to Academy! Please assign the Ball GameObject in the Inspector.", ballGameObject);
        }
        else
        {
            Debug.LogError("Ball GameObject not assigned in the Academy Inspector! Please assign it.", this);
        }

        if (scoreManager == null)
        {
             Debug.LogError("ScoreManager not assigned in the Academy Inspector! Please assign it.", this);
        }

         // Also ensure the ScoreManager has the ballScript reference if it needs it
         if (scoreManager != null && scoreManager.ballScript == null)
         {
             scoreManager.ballScript = ballGameObject.GetComponent<ballScript>();
             if (scoreManager.ballScript == null) Debug.LogError("ballScript not found on Ball GameObject assigned to ScoreManager! Please add ballScript.", ballGameObject);
         }
    }

    // OnEpisodeBegin is called at the beginning of each episode
    // Public method that can be called, likely from ScoreManager when a point ends
    public void CustomOnEpisodeBegin()
    {
        Debug.Log("Academy Custom OnEpisodeBegin (New Point)...");

        // Reset ball position and velocity here, centralized in the Academy
        if (ballRigidbody != null && scoreManager != null && scoreManager.resetPositions != null)
        {
             // Decide which side serves (e.g., randomly choose player 1 or player 2 start position)
             // For now, let's reset it to one of the player's initial positions (e.g., Player's reset position)
             string servingPlayerName = "Player"; // Default serving player
             // TODO: Add logic here to alternate servingPlayerName, e.g., based on scoreManager or a counter.

             Vector3 resetPos;
             // Use TryGetValue for safety
             if (scoreManager.resetPositions.TryGetValue(servingPlayerName, out resetPos))
             {
                 ballRigidbody.linearVelocity = Vector3.zero;
                 ballRigidbody.angularVelocity = Vector3.zero;
                 ballGameObject.transform.position = resetPos;
                 Debug.Log($"Academy: Ball reset to {servingPlayerName}'s position: {resetPos}", ballGameObject);
             }
             else
             {
                 Debug.LogError($"Academy: ScoreManager.resetPositions does not contain key '{servingPlayerName}'! Cannot reset ball.", this);
                  // Fallback: reset ball to a default position if the serving player's name is not found
                 ballRigidbody.linearVelocity = Vector3.zero;
                 ballRigidbody.angularVelocity = Vector3.zero;
                 // TODO: You might need a general center court starting position here if names don't match
                 // ballGameObject.transform.position = new Vector3(..., ..., ...);
                  Debug.LogWarning("Academy: Falling back to general ball velocity reset.");
             }
        }
        else
        {
             Debug.LogError("Academy: Missing Rigidbody on Ball, ScoreManager, or resetPositions dictionary! Cannot reset ball.", this);
             // Fallback: just try to zero out velocity if Rigidbody exists
              if (ballRigidbody != null)
              {
                 ballRigidbody.linearVelocity = Vector3.zero;
                 ballRigidbody.angularVelocity = Vector3.zero;
                 Debug.LogWarning("Academy: ScoreManager or dictionary not available, just zeroing ball velocity.");
              }
        }

        // Agents should have their OnEpisodeBegin methods called automatically by the ML-Agents system
        // when an episode ends (e.g., via Agent.EndEpisode() or Academy.Done() if implemented).
        // Their OnEpisodeBegin methods should handle resetting their own position/state.
    }

    // You can override other methods like OnAcademyStep etc. if needed.
} 