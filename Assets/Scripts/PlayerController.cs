using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public static PlayerController instance;
    public Rigidbody2D rb;
    public float movementSpeed;
    public Animator animator;
    public string areaTransitionName;
    public bool canMove = true;

    private Vector3 bottomLeftLimit;
    private Vector3 topRightLimit;

    private Vector2 movement;

    // Start is called before the first frame update
    void Start() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update() {
        if (!canMove) return;
        
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        animator.SetFloat("moveX", rb.velocity.x);
        animator.SetFloat("moveY", rb.velocity.y);

        if (Input.GetAxisRaw("Horizontal") == 1 || Input.GetAxisRaw("Horizontal") == -1
            || Input.GetAxisRaw("Vertical") == 1 || Input.GetAxisRaw("Vertical") == -1) {
            if (canMove) {
                animator.SetFloat("lastMoveX", Input.GetAxisRaw("Horizontal"));
                animator.SetFloat("lastMoveY", Input.GetAxisRaw("Vertical"));
            }
        }

        // keep the player inside the bounds
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, bottomLeftLimit.x, topRightLimit.x),
        Mathf.Clamp(transform.position.y, bottomLeftLimit.y,
        topRightLimit.y), transform.position.z);
    }

    private void FixedUpdate() {
        if (!canMove) {
            rb.velocity = Vector2.zero;
            return;
        }
        rb.MovePosition(rb.position + movement * movementSpeed * Time.fixedDeltaTime);
    }

    public void SetBounds(Vector3 bottomLeft, Vector3 topRight) {
        bottomLeftLimit = bottomLeft + new Vector3(.5f, .8f, 0);
        topRightLimit = topRight + new Vector3(-.5f, -.8f, 0);
    }
}
