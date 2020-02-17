using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject retryUi;
    public GameObject generalUi;
    public GameObject stageText;
    private TextMeshProUGUI _stageText;

    private void Start()
    {
        _stageText = stageText.GetComponent<TextMeshProUGUI>();
    }

    private void FixedUpdate()
    {
        if(GameManager.instance.isGameover)
        {
            retryUi.SetActive(true);
            generalUi.SetActive(false);
        }

        stageTextUpdate();
    }

    public void RetryButton()
    {
        generalUi.SetActive(true);
        GameManager.instance.stage = 1;
        SceneManager.LoadScene("GameScene_2D");
    }

    public void stageTextUpdate()
    {
        _stageText.text = "Stage : "+GameManager.instance.stage;
    }
}
