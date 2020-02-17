using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PlayerAction
{
    playerAcrion_Idle,
    playerAcrion_Move,
    playerAcrion_Attack,
    playerAcrion_End
}

public class PlayerController : MonoBehaviour
{
    //컨트롤을 입력받는 스크립트를 불러옵니다.
    private PlayerInput _playerInput;
    //플레이어의 정보를 저장하는 스크립트를 불러옵니다
    private CharacterState _playerState;
    //플레이어의 애니메이션을 불러옵니다
    private Animator _playerAni;
    public AnimationEvent _event;
    //플레이어 리지드바디를 불러옵니다
    private Rigidbody2D _playerRigid;
    
    //현재 플레이어 상태 입니다
    private PlayerAction action;
    //현재 플레이어 방향입니다.
    private Direction direction;
    //이전 공격시간을 저장합니다
    private float attackTime = 1.0f;

    //화살 공격 범위를 설정합니다
    public float attackRange;
    //화살 공격 속도를 설정합니다
    public float attackSpeed = 1.0f;
    //적 타겟을 자동으로 설정해줍니다.
    public GameObject targetEnemy;

    //현재 공격 모션 상태인지 판단합니다.
    private bool isAttacking;
    public bool IsAttacking
    {
        get { return isAttacking; }
        set
        {
            isAttacking = value;
        }
    }

    void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerState = GetComponent<CharacterState>();
        _playerAni = GetComponentInChildren<Animator>();
        _playerRigid = GetComponent<Rigidbody2D>();

        isAttacking = false;

        direction = Direction.Right;
    }

    void FixedUpdate()
    {
        //매 업데이트마다 움직임을 처리합니다.
        updateInput();
        updateState();
    }
    private void updateInput()
    {
        isAttacking = _playerAni.GetBool("IsAttack");

        //입력된 상태로 상태를 업데이트해줍니다.
        if (_playerInput.Horizontal != 0.0f || _playerInput.Vertical != 0.0f)
        {

            action = PlayerAction.playerAcrion_Move;
            isAttacking = false;
        }
        else
        {
            //타겟 적을 확인합니다.
            CheckTargetEnemy();

            if (targetEnemy != null)
            {
                action = PlayerAction.playerAcrion_Attack;
            }
            else
            {
                action = PlayerAction.playerAcrion_Idle;
            }
        }

        //속도를 애니메이션에 적용합니다.
        _playerAni.SetFloat("Speed", Mathf.Abs(_playerRigid.velocity.x + _playerRigid.velocity.y));
    }

    private void updateState()
    {
        switch (action)
        {
            case PlayerAction.playerAcrion_Idle:
                _playerRigid.velocity = Vector2.zero;
                break;
            case PlayerAction.playerAcrion_Move:
                Move();
                break;
            case PlayerAction.playerAcrion_Attack:
                _playerRigid.velocity = Vector2.zero;
                StartCoroutine("Attack");
                break;
            case PlayerAction.playerAcrion_End:
                break;
        }
    }

    private void Move()
    {
        //입력 값에 따른 이동방향을 설정합니다
        Vector2 v = _playerInput.Vertical * Vector3.up;
        Vector2 h = _playerInput.Horizontal * Vector3.right;

        //방향을 설정하고 속도를 곱해줍니다.
        Vector2 moveDir = ((v + h).normalized) * _playerState.moveSpeed;

        //바라보는 방향을 조절합니다.
        AniDirection(_playerInput.Horizontal);

        //속력을 적용합니다.
        _playerRigid.velocity = moveDir;
    }

    IEnumerator Attack()
    {
        if (!isAttacking)
        {
            _playerAni.SetBool("IsAttack", true);
            _playerAni.SetFloat("AttackSpeed", attackTime);

            yield return new WaitForSeconds(1.0f);

            if (targetEnemy!=null && action == PlayerAction.playerAcrion_Attack)
            {
                //화살을 발사하면서 발사방향이 어느 방향인지 판단합니다.
                if (GameManager.instance.NewCrateArrow_Right(new Vector2(transform.position.x, transform.position.y+1.0f), targetEnemy.transform.position))
                {
                    AniDirection(1.0f);
                }
                else
                {
                    AniDirection(-1.0f);
                }
            }
        }

        yield return null;
    }

    private void AniDirection(float horizontal)
    {
        //애니매이션의 방향을 돌려줍니다.
        if (horizontal > 0)
        {
            if (direction == Direction.Left)
            {
                //_playerAni.gameObject.transform.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                _playerAni.gameObject.transform.localScale = new Vector3(2, 2, 0);
                direction = Direction.Right;
            }
        }
        else if (horizontal < 0)
        {
            if (direction == Direction.Right)
            {
                //_playerAni.gameObject.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                _playerAni.gameObject.transform.localScale = new Vector3(-2, 2, 0);
                direction = Direction.Left;
            }
        }
    }

    void CheckTargetEnemy()
    {
        //적 타겟을 판단합니다.
        int num = -1;
        float min = 0.0f;
        float distance = 0.0f;

        //오브젝트 풀에 있는 몬스터를 불러옵니다.
        var t = GameManager.instance.enemyPool;

        for (int i = 0; i < t.Count; i++)
        {
            if (t[i].activeSelf == false) continue;

            distance = Vector2.Distance(transform.position, t[i].transform.position);

            if (distance <= attackRange)
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
        }
        else
        {
            targetEnemy = null;
        }
    }
}
