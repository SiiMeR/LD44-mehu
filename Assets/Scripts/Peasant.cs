using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Peasant : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10;
    [SerializeField] private int _maxHealth = 20;
    [SerializeField] private int _moveRange = 20;
    
    private int _currentHealth;
    private SpriteRenderer _renderer;

    private bool _isMovingRight;
    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();

        MoveAround();
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
    }
}
