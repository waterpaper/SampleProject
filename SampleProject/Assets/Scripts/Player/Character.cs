using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Null, Left, Right, End
}
public enum ECharacterState
{
    Null,
    Idle,
    Walk,
    Run,
    Attack,
    Jump,
    End
}
public class Character : MonoBehaviour
{
    // 데이터를 저장한다
    public float moveSpeed;
    public float jumpPower;
    public int exp;
    public int expMax;
    public ECharacterState nowState;

    protected State hp;
    protected State mp;
    protected State level;
    protected State strikingPower;
    protected State defensivePower;


    public void HpPotion(int value)
    {
        hp.CurrentValue += value;
    }
    public void MpPotion(int value)
    {
        mp.CurrentValue += value;
    }
    public void SkillMana(int value)
    {
        mp.CurrentValue -= value;
    }
    public void Damage(int value)
    {
        hp.CurrentValue -= value;
        if (hp.CurrentValue <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }
    public void AddExp(int value)
    {
        exp += value;

        if(exp >= expMax)
        {
            level.CurrentValue += 1;
            exp -= expMax;
            expMax += (int)(level.CurrentValue * 100);
        }
    }
    public float attackDamageValue()
    {
        return strikingPower.CurrentValue;
    }
    public float defensivePowerValue()
    {
        return defensivePower.CurrentValue;
    }
}
