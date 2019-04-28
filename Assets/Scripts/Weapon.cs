using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private float _windupTime = 1.5f;
    [SerializeField] private float _coolDown = 2f;

    private BoxCollider2D _collider;
    public float _coolDownTimer;
    public float _windupTimer;
    public bool _punching;
    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        _collider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        _windupTimer -= Time.deltaTime;
        _coolDownTimer -= Time.deltaTime;
    }
    
    public IEnumerator StartWindupAnimation(GameObject crow)
    {
        _windupTimer = _windupTime;
        _punching = true;

        while ((_windupTimer -= Time.deltaTime) > 0)
        {
            var dir = crow.transform.position - transform.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

            yield return null;
        }
        
        _collider.enabled = true;

        var targetPos = crow.transform.position - transform.position;

        print(targetPos);
        var targ = Vector2.MoveTowards(transform.position, targetPos, GetComponent<SpriteRenderer>().bounds.size.y);
        transform
            .DOPunchPosition(targetPos, 1.5f, 0, 0.1f)
            .OnComplete(() =>
            {
                _collider.enabled = false;
                _punching = false;
                _coolDownTimer = _coolDown;
            });
    }

   

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 8 || other.gameObject.layer == 11)
        {
            var go = other.gameObject;
            go.layer = 0;
            
            Destroy(go.GetComponent<BoxCollider2D>());

            var boidBehaviour = go.gameObject.GetComponent<BoidBehaviour>();

            if (boidBehaviour && boidBehaviour.Controller)
            {
                boidBehaviour.Controller.boids?.Remove(go);
            }

            if (boidBehaviour.isMainBoid)
            {
                if (boidBehaviour)
                {
                    if (boidBehaviour.Controller)
                    {
                        FindObjectOfType<CinemachineVirtualCamera>().Follow = boidBehaviour.Controller.boids[0].transform;
                    }
                }
            }



            boidBehaviour.enabled = false;

            var joint = gameObject.AddComponent<FixedJoint2D>();

            var closestPoint = GetComponent<Collider2D>().bounds.ClosestPoint(go.transform.position);

            joint.anchor = closestPoint;

            joint.connectedBody = other.gameObject.GetComponent<Rigidbody2D>();

            joint.enableCollision = false;

            go.transform.parent = joint.transform;
            
            Destroy(go.GetComponent<Animator>());

            var seq = DOTween.Sequence();
            seq.AppendInterval(2.0f)
                .AppendCallback(() => go.GetComponentInChildren<ParticleSystem>().Play())
                .AppendCallback(() => go.GetComponent<SpriteRenderer>().enabled = false)
                .AppendInterval(1.0f)
                .AppendCallback(() => Destroy(go))    
                .Play();
//            go.GetComponent<SpriteRenderer>()
//                .DOFade(0f, 2.5f)
//                .SetEase(Ease.Linear);

        }
    }
}
