using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputManager input;
    [SerializeField] private LobbyManager lobby;
    [SerializeField] private GameObject mainCamera; 
    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator anim;

    [Header("Move")]
    // Move
    public float walkSpeed = 2f;
    public float runSpeed = 5.5f;
    
    // �Է¹��� �ӷ� -> ��ǥ�� �ϴ� �ӷ����� ��ȯ�ϴ� ���� (Lerp)
    public float speedChangeRate = 10f;

    // x��, z�� �Է¿� ���� ���� normalized�� ����. ����ȭ�ؼ� ���⺤�͸� ��������
    // �� speed���� ���ϴ� ���� x,z �Է¹��� ���� magnitude�ϰ� normalized�ϰ� �ӷ��� ���Ѵ�...
    private float speed;
    // �ִϸ��̼� ���忡 ���� ��
    private float animBlend;

    // ��ǥ�� �ϴ� ȸ����
    private float targetRotation = 0f;
    private float rotationVelocity;
    public float rotationSmoothTime = 0.12f;

    [Header("Jump")]
    // Jump
    public float jumpHeight = 1.5f;
    public float gravity = -15f;
    public bool isGrounded = true;
    public float groundOffset = -0.14f;
    public float groundRadius = 0.28f;
    public LayerMask groundLayers;

    public float jumpTimeout = 0.5f;
    public float fallTimeout = 0.15f;

    // ������ �ϱ� ���� �ʿ��� �ð�, �������ڸ��� ������ ����ϴϱ�. 0f�� ������ ������ �����ϵ���
    // ������ǿ� �����ϱ� ���� �ʿ��� �ð�, �Ʒ�������� �������� �ٷ� fall��� ������ ���
    private float jumpTimeoutDelta;// ���� ������ ���� �ð��� ��Ÿ���� ����
    private float fallTimeoutDelta;

    // controller.Move�Ҷ� == ������ �̵��ϴ� �޼��� ������ �ӷ�.
    public float horizontalVelocity;
    public float verticalVelocity;

    private float terminalVelocity = 53f; // �߷¿� ���� �ִ� �ӷ�

    public Vector3 velocity;
    private Vector3 rotation;

    
    // Animation IDs
    private int animIDSpeed;
    private int animIDJump;
    private int animIDGround;
    private int animIDFall;

    [Header("Cinemachine")]
    [SerializeField] private GameObject cinemachineCameraTarget;
    public float cameraRotateSpeed = 2f;
    public float TopClamp = 70f;
    public float BottomClamp = -30f;
    private const float threshold = 0.01f;

    // Roll : x�� �߽����� ȸ��, Pitch : y�� �߽����� ȸ��, Yaw : z�� �߽����� ȸ�� 
    private float cinemachineTargetPitch;
    private float cinemachineTargetYaw;

    private void Start()
    {
        input = GameObject.Find("GameManager").GetComponent<InputManager>();
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        cinemachineTargetYaw = cinemachineCameraTarget.transform.rotation.eulerAngles.y; 

        AssignAnimationIDs();

        jumpTimeoutDelta = jumpTimeout;
        fallTimeoutDelta = fallTimeout;
    }

    private void Update()
    {
        Jump();
        CheckGround();
        Move();
        CameraRotation();   
    }

    private void FixedUpdate()
    {
    }

    private void AssignAnimationIDs()
    {
        animIDSpeed = Animator.StringToHash("speed");
        animIDJump = Animator.StringToHash("isJump");
        animIDGround = Animator.StringToHash("isGround");
        animIDFall = Animator.StringToHash("isFall");
    }

    private void Move()
    {
        // �̵��ϴµ�, ���� �ʿ�����? �ӷ�, �÷��̾� ����, ī�޶� ����

        // Lerp�� ����ؼ� �ε巴�� �̵��� �ǵ�, �׷����� ��ǥ�� �ϴ� �ӵ��� �ʿ��ϴ�.
        float targetSpeed = input.press_Shift ? runSpeed : walkSpeed;

        if (input.move_X == 0 && input.move_Z == 0) targetSpeed = 0f;

        // �ǽð����� �Է¹��� move�� �ӷ�
        float currentHorizontalSpeed = new Vector3(input.move_X, 0, input.move_Z).magnitude;

        // ��ǥ�� �ϴ� �ӵ����� +- 0.1��ŭ �������� �ӵ��� �ε巴�� �̾�������. �� ����ó��? ������, �����Ҷ� ����ó���ϴ°�ó��
        float speedOffset = 0.1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, speedChangeRate * Time.deltaTime);

            // round speed to 3 decimal places
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else
        {
            speed = targetSpeed;
        }

        animBlend = Mathf.Lerp(animBlend, targetSpeed, speedChangeRate * Time.deltaTime);
        if (animBlend < 0.1f) animBlend = 0.0f;

        // �Է¹��� Ű�� ����
        Vector3 inputDir = new Vector3(input.move_X, 0, input.move_Z).normalized;

        if(input.move_X != 0 || input.move_Z != 0)
        {
            targetRotation = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + mainCamera.transform.rotation.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmoothTime);

            // ī�޶� ��ġ�� ��ǲ���� ���� ������ �ٶ󺻴�
            transform.rotation = Quaternion.Euler(0, rotation, 0);
        }

        // ȸ���� ���⺤��
        Vector3 targetDirection = Quaternion.Euler(0, targetRotation, 0) * Vector3.forward; 

        controller.Move(targetDirection.normalized * speed * Time.deltaTime + new Vector3(0, verticalVelocity, 0) * Time.deltaTime);

        anim.SetFloat(animIDSpeed, animBlend);

    }

    private void Jump()
    {
        if(isGrounded)
        {
            anim.SetBool(animIDJump, false);
            anim.SetBool(animIDFall, false);

            // ���� �پ����� �� ������ -�Ǵ°� ����
            if(verticalVelocity < 0.0f)
            {
                verticalVelocity = -2.0f;
            }

            // ����
            if(input.press_Space && jumpTimeoutDelta <= 0f)
            {
                // ���� ���̿� �����ϱ� ���� ���� �ӵ��� ���
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

                anim.SetBool(animIDJump, true);
            }

            // ���� timeout
            if (jumpTimeoutDelta >= 0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else // ���߿� ���ִµ���
        {
            // ���� Ÿ�̸� ����
            jumpTimeoutDelta = jumpTimeout;

            // ���� timeout
            if(fallTimeoutDelta >= 0f)
            {
                fallTimeoutDelta -= Time.deltaTime;
            }
            else // ���ϸ�� ���
            {
                anim.SetBool(animIDFall, true);
            }

            // ���߿� �ִµ��� ���� ����
            input.press_Space = false;
        }
        
        // �ִ� �ӷ±��� ���ӵ� ����
        if(verticalVelocity < terminalVelocity)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

    }

    private void CheckGround()
    {
        // ������ �Ǻ��ϱ� ���� sphere ��ġ ����
        Vector3 spherePos = new Vector3(transform.position.x, transform.position.y - groundOffset, transform.position.z);
        isGrounded = Physics.CheckSphere(spherePos, groundRadius, groundLayers, QueryTriggerInteraction.Ignore);

        anim.SetBool(animIDGround, isGrounded);
        DebugDrawSphere(spherePos, groundRadius, isGrounded ? Color.green : Color.red);
    
    }

    void DebugDrawSphere(Vector3 center, float radius, Color color)
    {
        // Draw a wireframe sphere in the scene view
        Debug.DrawRay(center + Vector3.up * radius, Vector3.right * radius, color);
        Debug.DrawRay(center + Vector3.up * radius, Vector3.left * radius, color);
        Debug.DrawRay(center + Vector3.up * radius, Vector3.forward * radius, color);
        Debug.DrawRay(center + Vector3.up * radius, Vector3.back * radius, color);

        Debug.DrawRay(center - Vector3.up * radius, Vector3.right * radius, color);
        Debug.DrawRay(center - Vector3.up * radius, Vector3.left * radius, color);
        Debug.DrawRay(center - Vector3.up * radius, Vector3.forward * radius, color);
        Debug.DrawRay(center - Vector3.up * radius, Vector3.back * radius, color);

        Debug.DrawRay(center + Vector3.right * radius, Vector3.forward * radius, color);
        Debug.DrawRay(center + Vector3.right * radius, Vector3.back * radius, color);

        Debug.DrawRay(center - Vector3.right * radius, Vector3.forward * radius, color);
        Debug.DrawRay(center - Vector3.right * radius, Vector3.back * radius, color);
    }

    private void CameraRotation()
    {
        Vector2 look = new Vector2(input.mouse_Rotate_Y, input.mouse_Rotate_X);

        if(look.sqrMagnitude >= threshold)
        {
            cinemachineTargetYaw += input.mouse_Rotate_Y * cameraRotateSpeed;
            cinemachineTargetPitch += -input.mouse_Rotate_X * cameraRotateSpeed;
        }

        // clamp our rotations so our values are limited 360 degrees
        cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
        cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);

        // �ó׸ӽ��� �� ��ǥ�� ���� ��
        cinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch, cinemachineTargetYaw, 0f) ;
    }

    private float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
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
