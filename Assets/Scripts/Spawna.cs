using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawna : MonoBehaviour
{

    public GameObject prefab;

    public int timer;

    private float timespent;

    public Transform point;
    public GameObject currentPr;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timespent += Time.deltaTime;

        if (timespent > timer)
        {
            timespent = 0;
            if (currentPr == null)
            {
               currentPr = Instantiate(prefab, point.position, Quaternion.identity);
            }
        }
    }    
}
