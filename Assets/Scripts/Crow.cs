using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crow : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _moveSpeed;
    
    private Rigidbody2D _rigidBody;
    private Camera _camera;
    private SpriteRenderer _renderer;

    // Start is called before the first frame update
    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _camera = Camera.main;
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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
        
        
        
        transform.position += transform.right * Time.deltaTime * _moveSpeed;
        _rigidBody.MovePosition(_rigidBody.position + new Vector2(transform.right.x, transform.right.y) * Time.deltaTime * _moveSpeed );
    }
}
