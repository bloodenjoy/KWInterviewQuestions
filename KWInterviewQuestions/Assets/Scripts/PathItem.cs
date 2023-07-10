using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PathItem : MonoBehaviour
{
    private int m_startNum;
    [SerializeField]
    private Image m_image;
    [SerializeField]
    private TextMeshProUGUI m_proUGUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void Init(int count)
    {
        m_startNum = count;
    }

    internal void Reset()
    {
        m_proUGUI.text = m_startNum.ToString();
        m_proUGUI.color = Color.black;
    }

    internal void LightUp(bool v)
    {
        m_proUGUI.color = v ? Color.red : Color.black;
    }

    internal void OnSelect()
    {
        m_proUGUI.color = Color.green;
    }

    internal void SetText(String str)
    {
        m_proUGUI.text = str;
    }
}
