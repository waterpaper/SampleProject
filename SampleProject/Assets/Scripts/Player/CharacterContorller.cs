using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterContorller : MonoBehaviour
{
    private CharacterState _state;
    private Rigidbody2D _rigidbody;
    private Animator _ani;
    private bool _isMove;
    private Vector3 _moveDir;
    private Direction _direction;

    //목표타겟에 접근시 접근거리입니다.
    [SerializeField]
    private float _minDistance;

    //공격렉트입니다
    public GameObject attackRect;
    //플레이어와 움직일시 타겟 목표입니다.
    public GameObject targetCharacterPosition;
    //몬스터 타겟 목표입니다
    public GameObject targetEnemy;
    public Vector2 targetPosition;

    public bool _isTraceing;
    public bool IsTraceing
    {
        get { return _isTraceing; }
        set
        {
            _isTraceing = value;
        }
    }

    public bool _isAttack;
    public bool IsAttack
    {
        get { return _isAttack; }
        set
        {
            _isAttack = value;
        }
    }
    public bool _isAniAttack;


    //공격 사정거리
    public float attackDist = 2.0f;
    //추적 사정거리
    public float traceDist = 7.0f;
    //플레이어와 거리 사정거리
    public float playerDist = 15.0f;

    private void Start()
    {
        //해당 컴포넌트를 불러옵니다.
        _state = GetComponent<CharacterState>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _ani = GetComponentInChildren<Animator>();

        targetEnemy = null;
        _isMove = false;
        _isTraceing = false;
        _isAttack = false;
        _minDistance = 3.0f;
        _direction = Direction.Right;
    }

    private void FixedUpdate()
    {
        //이동 상태를 받아옵니다.
        _isMove = _ani.GetBool("IsMove");
        _isAniAttack = _ani.GetBool("IsAttack");


        if (!_isTraceing&&!_isAttack)
        {
            // 추적 상태가 아닐시에 접근합니다.
            CheckTrace();
        }


        if(_isTraceing)
        {
            TraceMove();
        }
        else if(_isAttack&&!_isAniAttack)
        {
            //공격상태이나 지금 공격애니매이션이 아닐 때
            StartCoroutine("Attack");
        }
        else if(!_isAniAttack&&!_isAttack)
        {
            //공격상태, 공격애니메이션이 아닐때
            Move();
        }

        _ani.SetFloat("Speed", Mathf.Abs(_rigidbody.velocity.x + _rigidbody.velocity.y));
    }

    private void Move()
    {
        //목표 포지션과 거리 차이를 확인합니다.
        if (CheckDistanceMove())
        {
            //움직임을 처리합니다.
            Vector2 moveDir = MoveDirction() * _state.moveSpeed;
            _rigidbody.velocity = moveDir;
            characterRotation(moveDir);
        }
        else
        {
            if (_isMove)
            {
                _rigidbody.velocity = new Vector2(0.0f, 0.0f);
            }
        }
    }

    private void TraceMove()
    {
        //추적하는 움직임을 처리합니다.
        Vector2 moveDir;
        float distance;

        //목표 적과의 거리를 구합니다.
        distance = Vector2.Distance(transform.position, targetEnemy.transform.position);

        if (distance < attackDist)
        {
            //공격범위 안에 있는 경우
            _rigidbody.velocity = new Vector2(0.0f, 0.0f);
            _isTraceing = false;
            _isAttack = true;
        }
        else if(distance < playerDist)
        {
            //플레이어와 멀리 떨어지지 않은 경우
            moveDir = ((Vector2)targetEnemy.transform.position - (Vector2)transform.position).normalized;
            _rigidbody.velocity = moveDir * _state.moveSpeed;
            characterRotation(moveDir);
        }
        else
        {
            //플레이어와 멀리 떨어진 경우 공격을 포기하고 플레이어로 움직입니다.
            _isTraceing = false;
            targetEnemy = null;
        }

    }


    bool CheckDistanceMove()
    {
        float distance;

        //목표포지션과 현재 위치의 차이를 비교해 이동여부를 판단합니다
        distance = Vector2.Distance(transform.position, targetCharacterPosition.transform.position);

        if (distance > _minDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void CheckTrace()
    {
        //몬스터 추적 유무를 판단합니다.
        int num = -1;
        float min = 0.0f;
        float distance = 0.0f;
        
        //오브젝트 풀에 있는 몬스터를 불러옵니다.
        var t = GameManager.instance.enemyPool;

        for (int i = 0; i < t.Count; i++)
        {
            if (t[i].activeSelf == false) continue;

            distance = Vector2.Distance(transform.position, t[i].transform.position);

            if (distance <= traceDist)
            {
                if (num == -1 || distance <= min)
                {
                    num = i;
                    min = distance;
                }
            }
        }


        if (num != -1)
        {
            targetEnemy = t[num];
            _isTraceing = true;
        }
        else
        {
            targetEnemy = null;
            _isTraceing = false;
        }
    }

    Vector2 MoveDirction()
    {
        Vector2 dir;

        //움직여야하는 방향벡터를 계산합니다.
        dir = ((Vector2)targetCharacterPosition.transform.position - (Vector2)transform.position).normalized;


        return dir;
    }

    void characterRotation(Vector2 moveDir)
    {
        //캐릭터의 좌우를 돌려줍니다.
        if (moveDir.x > 0.0f && _direction == Direction.Left)
        {
            transform.GetChild(0).transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            _direction = Direction.Right;
        }
        else if (moveDir.x < 0.0f && _direction == Direction.Right)
        {
            transform.GetChild(0).transform.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            _direction = Direction.Left;
        }
    }

    IEnumerator Attack()
    {
        // 공격렉트를 만들어 공격을 처리합니다.
        _rigidbody.velocity = new Vector2(0.0f, 0.0f);
        _ani.SetBool("IsAttack",true);

        attackRect.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        attackRect.SetActive(false);

        //적이 죽은 경우 재설정합니다.
        if (targetEnemy.activeSelf == false)
        {
            targetEnemy = null;
            _isAttack = false;
        }
        else
        {
            float distance = Vector2.Distance(transform.position, targetEnemy.transform.position);

            if (distance > attackDist)
            {
                _isAttack = false;
                _isTraceing = true;
            }
        }

    }
}
