using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDie : MonoBehaviour
{
    private void OnDisable()
    {
        GameManager.instance.isGameover = true;
    }
}
