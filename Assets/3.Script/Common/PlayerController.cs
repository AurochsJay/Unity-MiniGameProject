using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputManager input;
    [SerializeField] private LobbyManager lobby;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator anim;

    // Move
    public float walkSpeed = 2f;
    public float runSpeed = 5.5f;
    public float jumpForce = 5f;

    // x축, z축 입력에 따른 값을 normalized할 것임. 정규화해서 방향벡터만 가지도록
    // 이 speed값을 구하는 것은 x,z 입력받은 값을 magnitude하고 normalized하고 속력을 곱한다...
    private Vector2 speed;
    // 애니메이션 블렌드에 넣을 값
    private float animBlend;

    [Space(10)]
    // Jump
    public float jumpHeight = 1.5f;
    public float gravity = -15f;
    public bool isGrounded = true;

    // 점프를 하기 위해 필요한 시간, 착지하자마자 점프는 어색하니까. 0f가 됐을때 점프가 가능하도록
    // 착지모션에 도달하기 위해 필요한 시간, 아래계단으로 내려갈때 바로 fall모션 나오면 어색
    private float jumpTimeoutDelta = 0.5f;
    private float fallTimeoutDelta = 0.15f;

    // controller.Move할때 == 실제로 이동하는 메서드 곱해줄 속력.
    public float horizontalVelocity;
    public float verticalVelocity;

    public Vector3 velocity;
    private Vector3 rotation;

    // Animation IDs
    private int animSpeed;
    private int animJump;
    private int animGround;
    private int animFall;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        rotation = transform.rotation.eulerAngles;

        AssignAnimationIDs();
    }

    private void Update()
    {
        Move();
        Jump();
    }

    private void FixedUpdate()
    {
        CameraRotation();   
    }

    private void AssignAnimationIDs()
    {
        animSpeed = Animator.StringToHash("speed");
        animJump = Animator.StringToHash("isJump");
        animGround = Animator.StringToHash("isGround");
        animFall = Animator.StringToHash("isFall");
    }

    private void Move()
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

    private void CameraRotation()
    {

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
