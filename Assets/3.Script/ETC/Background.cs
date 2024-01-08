using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] private Grid2D grid;
    //[SerializeField] private GameObject empty_Prefab;
    [SerializeField] private GameObject cube_Prefab;
    [SerializeField] private Material[] colors;

    private int column = 35;
    private int width = 65;

    private List<List<GameObject>> columns = new List<List<GameObject>>();

    //private Vector3 offset = new Vector3(-21, -10, -4.5f);
    private Vector3 offset;

    private void Start()
    {
        CheckScene();
        grid.SetGrid(column, width);
        GenerateCubes();
        StartCoroutine(ChangeColor());
    }

    private void CheckScene()
    {
        switch (GameManager.instance.presentScene)
        {
            case Scene.Tetris:
                offset = new Vector3(-24, -10, -4.5f);
                break;
            case Scene.Snake:
                offset = new Vector3(-21, -1, -6);
                break;
        }

    }

    private void GenerateCubes()
    {
        for(int j = 0; j < column; j++)
        {
            //GameObject Column = Instantiate(empty_Prefab, Vector3.zero, Quaternion.identity);
            GameObject emptyObject = new GameObject($"Column[{j}]");
            List<GameObject> column = new List<GameObject>();

            for(int i = 0; i < width; i++)
            {
                Vector3 cubePos = new Vector3(i, 0, j);
                GameObject cube = Instantiate(cube_Prefab, cubePos+offset, Quaternion.identity);
                cube.transform.SetParent(emptyObject.transform);
                column.Add(cube);
            }

            columns.Add(column);
        }
    }

    private IEnumerator ChangeColor()
    {
        while(true)
        {
            for (int j = 0; j < column; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    //grid.array[j, i] = color_Number;
                    int rand = Random.Range(0, 100);
                    int color_Number;

                    if (0 <= rand && rand < 10)
                    {
                        color_Number = 0;
                    }
                    else if (10 <= rand && rand < 30)
                    {
                        color_Number = 1;
                    }
                    else if (30 <= rand && rand < 60)
                    {
                        color_Number = 2;
                    }
                    else
                    {
                        color_Number = 3;
                    }

                    columns[j][i].GetComponent<Renderer>().material = colors[color_Number];
                }
            }

            yield return new WaitForSeconds(2f);
        }

        
        yield return null;
    }

}
