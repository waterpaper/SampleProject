using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyState : MonoBehaviour
{
    public EnemyAI enemyAI;

    //플레이어의 초기값을 설정해줍니다.
    public float hp = 100.0f;
    public float strikingPower = 10.0f;
    public float attackSpeed = 1.0f;
    public float defensivePower = 0.0f;
    public float patrolMoveSpeed = 1.5f;
    public float traceMoveSpeed = 2.0f;
    
    //추가값의 최대값을 설정해줍니다.
    public float maxHp = 100.0f;
    public float maxStrikingPower = 10000.0f;
    public float maxDefensivePower = 10000.0f;

    //화면에 체력바를 보여주기 위한 UI이다
    public GameObject healthBarBackground;
    public Image healthBarFilled;

    private void OnEnable()
    {
        hp = maxHp;
        healthBarFilled.fillAmount = 1;

        enemyAI = GetComponent<EnemyAI>();

        StartCoroutine("UIBarUpdate");
    }

    IEnumerator UIBarUpdate()
    {
        while (hp > 0)
        {
            healthBarFilled.fillAmount = hp / maxHp;

            yield return new WaitForSeconds(0.2f);
        }
    }

    public void Damage(float value)
    {
        hp -= value;

        if(hp<=0)
        {
            enemyAI.action = EnemyAction.Die;
        }
    }
}
