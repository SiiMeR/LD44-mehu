using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject reallyQuit;
    // Start is called before the first frame update
    void Start()
    {
        reallyQuit.gameObject.SetActive(false);
    }
    
    

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = reallyQuit.gameObject.activeInHierarchy ? 0.0f : 1.0f;
        
        if (Input.GetButtonDown("Cancel"))
        {
            reallyQuit.gameObject.SetActive(!reallyQuit.gameObject.activeInHierarchy);
            
        }
        
        if (reallyQuit.gameObject.activeInHierarchy)
        {
                if(Input.GetKeyDown(KeyCode.Y))
                    Application.Quit();
                if(Input.GetKeyDown(KeyCode.N))
                    reallyQuit.gameObject.SetActive(false);
        }
    }
}
