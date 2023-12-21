using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisManager : MonoBehaviour
{
    [SerializeField] private Grid2D grid;
    public bool isFieldTetromino = true;

    private void Start()
    {
        grid = GameObject.Find("GameManager").GetComponent<Grid2D>();
    }

    private void Update()
    {
        
    }
}
