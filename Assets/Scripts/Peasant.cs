using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Peasant : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10;
    [SerializeField] private int _maxHealth = 20;
    [SerializeField] private int _moveRange = 20;
    [SerializeField] private float _detectionRadius = 10.0f;
    [SerializeField] private LayerMask _crows;
    [SerializeField] private Weapon _weapon;

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
        transform.DOMoveX(transform.position.x + (moveValue * Random.Range(1, 2)), 5f)
            .SetEase(Ease.Linear)
            .OnComplete(MoveAround);
    }

    // Update is called once per frame
    void Update()
    {
        if (_weapon._coolDownTimer <= 0 && !_weapon._punching && _weapon._windupTimer <= 0)
        {
            var pos = transform.position;
            var crowsNear =
                Physics2D.CircleCastAll(pos, _detectionRadius, pos, 0f, _crows);

            if (crowsNear.Length > 0)
            {
                StartCoroutine(_weapon.StartWindupAnimation(crowsNear[0].transform.gameObject));
            }
        }
    }
}