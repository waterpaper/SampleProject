using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager m_instance;

    [Header("Object Pool")]
    //적 캐릭터가 출현할 위치를 담을 배열
    public Transform[] points;
    //적 캐릭터의 최대 생성 개수
    public int maxEnemy = 5;
    //적 캐릭터 프리팹
    public GameObject enemyPrefabs;
    //오브젝트 풀에 저장할 에너미 수
    public List<GameObject> enemyPool = new List<GameObject>();
   
    //싱글톤 접근
    public static GameManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<GameManager>();
            }
            return m_instance;
        }
    }

    //게임오버선택
    public bool isGameover { get; set; }

    private void Awake()
    {
        if (instance != this)
        {
            Destroy(gameObject);
        }
        CreatePooling();
        StartCoroutine("CreateEnemy");
        isGameover = false;
    }
    
    //외부에서 새적을 생성하라고 요청하는 함수
    public void NewCreatEnemy()
    {
        StartCoroutine("CreateEnemy");
    }

    //모든 적이 비활성화 상태인지 확인하는 함수
    public bool EmptyNowEnemy()
    {
        for(int i =0;i< enemyPool.Count; i++)
        {
            if (enemyPool[i].activeSelf == true)
                return false;
        }

        return true;
    }

    //적 캐릭터를 생성하는 코루틴함수
    IEnumerator CreateEnemy()
    {
        int num = Random.Range(1, maxEnemy);

        for (int i = 0; i < num; i++)
        {
            //비황성화 여부로 사용 가능한 오브젝트인지를 판단
            if (enemyPool[i].activeSelf == false)
            {
                //불 규칙적인 위치 산출
                int idx = Random.Range(1, points.Length);
                //적 캐릭터의 오브젝트를 활성화 시킨다.
                enemyPool[i].transform.localPosition = points[idx].position;
                enemyPool[i].transform.localRotation = points[idx].rotation;
                enemyPool[i].SetActive(true);
            }
        }
        yield return null;
    }

    //적 캐릭터를 시작시 생성합니다.
    //오브젝트 풀에 적을 생성하는 함수
    public void CreatePooling()
    {
        //총알을 생성해 차일드화할 페이런트 게임오브젝트를 생성
        GameObject objectPools = new GameObject("objectPools");

        //풀링개수만큼 미리 적을 생성
        for (int i = 0; i < maxEnemy; i++)
        {
            var obj = Instantiate<GameObject>(enemyPrefabs, objectPools.transform);
            obj.name = "Enemy" + i.ToString("00");
            //비활성화
            obj.SetActive(false);
            //리스트에 생성한 총알 추가
            enemyPool.Add(obj);
        }
    }
}
