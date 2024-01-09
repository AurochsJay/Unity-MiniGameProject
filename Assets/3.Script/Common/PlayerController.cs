using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum State { idle, walk, run, jump};

    [SerializeField] private InputManager input;
    [SerializeField] private LobbyManager lobby;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator anim;

    private State action = State.idle;
    public Vector3 velocity;
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 5f;

    private Vector3 rotation;
    private bool isGrounded = true;

    private void Start()
    {
        controller = this.GetComponent<CharacterController>();
        anim = this.GetComponent<Animator>();
        rotation = transform.rotation.eulerAngles;
    }

    private void Update()
    {
        CheckState();
        ActByState();
        Rotate();
        //Move();
        //Rotate();
        //Jump();
    }

    private void CheckState()
    {
        if((input.move_X !=0 || input.move_Z != 0) && isGrounded)
        {
            action = State.walk;

            if(input.press_Shift)
            {
                action = State.run;
            }
        }
        
        if(input.press_Space && isGrounded)
        {
            action = State.jump;
        }

        if(input.move_X == 0 && input.move_Z ==0 && !input.press_Space)
        {
            action = State.idle;
        }
    }

    private void ActByState()
    {
        switch (action)
        {
            case State.idle:
                Idle();
                break;
            case State.walk:
                Walk();
                break;
            case State.run:
                Run();
                break;
            case State.jump:
                Jump();
                break;
        }
    }

    private void Idle()
    {
        anim.SetBool("isWalk", false);
        anim.SetBool("isRun", false);
        anim.SetBool("isJump", false);
        velocity = Vector3.zero;
    }

    private void Walk()
    {
        anim.SetBool("isWalk", true);

        Vector3 moveDir = new Vector3(input.move_X, 0f, input.move_Z);
        velocity = moveDir * walkSpeed * Time.deltaTime;
        controller.Move(velocity);
        //transform.Translate(moveDir * walkSpeed * Time.deltaTime);
    }

    private void Run()
    {
        anim.SetBool("isRun", true);

        Vector3 moveDir = new Vector3(input.move_X, 0f, input.move_Z);
        velocity = moveDir * runSpeed * Time.deltaTime;
        controller.Move(velocity);
    }

    private void Jump()
    {
        anim.SetBool("isJump", true);
        isGrounded = false;

        Vector3 moveDir = Vector3.up * jumpForce * Time.deltaTime;
        velocity = velocity + moveDir;
        controller.Move(velocity);
    }

    private void Rotate()
    {
        rotation.y += input.mouse_Rotate_Y;
        transform.rotation = Quaternion.Euler(rotation);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("GameSelector"))
        {
            lobby.gKeyImage.SetActive(true);

            if (input.press_G)
            {
                lobby.showGameTitleImage.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("GameSelector"))
        {
            lobby.gKeyImage.SetActive(false);
            lobby.showGameTitleImage.SetActive(false);
        }
    }

}
