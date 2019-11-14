using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackRect : MonoBehaviour
{
    //enemy의 attackrect를 처리하고 데미지를 띄워주는 클래스입니다.
    public GameObject Character;
    private float damage;

    // Start is called before the first frame update
    void Awake()
    {
        damage = Character.GetComponentInParent<EnemyState>().strikingPower;
        this.gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<CharacterState>().Damage((int)damage); 
        }
    }
}
