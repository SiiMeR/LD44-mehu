using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Kunn : MonoBehaviour
{
    private Animator anim;


    public SwordCollisoin weapon;
    public float DetectionRange = 10.0f;
    public float WindupTime = 0.5f;
    public float CoolDown = 5.0f;
        
    public int MaxHealth = 20;
    private int _currentHealth;
    
    private bool _swinging;
    private float _windupTimer;
    private float _cooldownTimer;

    private BoxCollider2D _collider2D;

    public LayerMask _crows;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 8 || other.gameObject.layer == 11)
        {
            if (_currentHealth-- <= 0)
            {
                Die();
                return;
            }
        }
    }

    private void Die()
    {
        GetComponent<BoxCollider2D>().isTrigger = false;
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        _currentHealth = MaxHealth;
        anim = GetComponent<Animator>();
        _collider2D = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        _windupTimer -= Time.deltaTime;
        _cooldownTimer -= Time.deltaTime;

        if (_cooldownTimer <= 0 && !_swinging && _windupTimer <= 0)
        {
            var pos = transform.position;
            Debug.DrawLine(transform.position, transform.position * DetectionRange);
            var crowsNear =
                Physics2D.CircleCastAll(transform.position, DetectionRange, transform.position, 0f, _crows);

            if (crowsNear.Length > 0)
            {
                StartCoroutine(StartWindupAnimation(crowsNear[0].transform.gameObject));
            }
        }
    }

    IEnumerator StartWindupAnimation(GameObject crow)
    {
        _windupTimer = WindupTime;
        _swinging = true;

//        _animationController.
        while ((_windupTimer -= Time.deltaTime) > 0)
        {
            yield return null;
        }
        weapon.GetComponent<BoxCollider2D>().enabled = true;

        var shouldFlip = crow.transform.position.x < transform.position.x;

        transform.rotation = Quaternion.Euler(0, shouldFlip ? 180 : 0, 0);

        anim.SetTrigger("Attack");
    }

    public IEnumerator AttackComplete()
    {
        yield return new WaitForSeconds(0.5f);
        anim.SetTrigger("Attack");   
    }

    public void OnSecondAttackStarting()
    {
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 180, 0);
    }
    public IEnumerator SecondAttackCOmplete()
    {
        _swinging = false;
        _cooldownTimer = CoolDown;

        weapon.GetComponent<BoxCollider2D>().enabled = false;

        yield return new WaitForSeconds(0.5f);

        var srenderers = GetComponentsInChildren<SpriteRenderer>();
        
        foreach (var spriteRenderer in srenderers)
        {
            spriteRenderer.DOColor(Color.black, 0.4f).SetEase(Ease.Linear).SetLoops(14, LoopType.Yoyo);
        }
        
        yield return new WaitForSeconds(5.0f);
        
        anim.SetTrigger("VulnerEnd");


    }
}