//
// Boids - Flocking behavior simulation.
//
// Copyright (C) 2014 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using UnityEngine;
using System.Collections;
using Cinemachine;

public class BoidBehaviour : MonoBehaviour
{
    // Reference to the controller.
    public BoidController controller;
    
    public BoidController Controller
    {
        get
        {
            if (controller == null)
            {
                controller = FindObjectOfType<BoidController>();
            }
            return controller;
        }
        set => controller = value;
    }

    // Options for animation playback.
    public float animationSpeedVariation = 0.2f;

    public bool isMainBoid;

    public LayerMask _terrainPushAwayLayer;
    
    // Random seed.
    float noiseOffset;

    private SpriteRenderer _renderer;

    private Rigidbody2D _rigidBody;
    // Caluculates the separation vector with a target.
    Vector3 GetSeparationVector(Transform target)
    {
        
        var diff = transform.position - target.transform.position;
        var diffLen = diff.magnitude;
        var scaler = Mathf.Clamp01(1.0f - diffLen / Controller.neighborDist);
        return diff * (scaler / diffLen);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == 10)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 10)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        Controller.boids.Remove(gameObject);

        if (Controller.transform.parent == gameObject.transform)
        {
            if (Controller.boids.Count == 0) return;
            var controllerBoid = Controller.boids[0];    
            Controller.transform.parent = controllerBoid.transform;
            Controller.transform.localPosition = new Vector3(5,0,0);
            Controller.enabled = true;
            FindObjectOfType<CinemachineVirtualCamera>().Follow = controllerBoid.transform;
            var boidBehaviour = controllerBoid.GetComponent<BoidBehaviour>();
            boidBehaviour.isMainBoid = true;
            boidBehaviour.gameObject.layer = 0;
            controllerBoid.GetComponent<BoxCollider2D>().isTrigger = true;
        }
    }

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();

        noiseOffset = Random.value * 10.0f;

        var animator = GetComponent<Animator>();
        if (animator)
            animator.speed = Random.Range(0.6f,1.0f) * animationSpeedVariation + 1.0f;
    }

    public void DestroyThis()
    {
        Destroy(this);
    }
    
    
    
    void FixedUpdate()
    {
        var rot = transform.rotation.eulerAngles.z;
        _renderer.flipY = rot <= 270f && rot > 90f;

        if (isMainBoid)
        {
            var horizontalMovement = Input.GetAxisRaw("Horizontal");
            if (horizontalMovement > 0f)
            {
                transform.Rotate(Vector3.back * 200 * Time.deltaTime);
            }
            else if (horizontalMovement < 0f)
            {
                transform.Rotate(Vector3.forward * 200 * Time.deltaTime);            
            }
            
            var targetPos = new Vector2(transform.right.x, transform.right.y) * Time.deltaTime * Controller.velocity;

            _rigidBody.MovePosition(_rigidBody.position + targetPos );
            
            return;

        }
        //        _renderer.flipY = Mathf.Abs(transform.rotation.eulerAngles.z % 360) > 90f;

        var currentPosition = transform.position;
        var currentRotation = transform.rotation;

        // Current velocity randomized with noise.
        var noise = Mathf.PerlinNoise(Time.time, noiseOffset) * 2.0f - 1.0f;
        var velocity = Controller.velocity * (1.0f + noise * Controller.velocityVariation);

        // Initializes the vectors.
        var separation = Vector3.zero;
        var alignment = Controller.transform.right;
        var cohesion = Controller.transform.position;

        // Looks up nearby boids.
        var nearbyBoids = Physics2D.OverlapCircleAll(currentPosition, Controller.neighborDist, Controller.searchLayer);

        // Accumulates the vectors.
        foreach (var boid in nearbyBoids)
        {
            if (boid.gameObject == gameObject) continue;
            if (Controller.transform.parent.name == boid.gameObject.name && isMainBoid)
            {
                print("Mothefucker");
                continue;
            }
            var t = boid.transform;
            separation += GetSeparationVector(t);
            alignment += t.right;
            cohesion += t.position;
        }

        var avg = 1.0f / nearbyBoids.Length;
        alignment *= avg;
        cohesion *= avg;
        cohesion = (cohesion - currentPosition).normalized; 

        // Calculates a rotation from the vectors.
        var direction = separation + alignment + cohesion;
        direction.z = 0;

        var targetRigidBodyPos = new Vector2(transform.right.x, transform.right.y) * Time.deltaTime * velocity;

       
        var rayCastHit = Physics2D.Raycast(transform.position, transform.right, 6.0f,_terrainPushAwayLayer);
        
        Debug.DrawRay(transform.position, transform.right * 6.0f, Color.cyan);
       
        if (rayCastHit)
        {

            var rotAngle = transform.rotation.eulerAngles.z;
            if (Mathf.Abs(rotAngle % 90) < float.Epsilon) // perfect values
            {
                // skip this branch                
            }
            else
            {
                var quat = Quaternion.FromToRotation(transform.right, rayCastHit.normal) * transform.rotation;
                // var quat = Quaternion.FromToRotation(transform.right, rayCastHit.normal);
                transform.rotation = Quaternion.Lerp(transform.rotation, quat, 0.2f);
 
            }

            targetRigidBodyPos *= 0.1f + (rayCastHit.distance/6.0f);
            
        }
        else
        {
            var rotation = Quaternion.FromToRotation(Vector3.right, direction.normalized);

            // Applys the rotation with interpolation.
            if (rotation != currentRotation)
            {
                var ip = Mathf.Exp(-Controller.rotationCoeff * Time.deltaTime);
                transform.rotation = Quaternion.Slerp(rotation, currentRotation, ip);
            }
        }


        _rigidBody.MovePosition(_rigidBody.position + targetRigidBodyPos );
        
        // Moves forawrd.
//        transform.position = currentPosition + transform.right * (velocity * Time.deltaTime);
    }
}