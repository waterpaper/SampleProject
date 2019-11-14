using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject retryUi;
    private void FixedUpdate()
    {
        if(GameManager.instance.isGameover)
        {
            retryUi.SetActive(true);
        }
    }

    public void RetryButton()
    {
        SceneManager.LoadScene("GameScene_2D");
    }
}
