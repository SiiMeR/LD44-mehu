using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Crow : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private TextMeshProUGUI _crowLeftText;
    [SerializeField] private LayerMask _terrainPushAwayLayer;
    
    
    private BoidController _controller;
    private Camera _camera;
    private SpriteRenderer _renderer;
    private Rigidbody2D _rigidbody2D;

    private int _crows;
    public int Crows
    {
        get
        {
            return _crows;
        }
        set
        {
            _crowLeftText.text = "<sprite=0> = " + value.ToString();
            _crows = value;
        }
    }
    // Start is called before the first frame update
    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();
        _camera = Camera.main;
        _controller = GetComponentInChildren<BoidController>();
        Crows = _controller.BoidsCount + 1;
        
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Z) && _controller.boids.Count > 0)
        {
            var controllerBoid = _controller.boids[0];
            _controller.boids.Remove(controllerBoid);
            Destroy(controllerBoid);
        }
        
        var rot = transform.rotation.eulerAngles.z;
        _renderer.flipY = rot <= 270f && rot > 90f;
        var horizontalMovement = Input.GetAxisRaw("Horizontal");
        if (horizontalMovement > 0f)
        {
            transform.Rotate(Vector3.back * _rotationSpeed * Time.deltaTime);
        }
        else if (horizontalMovement < 0f)
        {
            transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);            
        }
        
        Crows = _controller.BoidsCount + 1;
     
        var targetRigidBodyPos = new Vector2(transform.right.x, transform.right.y) * Time.deltaTime * _moveSpeed;


        var rayCastHit = Physics2D.Raycast(transform.position, transform.right, 5.0f,_terrainPushAwayLayer);
        
        Debug.DrawRay(transform.position, transform.right * 5.0f, Color.red);
       
        if (rayCastHit)
        {
            Debug.DrawRay(transform.position, rayCastHit.normal * 5.0f, Color.yellow);

            var angle = 1;
            var hitIsLeft = (rayCastHit.point - new Vector2(transform.position.x, transform.position.y)).x < 0;
            var hitIsDown = (rayCastHit.point - new Vector2(transform.position.x, transform.position.y)).y < 0; 
            
//            var quat = Quaternion.FromToRotation(new Vector3(1,0, 0), rayCastHit.normal);
            var rotAngle = transform.rotation.eulerAngles.z;
            if (rotAngle % 90 == 0) // perfect values
            {
            // skip this branch                
            }
            else
            {
                var quat = Quaternion.FromToRotation(transform.right, rayCastHit.normal) * transform.rotation;
                // var quat = Quaternion.FromToRotation(transform.right, rayCastHit.normal);
                transform.rotation = Quaternion.Lerp(transform.rotation, quat, 0.1f);
 
            }

            targetRigidBodyPos *= 0.1f + (rayCastHit.distance/5.0f);
            
        }
            
        _rigidbody2D.MovePosition(_rigidbody2D.position + targetRigidBodyPos );
        

    }
}
