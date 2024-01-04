using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SnakeManager : MonoBehaviour
{
    [SerializeField] public Grid2D grid;
    [SerializeField] private GameObject target;

    private readonly int height = 22;
    private readonly int width = 22;

    private Vector3 startPos = new Vector3(5, 0, 10);

    private void Start()
    {
        grid.SetGrid(height, width);
        SetGridToWall();
        CreateTarget();
    }

    // ±×¸®µå¿¡ Wall ¼³Á¤, grid.array[y,x] == 0:ºóÄ­, 1:SnakeÄ­, 2:TargetÄ­, 3:WallÄ­
    private void SetGridToWall()
    {
        for(int j=0; j<height; j++)
        {
            for(int i=0; i<width; i++)
            {
                if(i == 0 || i == width-1 || j == 0 || j == height-1)
                {
                    grid.array[j, i] = 3;
                }
            }
        }
    }

    

    // Target ·£´ý »ý¼º <- ÃÊ±â ¹× Snake°¡ TargetÀ» ¸ÔÀ¸¸é ½ÇÇà
    public void CreateTarget()
    {
        List<Vector2> ableNumber = new List<Vector2>();
            
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                if (grid.array[j, i] == 1 || grid.array[j, i] == 3)
                {
                    continue;
                }
                else
                {
                    ableNumber.Add(new Vector2(i, j));
                }
            }
        }

        int rand = Random.Range(0, ableNumber.Count);

        grid.array[(int)ableNumber[rand].y, (int)ableNumber[rand].x] = 2;
        target.transform.position = new Vector3((int)ableNumber[rand].x, 0, (int)ableNumber[rand].y);
    }
}
