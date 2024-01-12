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
    
    // 입력받은 속력 -> 목표로 하는 속력으로 변환하는 비율 (Lerp)
    public float speedChangeRate = 10f;

    // x축, z축 입력에 따른 값을 normalized할 것임. 정규화해서 방향벡터만 가지도록
    // 이 speed값을 구하는 것은 x,z 입력받은 값을 magnitude하고 normalized하고 속력을 곱한다...
    private float speed;
    // 애니메이션 블렌드에 넣을 값
    private float animBlend;

    // 목표로 하는 회전값
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

    // 점프를 하기 위해 필요한 시간, 착지하자마자 점프는 어색하니까. 0f가 됐을때 점프가 가능하도록
    // 착지모션에 도달하기 위해 필요한 시간, 아래계단으로 내려갈때 바로 fall모션 나오면 어색
    private float jumpTimeoutDelta;// 점프 간격의 남은 시간을 나타내는 변수
    private float fallTimeoutDelta;

    // controller.Move할때 == 실제로 이동하는 메서드 곱해줄 속력.
    public float horizontalVelocity;
    public float verticalVelocity;

    private float terminalVelocity = 53f; // 중력에 의한 최대 속력

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

    // Roll : x축 중심으로 회전, Pitch : y축 중심으로 회전, Yaw : z축 중심으로 회전 
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
        // 이동하는데, 뭐가 필요하지? 속력, 플레이어 방향, 카메라 방향

        // Lerp를 사용해서 부드럽게 이동할 건데, 그럴러면 목표로 하는 속도가 필요하다.
        float targetSpeed = input.press_Shift ? runSpeed : walkSpeed;

        if (input.move_X == 0 && input.move_Z == 0) targetSpeed = 0f;

        // 실시간으로 입력받은 move의 속력
        float currentHorizontalSpeed = new Vector3(input.move_X, 0, input.move_Z).magnitude;

        // 목표로 하는 속도에서 +- 0.1만큼 범위에서 속도가 부드럽게 이어지도록. 즉 마감처리? 끝날때, 시작할때 마감처리하는것처럼
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

        // 입력받은 키의 방향
        Vector3 inputDir = new Vector3(input.move_X, 0, input.move_Z).normalized;

        if(input.move_X != 0 || input.move_Z != 0)
        {
            targetRotation = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + mainCamera.transform.rotation.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmoothTime);

            // 카메라 위치와 인풋값에 따라 정면을 바라본다
            transform.rotation = Quaternion.Euler(0, rotation, 0);
        }

        // 회전된 방향벡터
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

            // 땅에 붙어있을 때 무한히 -되는것 방지
            if(verticalVelocity < 0.0f)
            {
                verticalVelocity = -2.0f;
            }

            // 점프
            if(input.press_Space && jumpTimeoutDelta <= 0f)
            {
                // 점프 높이에 도달하기 위한 수직 속도를 계산
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

                anim.SetBool(animIDJump, true);
            }

            // 점프 timeout
            if (jumpTimeoutDelta >= 0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else // 공중에 떠있는동안
        {
            // 점프 타이머 리셋
            jumpTimeoutDelta = jumpTimeout;

            // 낙하 timeout
            if(fallTimeoutDelta >= 0f)
            {
                fallTimeoutDelta -= Time.deltaTime;
            }
            else // 낙하모션 재생
            {
                anim.SetBool(animIDFall, true);
            }

            // 공중에 있는동안 점프 금지
            input.press_Space = false;
        }
        
        // 최대 속력까지 가속도 더함
        if(verticalVelocity < terminalVelocity)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

    }

    private void CheckGround()
    {
        // 땅인지 판별하기 위한 sphere 위치 설정
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

        // 시네머신은 이 목표를 따라갈 것
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
