using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    Dictionary<string, int> score = new Dictionary<string, int>();
    Dictionary<string, Vector3> resetPositions = new Dictionary<string, Vector3>();
    string lastHitBy = "Player2";
    bool hitGround = true;
    ballScript ballScript;
   public TextMeshProUGUI scorePlayer1;
    public TextMeshProUGUI scorePlayer2;


    

    void Start()
    {
        ballScript = GetComponent<ballScript>();
        scorePlayer1.text = "0";
        scorePlayer2.text = "0";
        score["Player1"] = 0;
        score["Player2"] = 0;
        resetPositions["Player1"] = new Vector3((float)7.68, (float)0.623, (float)11.93);
        resetPositions["Player2"] = new Vector3((float)21.51, (float)0.623, (float)14.4);

    }

   
    void Update()
    {
        scorePlayer1.text = score["Player1"].ToString();
        scorePlayer2.text = score["Player2"].ToString();

    }

   public void LastHitBy(string name)
    {
        Debug.Log($"{lastHitBy} a atins {hitGround} pamantu!");
        if (!hitGround)
        {
            score[lastHitBy]++;
           
            ballScript.setInitialPosition(resetPositions[lastHitBy]);

        }
        lastHitBy = name;
        hitGround = false;
    }
    public void HitGround()
    {
        hitGround = true;
    }

    public void WallHit()
    {
        if (hitGround) {
            score[lastHitBy]++;
            ballScript.setInitialPosition(resetPositions[lastHitBy]);
        }
        else
        {
            if(lastHitBy == "Player1")
            {
                score["Player2"]++;
                ballScript.setInitialPosition(resetPositions["Player2"]);
            }
            else
            {
                score["Player1"]++;
                ballScript.setInitialPosition(resetPositions["Player1"]);
            }
        }
    }

}
