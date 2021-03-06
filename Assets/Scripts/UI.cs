﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    private TextMeshProUGUI _textMeshProUgui;

    private BoidController _controller;

    public BoidController Controller
    {
        get
        {
            if (_controller == null)
            {
                _controller = FindObjectOfType<BoidController>();
            }
            return _controller;
        }
        set => _controller = value;
    }
    // Start is called before the first frame update
    void Start()
    {
        _textMeshProUgui = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        var len = FindObjectsOfType<BoidBehaviour>().Length;

        _textMeshProUgui.text = "<sprite=0> = " + len ;
    }
}
