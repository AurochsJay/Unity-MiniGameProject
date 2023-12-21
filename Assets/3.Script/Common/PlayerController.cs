using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputManager input;
    [SerializeField] private LobbyManager lobby;

    public float speed = 5f;
    private Vector3 rotation;

    private void Start()
    {
        rotation = transform.rotation.eulerAngles;
    }

    private void Update()
    {
        Move();
        Rotate();
    }

    private void Move()
    {
        Vector3 moveDir = new Vector3(input.move_X, 0f, input.move_Z);
        transform.Translate(moveDir * speed * Time.deltaTime);
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

            if (input.isG)
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
