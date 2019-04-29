using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Peasant : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10;
    [SerializeField] private int _moveRange = 20;
    [SerializeField] private float _detectionRadius = 10.0f;
    [SerializeField] private LayerMask _crows;
    [SerializeField] private Weapon _weapon;
    [SerializeField] private int _crowsOnDeath = 3;
    public Image HitpointsImage;


    [SerializeField] private int _maxHealth = 20;
    private int _currentHealth;
    private SpriteRenderer _renderer;

    private bool _isMovingRight;

    private ParticleSystem systemP;

    private bool     isdying;

    // Start is called before the first frame update
    void Start()
    {
        systemP = GetComponentInChildren<ParticleSystem>();
        _currentHealth = _maxHealth;
        _renderer = GetComponent<SpriteRenderer>();

        //MoveAround();
    }

    public void SpawnShit()
    {
        FindObjectOfType<BoidController>()?
            .Spawn(_crowsOnDeath, transform.position);
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 8 || other.gameObject.layer == 11)
        {
            if (_currentHealth-- <= 0)
            {
                Die();
                return;
            }

            HitpointsImage.DOFillAmount(_currentHealth / (_maxHealth * 1.0f), 0.05f);
        }
    }

    private void Die()
    {
        if (isdying)
            return;
        isdying = true;
        GetComponent<BoxCollider2D>().isTrigger = false;

        var seq = DOTween.Sequence();
        seq
            .AppendCallback(() =>
            {
                Destroy(HitpointsImage.rectTransform.parent.gameObject);
                systemP.Play();
                SpawnShit();

            })
            .AppendInterval(0.25f)
            .AppendCallback(() =>
            {
                GetComponentsInChildren<SkinnedMeshRenderer>().ToList().ForEach(sr => sr.enabled = false);

            })
            .AppendInterval(1.0f)
            .AppendCallback(() =>
            {

                Destroy(gameObject);
            })
            .Play();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isdying)
            return;
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