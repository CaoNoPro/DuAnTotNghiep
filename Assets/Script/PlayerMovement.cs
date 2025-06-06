using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    public float speed = 12f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;
    public float RunSpeed = 20f; // Speed when running

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;

    bool isGrounded;
    bool isMoving; 
    private Vector3 lastPosition = new Vector3(0f,0f,0f);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if(isGrounded && velocity.y<0)
        {
            velocity.y = -2f; //resetting the velocity when grounded
        }
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); //jumping
        }

        if(Input.GetKey(KeyCode.LeftShift)) // Check if the left shift key is pressed for running
        {
            controller.Move(move * RunSpeed * Time.deltaTime); // Move at running speed
        }
        else
        {
            controller.Move(move * speed * Time.deltaTime); // Move at normal speed
        }

        velocity.y += gravity * Time.deltaTime; //applying gravity

        controller.Move(velocity * Time.deltaTime);

        if(lastPosition != gameObject.transform.position && isGrounded == true)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        lastPosition =gameObject.transform.position;
    }
}
