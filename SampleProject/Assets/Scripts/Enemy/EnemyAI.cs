using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyAction
{
    Idle,
    Patrol,
    Trace,
    Attack,
    Die
}

public class EnemyAI : MonoBehaviour
{
    public EnemyAction action = EnemyAction.Patrol;

    //적 캐릭터의 위치를 저장할 변수
    private Transform _enemyTrans;
    //적 움직임을 저장하는 클래스
    private EnemyMovement _enemyMovement;
    //적 움직임을 저장하는 클래스
    private EnemyState _enemyState;
    //적 캐릭터의 애니메이션 클래스
    private Animator _enemyAni;

    //공격 사정거리
    public float attackDist = 1.0f;
    //추적 사정거리
    public float traceDist = 5.0f;
    //적 캐릭터의 사망 여부
    public bool isDie = false;
    //캐릭터 공격 애니매이션 끝난 여부를 가져옵니다
    public bool isAniAttack;

    //타겟 캐릭터
    public GameObject targetCharacter;
    //캐릭터들을 저장할 변수
    private GameObject[] Characters = new GameObject[3];

    private readonly int hashMove = Animator.StringToHash("IsMove");
    private readonly int hashAttack = Animator.StringToHash("IsAttack");

    private void Awake()
    {
        _enemyTrans = GetComponent<Transform>();
        _enemyMovement = GetComponent<EnemyMovement>();
        _enemyAni = GetComponentInChildren<Animator>();
        _enemyState = GetComponent<EnemyState>();

        var t = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < t.Length; i++)
        {
            Characters[i] = t[i].gameObject;
        }

        targetCharacter = null;
    }

    private void OnEnable()
    {
        //초기화할때 사용합니다
        action = EnemyAction.Idle;
        isDie = false;

        targetCharacter = null;
        _enemyAni.SetBool(hashMove, false);
        _enemyAni.SetBool(hashAttack, false);

        StartCoroutine("CheckState");
        StartCoroutine("Action");
    }

    IEnumerator CheckState()
    {
        //타겟 캐릭터와 현재 상태를 정의해주는 코르틴 함수이다.
        float dist;

        //사망하기 전까지 무한 루프
        while (!isDie)
        {
            isAniAttack = _enemyAni.GetBool(hashAttack);

            if (action == EnemyAction.Die) yield break;

            SearchMinDistanceCharacter();

            dist = Vector2.Distance(targetCharacter.transform.position, _enemyTrans.position);

            //공격 사정거리 이내인 경우
            if (dist <= attackDist)
            { 
                action = EnemyAction.Attack;
            }
            else if (dist <= traceDist)
            {
                action = EnemyAction.Trace;
            }
            else
            {
                //순찰과 대기를 확률적으로 선택합니다
                int ran = Random.Range(0, 100);

                if (ran > 50)
                {
                    action = EnemyAction.Patrol;
                }
                else
                {
                    action = EnemyAction.Patrol;
                }
            }


            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator Action()
    {
        //액션을 정의하는 코루틴함수이다.
        while (!isDie)
        {
            yield return new WaitForSeconds(0.3f);

            switch (action)
            {
                case EnemyAction.Patrol:
                    _enemyMovement.Patrolling = true;
                    _enemyMovement.Traceing = false;
                    _enemyAni.SetBool(hashMove, true);
                    _enemyAni.SetBool(hashAttack, false);
                    break;
                case EnemyAction.Trace:
                    _enemyMovement.Patrolling = false;
                    _enemyMovement.Traceing = true;
                    _enemyMovement.TraceTarget = targetCharacter.transform.position;
                    _enemyAni.SetBool(hashMove, true);
                    _enemyAni.SetBool(hashAttack, false);
                    break;
                case EnemyAction.Attack:
                    _enemyMovement.Stop();
                    _enemyAni.SetBool(hashMove, false);
                    _enemyAni.SetBool(hashAttack, true);

                    if (!isAniAttack)
                    {
                        _enemyMovement.IsAttack = true;
                    }
                    break;
                case EnemyAction.Idle:
                    _enemyMovement.Stop();
                    _enemyAni.SetBool(hashMove, false);
                    _enemyAni.SetBool(hashAttack, false);
                    break;
                case EnemyAction.Die:
                    isDie = true;
                    _enemyMovement.Stop();
                    _enemyAni.SetBool(hashMove, false);
                    _enemyAni.SetBool(hashAttack, false);
                    this.gameObject.SetActive(false);
                    break;

            }
        }
    }

    void SearchMinDistanceCharacter()
    {
        int num = -1;
        float minDist = 0.0f, dist;

        //가장 가까운 캐릭터를 검색한다
        for (int i = 0; i < Characters.Length; i++)
        {
            if (Characters[i].activeSelf != true) continue;

            dist = Vector2.Distance(Characters[i].transform.position, _enemyTrans.position);

            if (num == -1 || dist < minDist)
            {
                minDist = dist;
                num = i;
            }
        }

        //검색 결과를 저장한다
        if (num == -1)
        {
            targetCharacter = null;
        }
        else
        {
            targetCharacter = Characters[num];
        }

    }
}
