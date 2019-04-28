using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class EdgeKill : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _killWarning;
    [SerializeField] private float _secondsPerKill = 5;

    private float _timeInTrigger;

    private float _totalTime;

    private bool _lastDestroy;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _killWarning.DOFade(1.0f, 6.0f).SetEase(Ease.OutExpo);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == 8 || other.gameObject.layer == 11)
        {
            var boid = other.gameObject.GetComponent<BoidBehaviour>();

            if (!boid)
                return;

            if (boid.isMainBoid)
            {
                _timeInTrigger += Time.deltaTime;
                _totalTime += Time.deltaTime;
            }

            if ((_timeInTrigger > _secondsPerKill / (1 + (_totalTime * 0.2f))))
            {

                var b = FindObjectsOfType<BoidBehaviour>().Length;
                
                if (b <= 1)
                {
                    boid.DisableVisualAndPlay();
                    return;
                }
                else
                {
                    if (boid.isMainBoid)
                        return;
                    
                    boid.GetComponent<Rigidbody2D>().gravityScale = 80;
                }

                _timeInTrigger = 0f;

            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var boid = other.gameObject.GetComponent<BoidBehaviour>();
        if (!boid || boid.MarkedForKill)
            return;
            
        print($"exit? + {other.name}, {other.transform.position}");
        _killWarning.DOFade(0.0f, 4.0f);

        _totalTime = 0f;
    }


    // Update is called once per frame
    void Update()
    {
    }
}