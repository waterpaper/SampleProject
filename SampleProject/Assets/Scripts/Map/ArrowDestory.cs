using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowDestory : MonoBehaviour
{
    public GameObject particleObj;
    public float damage = 20.0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" || collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            if (collision.tag == "Enemy")
            {
                collision.gameObject.GetComponent<EnemyState>().Damage(damage);
            }

            StartCoroutine("arrowDestory");
        }
    }
    
    IEnumerator arrowDestory()
    {
        transform.parent.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Instantiate(particleObj,transform.position, transform.parent.rotation);
        yield return new WaitForSeconds(0.2f);
        transform.parent.gameObject.SetActive(false);
    }
    
}
