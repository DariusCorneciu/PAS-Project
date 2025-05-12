using UnityEngine;

public class BotScript : MonoBehaviour
{
    float speed = 3f;
    Animator animator;
    public Transform ball;
    public Transform target;

    float force = 10f;
    Vector3 targetPosition;
    float vertical = 0f;
    float horizontal = 0f;
    public bool eBot = false;
    bool isHitting;
    ShotManager shotManager;
    Shot currentShot;
    void Start()
    {
        targetPosition = transform.position;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {

        if (eBot)
        {
            targetPosition.z = ball.position.z;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
        else{

            float vertical = (Input.GetKey(KeyCode.UpArrow) ? 1f : 0f) - (Input.GetKey(KeyCode.DownArrow) ? 1f : 0f);
            float horizontal = (Input.GetKey(KeyCode.RightArrow) ? 1f : 0f) - (Input.GetKey(KeyCode.LeftArrow) ? 1f : 0f);



            if (Input.GetKeyDown(KeyCode.RightShift))
            {
                isHitting = true;
            }
            else if (Input.GetKeyUp(KeyCode.RightShift))
            {
                isHitting = false;
            }

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                isHitting = true;
                currentShot = shotManager.flat;
            }
            else if (Input.GetKeyUp(KeyCode.LeftControl))
            {

                isHitting = false;
            }

            if (isHitting)
            {
                target.Translate(new Vector3(horizontal, 0, 0) * (speed * 2) * Time.deltaTime);
            }


            if ((horizontal != 0 || vertical != 0) && !isHitting)
            {
                transform.Translate(new Vector3(horizontal, 0, vertical) * speed * Time.deltaTime);
            }
        }
       

        /////asta o tii pentru un semi bot
      

    }

    public void ResetPosition()
    {
        transform.position = new Vector3((float)21.88, (float)0.18, (float)16.08);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Vector3 vector3 = target.position - transform.position;
            other.GetComponent<Rigidbody>().linearVelocity = vector3.normalized * force + new Vector3(0, 6, 0);
            other.GetComponent<ScoreManager>().LastHitBy("Player2");
            var leftRight = other.transform.position - transform.position;

            if (leftRight.z <= 0)
            {
                animator.Play("normal");
            }
            else
            {
                animator.Play("rever");
            }
        }
    }
}
