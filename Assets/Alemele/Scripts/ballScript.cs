using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ballScript : MonoBehaviour
{
    Vector3 initialPosition;
    ScoreManager scoreManager;

    public BotScript bot;
    public playermovemont player;
    void Start()
    {
       
        scoreManager = GetComponent<ScoreManager>();
    }

   

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            scoreManager.HitGround();
        }
        if (collision.transform.CompareTag("Wall"))
        {
            scoreManager.WallHit();
            
        }

    }

    public void setInitialPosition(Vector3 pos)
    {
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        transform.position = pos;
        bot.ResetPosition();
        bot.ResetPosition();

    }
}
