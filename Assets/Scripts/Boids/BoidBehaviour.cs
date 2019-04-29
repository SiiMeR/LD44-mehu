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
using System.Linq;
using Cinemachine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    private BoxCollider2D _boxCollider2D;

    private ParticleSystem _particleSystem;

    public bool MarkedForKill;
    // Caluculates the separation vector with a target.
    Vector3 GetSeparationVector(Transform target)
    {
        
        var diff = transform.position - target.transform.position;
        var diffLen = diff.magnitude;
        var scaler = Mathf.Clamp01(1.0f - diffLen / Controller.neighborDist);
        return diff * (scaler / diffLen);
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 10)
        {
            DisableVisualAndPlay();
        }
    }

    public void DisableVisualAndPlay()
    {
        if (MarkedForKill)
        {
            return;
        }
        
        MarkedForKill = true;
//        if (isMainBoid && Controller.BoidsCount != 0)
//        {
//            var controllerBoid = Controller.boids[0];    
////            FindObjectOfType<CinemachineVirtualCamera>().Follow = controllerBoid.transform;
//            var boidBehaviour = controllerBoid.GetComponent<BoidBehaviour>();
//            boidBehaviour.isMainBoid = true;
//            boidBehaviour.gameObject.layer = 11;
//            controllerBoid.GetComponent<BoxCollider2D>().isTrigger = true;
//        }
        
        var seq = DOTween.Sequence();
        seq.AppendCallback(() =>
            {
                GetComponent<BoxCollider2D>().isTrigger = false;
                GetComponent<SpriteRenderer>().enabled = false;
                _particleSystem.Play();
            })
            .AppendInterval(1.0f)
            .AppendCallback(() =>
            {
                if (FindObjectsOfType<BoidBehaviour>().Length == 1)
                {
                    Controller.DeathScreen.DOFade(1.5f, 1.0f)
                        .OnComplete(() => SceneManager.LoadScene("End"));
                    Destroy(Controller.MainBoid);
                }
                else
                {
                    Destroy(gameObject);
                }
            })
            .Play();
    }    

    private void OnDestroy()
    {
        Controller?.boids?.Remove(gameObject);
    }

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _particleSystem = GetComponentInChildren<ParticleSystem>();
        
        noiseOffset = Random.value * 10.0f;

        var animator = GetComponent<Animator>();
        if (animator)
            animator.speed = Random.Range(0.6f,1.0f) * animationSpeedVariation + 1.0f;

        StartCoroutine(Craw());
    }
    
    private IEnumerator Craw()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0,5));
            var randIdx = Random.Range(1, 9);
            AudioManager.Instance.Play($"crowing-{randIdx}", 0.15f,position:transform.position);   
        }   
    }
    
    void FixedUpdate()
    {
        if (MarkedForKill)
            return;
        
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

            Debug.DrawRay(transform.position, transform.right * 6.0f, Color.cyan);
            // TODO : boxcast here?
            var bounds = _boxCollider2D.bounds;
            var rch = Physics2D.BoxCast(transform.position, bounds.size, transform.rotation.z,transform.right,6f,_terrainPushAwayLayer);
            if (rch)
            {

                var rotAngle = transform.rotation.eulerAngles.z;
                if (Mathf.Abs(rotAngle % 90) < float.Epsilon) // perfect values
                {
                    // skip this branch                
                }
                else
                {
                    var quat = Quaternion.FromToRotation(transform.right, rch.normal) * transform.rotation;
                    // var quat = Quaternion.FromToRotation(transform.right, rayCastHit.normal);
                    transform.rotation = Quaternion.Lerp(transform.rotation, quat, 0.1f);
 
                }

                targetPos *= 0.2f + (rch.distance/6.0f);
            
            }

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
        var alignment = Controller.MainBoid.transform.right;
        var cohesion = Controller.MainBoid.transform.position;

        // Looks up nearby boids.
        var nearbyBoids = Physics2D.OverlapCircleAll(currentPosition, Controller.neighborDist, Controller.searchLayer);

        // Accumulates the vectors.
        foreach (var boid in nearbyBoids)
        {
            if (boid.gameObject == gameObject) continue;
            if (isMainBoid)
            {
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

       
        var bounds2 = _boxCollider2D.bounds;
        var rch2 = Physics2D.BoxCast(transform.position, bounds2.size, transform.rotation.z,transform.right,6f,_terrainPushAwayLayer);

        Debug.DrawRay(transform.position, transform.right * 6.0f, Color.cyan);
       
        if (rch2)
        {

            var rotAngle = transform.rotation.eulerAngles.z;
            if (Mathf.Abs(rotAngle % 90) < float.Epsilon) // perfect values
            {
                // skip this branch                
            }
            else
            {
                var quat = Quaternion.FromToRotation(transform.right, rch2.normal) * transform.rotation;
                // var quat = Quaternion.FromToRotation(transform.right, rayCastHit.normal);
                transform.rotation = Quaternion.Lerp(transform.rotation, quat, 0.05f);
 
            }

            targetRigidBodyPos *= 0.5f + (rch2.distance/6.0f);
            
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
        var transformRotation = transform.rotation;
        transformRotation.eulerAngles = new Vector3(0, 0, transformRotation.eulerAngles.z);
        transform.rotation = transformRotation;
        // Moves forawrd.
//        transform.position = currentPosition + transform.right * (velocity * Time.deltaTime);
    }


}