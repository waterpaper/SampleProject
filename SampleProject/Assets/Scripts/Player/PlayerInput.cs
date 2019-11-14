using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    //위, 아래
    public string verticalMoveName = "Vertical";
    //좌, 우
    public string horizontalMoveName = "Horizontal";
    //공격
    public string attackName = "Fire1";
    //점프
    public string jumpName = "Jump";

    //플레이어의 입력값을 저장한다.
    public float Vertical { get; private set; }
    public float Horizontal { get; private set; }
    public bool IsJump { get; private set; }
    public bool IsAttack { get; private set; }
    
    void Update()
    {
        Vertical = Input.GetAxis(verticalMoveName);
        Horizontal = Input.GetAxis(horizontalMoveName);
        IsJump = Input.GetButton(jumpName);
        IsAttack = Input.GetButtonDown(attackName);
    }
}
