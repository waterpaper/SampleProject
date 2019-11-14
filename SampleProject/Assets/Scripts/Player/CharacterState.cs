using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterState : Character
{
    //캐릭터의 초기값을 설정해줍니다.
    public float initHp = 100.0f;
    public float initMp = 100.0f;
    public float initLevel = 1.0f;
    public float initStrikingPower = 10.0f;
    public float initDefensivePower = 0.0f;

    //추가값의 최대값을 설정해줍니다.
    public float maxHp = 100.0f;
    public float maxMp = 100.0f;
    public float maxLevel = 100.0f;
    public float maxStrikingPower = 10000.0f;
    public float maxDefensivePower = 10000.0f;

    //화면에 체력바를 보여주기 위한 UI이다
    public GameObject healthBarBackground;
    public Image healthBarFilled;

    private void Awake()
    {
        //캐릭터의 초기 데이터를 초기화합니다.
        moveSpeed = 5.0f;
        jumpPower = 5.0f;
        exp = 0;
        expMax = 100;

        hp.Initialize(initHp, maxHp);
        mp.Initialize(initMp, maxMp);
        level.Initialize(initLevel, maxLevel);
        strikingPower.Initialize(initStrikingPower, maxStrikingPower);
        defensivePower.Initialize(initDefensivePower, maxDefensivePower);

        healthBarFilled.fillAmount = 1;

        StartCoroutine("UIBarUpdate");
    }

    IEnumerator UIBarUpdate()
    {
        while (hp.CurrentValue > 0)
        {
            healthBarFilled.fillAmount = hp.CurrentValue / hp.MaxValue;

            yield return new WaitForSeconds(0.2f);
        }
    }
}
