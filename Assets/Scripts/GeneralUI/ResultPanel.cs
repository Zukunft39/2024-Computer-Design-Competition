using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class ResultPanel : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private Animator maskAnimator;
    [SerializeField]
    private Text resultText;
    [SerializeField]
    private Button closeBtn;

    
    private void Awake()
    {
        closeBtn.onClick.AddListener(EnterPreviousLevel);
    }

    public void ShowResultPanel(String result)
    {
        resultText.text = result;
        gameObject.SetActive(true);
        var s = transform.localScale;
        s.z = 0;
        transform.localScale = s;
        _animator.SetTrigger("fadeIn");
    }

    public void HideResultPanel()
    {
        resultText.text = "";
        gameObject.SetActive(false);
    }

    private void EnterPreviousLevel()
    {
        maskAnimator.gameObject.SetActive(true);
        maskAnimator.SetTrigger("fadeIn");
        StartCoroutine(EnterPreviousLevelCoroutine());
    }

    IEnumerator EnterPreviousLevelCoroutine()
    {
        yield return new WaitForSeconds(3);
        StartCoroutine(SceneChangeManager.Instance.LoadSceneAsync("DemoScene"));
    }
}
