using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JJumpManager : MonoBehaviour
{
    // �ش� ��ȣ�� ������ ����ߴ���. ���θ��� ������ �ѹ��� ����
    public bool[] usedLineNumber = new bool[5];  

    private void Start()
    {
        InitiateSetting();
    }

    private void InitiateSetting()
    {
        for(int i =0; i<usedLineNumber.Length; i++)
        {
            usedLineNumber[i] = false;
        }
    }

}
