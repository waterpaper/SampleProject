using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMap : MonoBehaviour
{
    //플레이어 오브젝트
    public GameObject player;
    public GameObject character1;
    public GameObject character2;
    //넘어갈때 생성될 위치
    public GameObject startPoint;
    //클리어를 알리는 파티클
    public ParticleSystem particle;

    public bool stageClear=false;


    private void FixedUpdate()
    {
        //모든 몬스터를 잡으면 업데이트한다
        if(!stageClear && GameManager.instance.EmptyNowEnemy())
        {
            stageClear = true;
            particle.Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if(stageClear)
            {
                //스테이지 클리어시 맵을 넘어간다.
                NewStage();
            }
        }
    }

    void NewStage()
    {
        //다음 맵을 세팅해 주는 함수
        //캐릭터를 이동시킵니다.
        player.transform.position = startPoint.transform.position;
        character1.transform.position = startPoint.transform.position;
        character2.transform.position = startPoint.transform.position;
        
        //세팅값을 설정합니다.
        stageClear = false;
        particle.Stop();

        GameManager.instance.stage += 1;
        //몬스터를 재배치합니다.
        GameManager.instance.NewCreatEnemy();
    }
}
