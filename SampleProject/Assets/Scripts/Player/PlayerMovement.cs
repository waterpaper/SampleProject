using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;


public class PlayerMovement : MonoBehaviour
{
    //컨트롤을 입력받는 스크립트를 불러옵니다.
    private PlayerInput _playerInput;
    //플레이어의 정보를 저장하는 스크립트를 불러옵니다
    private CharacterState _playerState;
    //플레이어의 애니메이션을 불러옵니다
    private SkeletonAnimation _playerAni;
    private Spine.AnimationState animationState;
    //플레이어 리지드바디를 불러옵니다
    private Rigidbody2D _playerRigid;


    //공격렉트를 가져옵니다
    public GameObject attackRect;
    //현재 멈춘 상태인지 판단합니다
    public bool _isIdle;
    //현재 움직이는 상태인지 판단합니다.
    public bool _isMove;
    //현재 공격 모션 상태인지 판단합니다.
    public bool _isAttacking;

    private Direction direction;

    // Start is called before the first frame update
    void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerState = GetComponent<CharacterState>();
        _playerAni = GetComponentInChildren<SkeletonAnimation>();
        _playerRigid = GetComponent<Rigidbody2D>();
        animationState = _playerAni.AnimationState;

        _playerAni.AnimationState.Start += OnSpineAnimationStart;
        _playerAni.AnimationState.Complete += OnSpineAnimationComplete;

        _isMove = false;
        _isAttacking = false;
        _isIdle = true;

        direction = Direction.Right;
        _playerAni.AnimationState.SetAnimation(0, "01_Idle", true);
    }

    private void OnSpineAnimationStart(Spine.TrackEntry entry)
    {
        //스파인 애니메이션 사용시 애니메이션이 시작되면 실행되는 함수입니다
        if (entry.Animation.Name == "01_Idle")
        {
            _isIdle = true;
            _isMove = false;
        }
        if (entry.Animation.Name == "03_run")
        {
            _isIdle = false;
            _isMove = true;
        }
    }

    private void OnSpineAnimationComplete(Spine.TrackEntry entry)
    {
        //스파인 애니메이션 사용시 애니메이션이 완료되면 실행되는 함수입니다.
        if (entry.Animation.Name=="05_attack")
        {
            _isAttacking = false;
            _isIdle = true;
            _playerAni.AnimationState.SetAnimation(0, "01_Idle", true);
            attackRect.SetActive(false);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        updateMovement();
    }

    private void updateMovement()
    {
        if (!_isAttacking)
        {
            //공격상태가 아닐때 플레이어의 움직임을 처리합니다
            Move();
            Debug.Log(transform.position.x+" : "+transform.position.y);
        }

        if (_playerInput.IsAttack)
        {
            if (!_isAttacking)
            {
                //공격하기전 움직임을 멈춥니다.
                _playerRigid.velocity = Vector2.zero;
                //현재 공격 모션이 끝나면 공격이 가능하게 설정합니다
                Attack();
            }
        }
    }

    private void Idle()
    {
        _isIdle = true;
        _playerAni.AnimationState.SetAnimation(0, "01_Idle", true);
    }

    private void Move()
    {
        //입력 값에 따른 이동방향을 설정한다
        Vector2 v = _playerInput.Vertical * Vector3.up;
        Vector2 h = _playerInput.Horizontal * Vector3.right;

        //방향을 설정하고 속도를 곱해준다.
        Vector2 moveDir = ((v + h).normalized) * _playerState.moveSpeed;

        AniDirection(_playerInput.Horizontal);
        if (moveDir != Vector2.zero)
        {
            //실제 움직임이 있을시 움직임을 처리합니다
            if (!_isMove)
            {
                _playerAni.AnimationState.SetAnimation(0, "03_run", true);
            }
            _isMove = true;
            _playerRigid.velocity = moveDir;
        }
        else
        {
            //아무 움직임이 없을시 대기상태로 전환합니다
            if (!_isIdle)
            {
                _playerAni.AnimationState.SetAnimation(0, "01_Idle", true);
            }
            _isIdle = true;
            _isMove = false;
            _playerRigid.velocity=Vector3.zero;
        }
    }

    private void Attack()
    {
        //플레이어 공격을 처리합니다.
        _isIdle = false;
        _isMove = false;
        _isAttacking = true;
        _playerAni.AnimationState.SetAnimation(0, "05_attack", false);
        attackRect.SetActive(true);
    }

    private void AniDirection(float horizontal)
    {
        //애니매이션의 방향을 돌려줍니다.
        if (horizontal > 0)
        {
            if (direction == Direction.Left)
            {
                _playerAni.gameObject.transform.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                direction = Direction.Right;
            }
        }
        else if (horizontal < 0)
        {
            if (direction == Direction.Right)
            {
                _playerAni.gameObject.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                direction = Direction.Left;
            }
        }
    }
}
