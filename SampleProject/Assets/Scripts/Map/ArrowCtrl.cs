using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCtrl : MonoBehaviour
{
    //총알 기본 스텟
    public float speed = 1000.0f;

    private void OnEnable()
    {
        GetComponent<Rigidbody2D>().AddForce(transform.right*speed);
    }

    private void OnDisable()
    {
        GetComponent<Rigidbody2D>().velocity=new Vector2(0.0f, 0.0f);
    }
}
