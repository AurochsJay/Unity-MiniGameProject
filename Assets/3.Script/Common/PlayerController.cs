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

    // x��, z�� �Է¿� ���� ���� normalized�� ����. ����ȭ�ؼ� ���⺤�͸� ��������
    // �� speed���� ���ϴ� ���� x,z �Է¹��� ���� magnitude�ϰ� normalized�ϰ� �ӷ��� ���Ѵ�...
    private Vector2 speed;
    // �ִϸ��̼� ���忡 ���� ��
    private float animBlend;

    [Space(10)]
    // Jump
    public float jumpHeight = 1.5f;
    public float gravity = -15f;
    public bool isGrounded = true;

    // ������ �ϱ� ���� �ʿ��� �ð�, �������ڸ��� ������ ����ϴϱ�. 0f�� ������ ������ �����ϵ���
    // ������ǿ� �����ϱ� ���� �ʿ��� �ð�, �Ʒ�������� �������� �ٷ� fall��� ������ ���
    private float jumpTimeoutDelta = 0.5f;
    private float fallTimeoutDelta = 0.15f;

    // controller.Move�Ҷ� == ������ �̵��ϴ� �޼��� ������ �ӷ�.
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
