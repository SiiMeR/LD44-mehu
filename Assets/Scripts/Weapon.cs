using System.Collections;
using System.Collections.Generic;
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
        _collider.enabled = true;
        _windupTimer = _windupTime;
        _punching = true;

        while ((_windupTimer -= Time.deltaTime) > 0)
        {
            var dir = crow.transform.position - transform.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

            yield return null;
        }

        var targetPos = crow.transform.position - transform.position;

        transform
            .DOPunchPosition(targetPos * 0.75f, 1.5f, 0, 0.1f)
            .OnComplete(() =>
            {
                _collider.enabled = false;
                _punching = false;
            });
    }

   

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 8 || other.gameObject.layer == 11)
        {
            var go = other.gameObject;
            go.layer = 0;
            
            Destroy(other.gameObject);
//            Destroy(go.gameObject.GetComponent<BoidBehaviour>());
//        
//            var joint = gameObject.AddComponent<FixedJoint2D>();
//
//            var closestPoint = GetComponent<Collider2D>().bounds.ClosestPoint(go.transform.position);
//
//            joint.anchor = closestPoint;
//
//            joint.connectedBody = other.gameObject.GetComponent<Rigidbody2D>();
//
//            joint.enableCollision = false;

        }
    }
}
