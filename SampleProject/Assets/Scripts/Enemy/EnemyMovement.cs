using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyMovement : MonoBehaviour
{
    //순찰지점들을 저장하기 위한 변수
    public List<Transform> wayPoints;
    //다음 순찰 지점의 배열의 index
    public int nextIdx;
    //현재 이동 속도를 가지고 있는 변수
    public float nowSpeed;
    //현재 이동 위치를 가지고 있는 변수
    public Vector2 destPos;
    //현재 방향을 나타내는 변수
    public Direction dirction = Direction.Left;
    //적 공격 렉트를 나타냅니다
    public GameObject enemyAttackRect;


    //적들의 정보를 가지고 있는 클래스
    private EnemyState _enemyState;
    //적의 리지드바디
    private Rigidbody2D _enemyRigid;

    //공격 상태를 판단하는 변수
    private bool _isAttack = false;
    public bool IsAttack
    {
        get { return _isAttack; }
        set
        {
            _isAttack = value;
        }
    }
    //순찰 상태를 판단하는 변수
    private bool _patrolling;
    public bool Patrolling
    {
        get { return _patrolling; }
        set
        {
            _patrolling = value;
        }
    }
    //추적을 판단하는 변수
    private bool _traceing;
    public bool Traceing
    {
        get { return _traceing; }
        set
        {
            _traceing = value;
        }
    }
    private Vector2 _traceTarget;
    public Vector2 TraceTarget
    {
        get { return _traceTarget; }
        set
        {
            _traceTarget = value;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        _enemyState = GetComponent<EnemyState>();
        _enemyRigid = GetComponent<Rigidbody2D>();

        var group = GameObject.Find("WayPointGroup");

        if (group != null)
        {
            group.GetComponentsInChildren<Transform>(wayPoints);
        }
        wayPoints.RemoveAt(0);

        nextIdx = Random.Range(0, wayPoints.Count);
    }

    private void OnEnable()
    {
        _isAttack = false;
        _patrolling = true;
        _traceing = false;

    }


    // Update is called once per frame
    private void FixedUpdate()
    {
        if (_patrolling)
        {
            MoveWayPoint();
        }
        else if (_traceing)
        {
            MoveTarceTarget(TraceTarget);
        }
        else if (_isAttack)
        {
            StartCoroutine("Attack");
        }

    }

    void MoveWayPoint()
    {
        nowSpeed = _enemyState.patrolMoveSpeed;
        destPos = wayPoints[nextIdx].position;
        StartCoroutine("Move");
    }

    void MoveTarceTarget(Vector2 target)
    {
        nowSpeed = _enemyState.traceMoveSpeed;
        destPos = target;
        StartCoroutine("Move");
    }

    void NextPatrol()
    {
        //다음 패트롤 위치로 가야되는지 판단합니다.
        if (Vector2.Distance(wayPoints[nextIdx].position, transform.position) <= 1.0f)
        {
            nextIdx = Random.Range(0, wayPoints.Count);
        }
    }

    void EnemyRotation(Vector2 moveDir)
    {
        if (moveDir.x > 0.0f && dirction == Direction.Left)
        {
            transform.GetChild(0).transform.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            dirction = Direction.Right;
        }
        else if (moveDir.x < 0.0f && dirction == Direction.Right)
        {
            transform.GetChild(0).transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            dirction = Direction.Left;
        }
    }

    public void Stop()
    {
        _enemyRigid.velocity = Vector2.zero;
        _patrolling = false;
        _traceing = false;
    }

    IEnumerator Move()
    {
        Vector2 dir = (destPos - (Vector2)transform.position).normalized;
        Vector2 moveDir = dir * nowSpeed;
        _enemyRigid.velocity = moveDir;

        //순찰 포인트 근처에 도착햇는지 판단한다.
        NextPatrol();

        //방향 회전을 처리한다
        EnemyRotation(moveDir);

        yield return null;
    }
    IEnumerator Attack()
    {
        _isAttack = false;
        enemyAttackRect.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        enemyAttackRect.SetActive(false);
    }
}
