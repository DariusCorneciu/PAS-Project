using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class TennisAgent : Agent
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private Transform ball;
    [SerializeField] private Transform target;
    [SerializeField] private float hitForce = 15f;
    [SerializeField] private float upForce = 8f;
    
    private Animator animator;
    private ShotManager shotManager;
    private Shot currentShot;
    private Vector3 initialPosition;
    private Rigidbody ballRigidbody;
    private ScoreManager scoreManager;

    public override void Initialize()
    {
        animator = GetComponent<Animator>();
        shotManager = GetComponent<ShotManager>();
        if (shotManager == null)
        {
            Debug.LogError("ShotManager component not found on the Agent GameObject! Please add it.", this);
        }
        else
        {
            currentShot = shotManager.topSpin;
        }
        initialPosition = transform.position;
        
        if (ball != null)
        {
            ballRigidbody = ball.GetComponent<Rigidbody>();
            scoreManager = ball.GetComponent<ScoreManager>();
            if (ballRigidbody == null) Debug.LogError("Rigidbody not found on Ball GameObject!", ball);
            if (scoreManager == null) Debug.LogError("ScoreManager not found on Ball GameObject!", ball);
            Debug.Log($"Test2");
        }
        else
        {
            Debug.LogError("Ball Transform is not assigned in the Inspector!", this);
        }
    }

    public override void OnEpisodeBegin()
    {
        transform.position = initialPosition;
        target.position = transform.position + Vector3.forward;
        Debug.Log($"Test");
        if (scoreManager != null && scoreManager.ballScript != null && scoreManager.resetPositions != null)
        {
             if (scoreManager.resetPositions.ContainsKey(gameObject.name))
             {
                 Vector3 resetPos = scoreManager.resetPositions[gameObject.name];
                 scoreManager.ballScript.setInitialPosition(resetPos);
                 Debug.Log($"Agent {gameObject.name}: Ball reset to position from dictionary.", this);
             }
             else
             {
                 Debug.LogWarning($"ScoreManager.resetPositions does not contain key '{gameObject.name}'. Using initial position for reset.", this);
                 if (ballRigidbody != null)
                 {
                    ballRigidbody.linearVelocity = Vector3.zero;
                    ballRigidbody.angularVelocity = Vector3.zero;
                    ball.position = initialPosition;
                    Debug.Log($"Agent {gameObject.name}: Ball reset to agent's initial position as fallback.", this);
                 }
                 else
                 {
                      Debug.LogWarning($"Agent {gameObject.name}: Cannot reset ball position, Rigidbody is null.", this);
                 }
             }
        }
        else
        {
             Debug.LogWarning($"Agent {gameObject.name}: ScoreManager, ballScript, or resetPositions dictionary not available to reset ball position.", this);
             if (ballRigidbody != null)
             {
                 ballRigidbody.linearVelocity = Vector3.zero;
                 ballRigidbody.angularVelocity = Vector3.zero;
                 Debug.Log($"Agent {gameObject.name}: Ball velocity reset as fallback.", this);
             }
             else
             {
                 Debug.LogWarning($"Agent {gameObject.name}: Cannot reset ball velocity, Rigidbody is null.", this);
             }
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (ball == null || ballRigidbody == null || target == null)
        {
             Debug.LogError("Missing Ball, Ball Rigidbody, or Target reference on " + gameObject.name + "! Cannot collect observations.", this);
             sensor.AddObservation(new float[12]);
             return;
        }

        sensor.AddObservation(transform.position);
        
        sensor.AddObservation(ball.position);
        sensor.AddObservation(ballRigidbody.linearVelocity);
        
        sensor.AddObservation(target.position);
        
        sensor.AddObservation(Vector3.Distance(transform.position, ball.position));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Debug.Log($"OnActionReceived entered for {gameObject.name}", this);

        if (target == null) return;

        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        
        float hitAction = actions.ContinuousActions[2];
        
        Debug.Log($"Agent {gameObject.name} received actions: X={moveX}, Z={moveZ}, Hit={hitAction}");

        Vector3 movement = new Vector3(moveX, 0, moveZ) * speed * Time.deltaTime;
        transform.Translate(movement);
        
        Debug.Log($"Agent {gameObject.name} calculated movement: {movement}");

        if (hitAction > 0.5f)
        {
            target.Translate(new Vector3(moveX, 0, 0) * (speed * 2) * Time.deltaTime);
        }
        
        AddReward(-0.001f);
        Debug.Log($"Agent {gameObject.name} received step penalty: -0.001", this);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
        
        continuousActionsOut[2] = Input.GetKey(KeyCode.Space) ? 1f : 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            if (target == null || scoreManager == null || animator == null)
            {
                 Debug.LogError("Missing Target, ScoreManager, or Animator on " + gameObject.name + "! Cannot process ball hit.", this);
                 return;
            }

            Vector3 hitDirection = target.position - transform.position;
            Rigidbody ballRb = other.GetComponent<Rigidbody>();
            ScoreManager ballScoreManager = other.GetComponent<ScoreManager>();

            if (ballRb != null)
            {
                ballRb.linearVelocity = hitDirection.normalized * hitForce + new Vector3(0, upForce, 0);
            }
            else
            {
                Debug.LogError("Rigidbody not found on Ball GameObject when hitting!", other.gameObject);
            }

            if (ballScoreManager != null)
            {
                ballScoreManager.LastHitBy(gameObject.name);
            }
            else
            {
                 Debug.LogError("ScoreManager not found on Ball GameObject when hitting!", other.gameObject);
            }
            
            var leftRight = other.transform.position - transform.position;
            if (leftRight.z <= 0)
            {
                animator.Play("normal");
            }
            else
            {
                animator.Play("rever");
            }
            
            AddReward(0.1f);
        }
    }

    public void AddScoreReward(float reward)
    {
        AddReward(reward);
    }
} 