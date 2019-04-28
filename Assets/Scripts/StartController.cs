using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
            Application.Quit();

        if (Input.GetKeyDown(KeyCode.Return))
        {
            var seq = DOTween.Sequence();
            
            FindObjectsOfType<TextMeshProUGUI>().ToList().ForEach(Text => seq.Append(Text.DOFade(0f,0.75f)));

            seq.AppendCallback(() => SceneManager.LoadScene("Siim"));

            seq.Play();
        }
    }
}
