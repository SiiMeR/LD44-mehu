using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Peasant : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10;
    [SerializeField] private int _maxHealth = 20;
    [SerializeField] private int _moveRange = 20;
    [SerializeField] private GameObject _weapon;
    [SerializeField] private float _detectionRadius = 10.0f;
    [SerializeField] private float _windupTime = 1.5f;
    [SerializeField] private float _coolDown = 2f;
    [SerializeField] private LayerMask _crows;
    

    private float _windupTimer;
    private float _coolDownTimer;
    
    private int _currentHealth;
    private SpriteRenderer _renderer;
    
    private bool _isMovingRight;
    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();

        //MoveAround();
    }

    void MoveAround()
    {
        _isMovingRight = !_isMovingRight;
        _renderer.flipX = _isMovingRight;
        var moveValue = _isMovingRight ? _moveRange : -_moveRange;
        transform.DOMoveX(transform.position.x + (moveValue * Random.Range(1,2)), 5f)
            .SetEase(Ease.Linear)
            .OnComplete(MoveAround);
    }

    // Update is called once per frame
    void Update()
    {
        _windupTimer -= Time.deltaTime;
        _coolDownTimer -= Time.deltaTime;

        if (_coolDownTimer <= 0)
        {
            if (_windupTimer <= 0)
            {
                var crowsNear =
                    Physics2D.CircleCastAll(transform.position, _detectionRadius, transform.position, 0f, _crows);

                if (crowsNear.Length > 0)
                {
                    print("rows");
                    StartCoroutine(StartWindupAnimation(crowsNear));
                }
            }
        }
    }

    private IEnumerator StartWindupAnimation(RaycastHit2D[] crowsNear)
    {
        _windupTimer = _windupTime;
            
        while ((_windupTimer -= Time.deltaTime) > 0)
        {
            var dir = crowsNear[0].point - new Vector2(_weapon.transform.position.x, _weapon.transform.position.y);
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            _weapon.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

            yield return null;
        }
    }
}
