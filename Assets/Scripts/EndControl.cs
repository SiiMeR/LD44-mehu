using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndControl : MonoBehaviour
{
    public TextMeshProUGUI _fail;
    public TextMeshProUGUI _pressEnter;
    public TextMeshProUGUI _escText;
    // Start is called before the first frame update
    void Start()
    {
        _fail.color = Color.clear;
        _escText.color = Color.clear;
        _pressEnter.color = Color.clear;

        var seq = DOTween.Sequence();
        seq.Append(_fail.DOColor(Color.white, 2.0f))
            .Append(_escText.DOColor(Color.white, 0.25f))
            .Append(_pressEnter.DOColor(Color.white, 1.5f))
            .OnComplete(() => StartCoroutine(WaitEnter()));
        
    }

    private IEnumerator WaitEnter()
    {
        yield return new WaitUntil(() =>Input.GetKeyDown(KeyCode.Return));
        SceneManager.LoadScene("Siim");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Cancel"))
            Application.Quit();
    }
}
