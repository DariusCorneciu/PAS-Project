using UnityEngine;

public class playermovemont : MonoBehaviour
{
    [SerializeField]
    private float speed = 2f;
    [SerializeField]
    private Transform target;

    bool isHitting;
    float horizontal = 0f;
    float vertical = 0f;

    Animator animator;

    ShotManager shotManager;
    Shot currentShot;

    void Start()
    {
        shotManager = GetComponent<ShotManager>();
        currentShot = shotManager.topSpin;
        animator = GetComponent<Animator>();
    }

    public void ResetPosition()
    {
        transform.position = new Vector3((float)6.66, (float)0.18, (float)10.23);
    }
 
    
    void Update()
    {
        float vertical = (Input.GetKey(KeyCode.W) ? 1f : 0f) - (Input.GetKey(KeyCode.S) ? 1f : 0f);
        float horizontal = (Input.GetKey(KeyCode.D) ? 1f : 0f) - (Input.GetKey(KeyCode.A) ? 1f : 0f);




        if (Input.GetKeyDown(KeyCode.Space))
        {
            isHitting = true;
        }else if (Input.GetKeyUp(KeyCode.Space))
        {
            isHitting = false;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            isHitting = true;
            currentShot = shotManager.flat;
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            
            isHitting = false;
        }

        if (isHitting) {
            target.Translate(new Vector3(horizontal, 0,0 ) * (speed*2) * Time.deltaTime);
        }
  

        if ((horizontal != 0 || vertical != 0) && !isHitting)
        {
            transform.Translate(new Vector3(horizontal, 0, vertical) * speed * Time.deltaTime);
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Vector3 vector3 = target.position - transform.position;
            other.GetComponent<Rigidbody>().linearVelocity = vector3.normalized * currentShot.hitForce + new Vector3(0, currentShot.upForce, 0);
            other.GetComponent<ScoreManager>().LastHitBy("Player1");
            var leftRight = other.transform.position- transform.position;

            if(leftRight.z <= 0) {
                animator.Play("normal");
            }
            else {
                animator.Play("rever");
            }

            currentShot = shotManager.topSpin;

        }

    }
}
