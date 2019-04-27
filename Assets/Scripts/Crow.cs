using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crow : MonoBehaviour
{
//    [SerializeField] public float _rotationSpeed;
//    [SerializeField] public float _moveSpeed;
//    [SerializeField] public LayerMask _terrainPushAwayLayer;
//
//    public GameObject _targetRemoteObject;
//
//    public GameObject TargetRemoteObject
//    {
//        get
//        {
//            if (_targetRemoteObject == null)
//            {
//                var targetRemoteObject = Controller.boids[0];    
//                _targetRemoteObject = targetRemoteObject.gameObject;
////                Destroy(targetRemoteObject);
//            }
//            return _targetRemoteObject;
//        }
//        set => _targetRemoteObject = value;
//    }
//
//    public BoidController Controller
//    {
//        get
//        {
//            if (_controller == null)
//            {
//                _controller = FindObjectOfType<BoidController>();
//            }
//            return _controller;
//        }
//        set => _controller = value;
//    }
//    
//    private BoidController _controller;
//    private Camera _camera;
////    private SpriteRenderer _renderer;
////    private Rigidbody2D _rigidbody2D;
//
//    private SpriteRenderer _renderer => TargetRemoteObject.GetComponent<SpriteRenderer>();
//     
//    private Rigidbody2D _rigidbody2D => TargetRemoteObject.GetComponent<Rigidbody2D>();
//
//    private int _crows;    
//    public int Crows
//    {
//        get => _crows;
//        set => _crows = value;
//    }
//    // Start is called before the first frame update
//    void Awake()
//    {
//        _camera = Camera.main;
//        Crows = Controller.BoidsCount + 1;
//        _targetRemoteObject.GetComponent<BoidBehaviour>().isMainBoid = true;
//    }
//
//
//    private void OnCollisionEnter2D(Collision2D other)
//    {
//        if (other.gameObject.layer == 10)
//        {
////            var controllerBoid = Controller.boids[0];
////            Controller.transform.parent = controllerBoid.transform;
//            Destroy(gameObject);
//        }
//    }
//
//
//    // Update is called once per frame
//    void FixedUpdate()
//    {
//        if (Input.GetKey(KeyCode.Z) && Controller.boids.Count > 0)
//        {
//            var controllerBoid = Controller.boids[0];
//            Controller.boids.Remove(controllerBoid);
//            Destroy(controllerBoid);
//        }
//        
//        var rot = TargetRemoteObject.transform.rotation.eulerAngles.z;
//        _renderer.flipY = rot <= 270f && rot > 90f;
//        var horizontalMovement = Input.GetAxisRaw("Horizontal");
//        if (horizontalMovement > 0f)
//        {
//            TargetRemoteObject.transform.Rotate(Vector3.back * _rotationSpeed * Time.deltaTime);
//        }
//        else if (horizontalMovement < 0f)
//        {
//            TargetRemoteObject.transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);            
//        }
//        
//        Crows = Controller.BoidsCount + 1;
//     
//        var targetRigidBodyPos = new Vector2(TargetRemoteObject.transform.right.x, TargetRemoteObject.transform.right.y) * Time.deltaTime * _moveSpeed;
//
//
//        var rayCastHit = Physics2D.Raycast(TargetRemoteObject.transform.position, TargetRemoteObject.transform.right, 6.0f,_terrainPushAwayLayer);
//        
//        Debug.DrawRay(TargetRemoteObject.transform.position, TargetRemoteObject.transform.right * 6.0f, Color.red);
//       
//        if (rayCastHit)
//        {
//            var rotAngle = TargetRemoteObject.transform.rotation.eulerAngles.z;
//            if (Math.Abs(rotAngle % 90) < float.Epsilon) // perfect values
//            {
//            // skip this branch                
//            }
//            else
//            {
//                var quat = Quaternion.FromToRotation(TargetRemoteObject.transform.right, rayCastHit.normal) * TargetRemoteObject.transform.rotation;
//                // var quat = Quaternion.FromToRotation(transform.right, rayCastHit.normal);
//                TargetRemoteObject.transform.rotation = Quaternion.Lerp(TargetRemoteObject.transform.rotation, quat, 0.15f);
// 
//            }
//
//            targetRigidBodyPos *= 0.1f + (rayCastHit.distance/6.0f);
//            
//        }
//            
//        _rigidbody2D.MovePosition(_rigidbody2D.position + targetRigidBodyPos );
//        
//
//    }
}
