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
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BoidController : MonoBehaviour
{
    public GameObject boidPrefab;

    public int spawnCount = 15;

    public float spawnRadius = 4.0f;

    [Range(0.1f, 20.0f)]
    public float velocity = 6.0f;

    [Range(0.0f, 0.9f)]
    public float velocityVariation = 0.5f;

    [Range(0.1f, 20.0f)]
    public float rotationCoeff = 4.0f;

    [Range(0.1f, 10.0f)]
    public float neighborDist = 2.0f;

    public LayerMask searchLayer;

    public List<GameObject> boids;

    public Image DeathScreen;
    public int BoidsCount => boids.Count;

    public CinemachineVirtualCamera camm;
    [SerializeField]
    private GameObject _mainBoid;

    public GameObject MainBoid
    {
        get
        {
            if (_mainBoid == null)
            {
                var boid = FindObjectOfType<BoidBehaviour>();

                if ((!FindObjectOfType<Kunn>()?.isdying) ?? false)
                {
                    camm.Follow = boid.transform;
                }
                boid.isMainBoid = true;
                _mainBoid = boid.transform.gameObject;
            }
            return _mainBoid;
        }
        set => _mainBoid = value;
    }

    void Start()
    {
        boids = new List<GameObject>();    

        for (var i = 0; i < spawnCount; i++)
            boids.Add(Spawn());
        
        DeathScreen.color = Color.black;
        DeathScreen.DOFade(0f, 4f)
            .SetEase(Ease.InQuint);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Plus)|| Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            AudioManager.Instance.musicVolume += 0.1f;
            AudioManager.Instance.soundVolume += 0.1f;
        } 
        
        if (Input.GetKeyDown(KeyCode.Minus)  || Input.GetKeyDown(KeyCode.KeypadMinus) )
        {
            AudioManager.Instance.musicVolume -= 0.1f;
            AudioManager.Instance.soundVolume -= 0.1f;
        }
        
        
        if (Input.GetKey(KeyCode.F1))
        {
            boids.Add(Spawn(MainBoid.transform.position));
        }

        if (Input.GetKey(KeyCode.F2))
        {
            boids.ForEach(boid => boid.GetComponent<BoidBehaviour>().DisableVisualAndPlay());
        }

        if (Input.GetKey(KeyCode.F3))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            boids.ForEach(boid => boid.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll);
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            Destroy(MainBoid);
        }
    }

    public GameObject Spawn()
    {
        var insideUnitCircle = Random.insideUnitCircle;
        return Spawn(MainBoid.transform.position + new Vector3(insideUnitCircle.x, insideUnitCircle.y, 0) * spawnRadius);
    }

    public void Spawn(int number, Vector3 position)
    {
        for (int i = 0; i < number; i++)
        {
            boids.Add(Spawn(position));
        }
    }

    public GameObject Spawn(Vector3 position)
    {
        var rotation = Quaternion.Slerp(MainBoid.transform.rotation, Random.rotation, 0.3f);
        var boid = Instantiate(boidPrefab, position, rotation);
        
        var r = Random.Range(0.7f, 1.3f);
        var boidCOmponent = boid.GetComponent<BoidBehaviour>();
        boidCOmponent.controller = this;
        boidCOmponent.transform.localScale *= r;
        
        return boid;
    }
}