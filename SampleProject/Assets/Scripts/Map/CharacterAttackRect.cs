using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttackRect : MonoBehaviour
{
    //적 공격렉트를 처리하는 클래스입니다.
    public GameObject Character;
    private float damage;

    // Start is called before the first frame update
    void Start()
    {
        damage = Character.GetComponent<CharacterState>().attackDamageValue();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<EnemyState>().Damage(damage);
        }
    }
}
