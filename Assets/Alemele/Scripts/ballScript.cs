using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ballScript : MonoBehaviour
{
    Vector3 initialPosition;
    // ScoreManager scoreManager; // This reference is obtained in Start/OnCollisionEnter, no need for a class variable unless used elsewhere.

    // Remove references to old player scripts
    // public BotScript bot;
    // public playermovemont player;

    void Start()
    {
        // scoreManager is obtained here, assuming ScoreManager is on the same GameObject
        // scoreManager = GetComponent<ScoreManager>(); // Already exists
    }

    // OnCollisionEnter logic seems fine, interacts with ScoreManager on the ball
    private void OnCollisionEnter(Collision collision)
    {
        // Ensure ScoreManager is found before calling methods
        ScoreManager sm = GetComponent<ScoreManager>();
        if (sm != null)
        {
            if (collision.transform.CompareTag("Ground"))
            {
                sm.HitGround();
            }
            if (collision.transform.CompareTag("Wall"))
            {
                sm.WallHit();
            }
        }
        else
        {
            Debug.LogError("ScoreManager not found on the Ball GameObject! Cannot process collisions.", this);
        }
    }

    public void setInitialPosition(Vector3 pos)
    {
        // Reset ball physics and position
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.position = pos;
        }
        else
        {
            Debug.LogError("Rigidbody not found on the Ball GameObject! Cannot set initial position.", this);
        }

        // REMOVE calls to old player scripts' ResetPosition - Agent reset is handled by OnEpisodeBegin
        // bot.ResetPosition();
        // bot.ResetPosition(); // This was likely a copy-paste error anyway

        // The ScoreManager will handle triggering agent resets (via EndEpisode/OnEpisodeBegin) when needed.
    }
}
