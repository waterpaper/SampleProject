using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoolOperation : StateMachineBehaviour
{
    //관리할 bool 네임입니다.
    public string boolName;

    //행동이 끝나고 바꿔줄 상태의 정보를 저장합니다.
    public bool status;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(boolName, !status);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(boolName, !status);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(boolName, status);
    }
}
