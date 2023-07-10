//   要求。转盘启动后 每个数字依次顺时针被点亮为红色， 速度从每秒点亮20个数字（设定最高频率为20个/秒），持续1-3秒，逐步（中间频率自定义，每个频率的持续时间都是1-3秒之间，总频率数不得少于5个频率）降低到每秒点亮2个数字，并等待2秒后，落到最终设定的停止的位置的数字上，并显示为绿色。  转盘停止转动时 所有格子内的数字都要同时变成100。

//每次启动转盘都从数字1开始，
//Author:lpf

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Turntable : MonoBehaviour
{
    [SerializeField]
    private Transform m_root;
    [SerializeField]
    private GameObject m_item;
    [SerializeField]
    private Button m_startBtn;
    [SerializeField]
    private TMP_InputField m_numInput;
    [SerializeField]
    private Button m_resetBtn;

    private List<PathItem> m_path = new List<PathItem>(16);

    private Vector2 m_itemSize = new Vector2(100,100);
    
    private int m_pathLen = 16;
    private int m_startIndex = 1;
    private int m_sideLen = 4;
    private E_State m_gameState;

    private void Start()
    {
        BuilbPath();
        Reset();
    }

    private void BuilbPath()
    {
        BuildSlde(new Vector2(-2.5f* m_itemSize.x, 2.5f* m_itemSize.y), new Vector2(m_itemSize.x, 0));
        BuildSlde((m_path[m_path.Count - 1].transform as RectTransform).anchoredPosition + new Vector2(m_itemSize.x, 0), new Vector2(0, -m_itemSize.y));
        BuildSlde((m_path[m_path.Count - 1].transform as RectTransform).anchoredPosition + new Vector2(0, -m_itemSize.y), new Vector2( -m_itemSize.x, 0));
        BuildSlde((m_path[m_path.Count - 1].transform as RectTransform).anchoredPosition + new Vector2(-m_itemSize.x, 0), new Vector2(0, m_itemSize.y));
    }

    private void OnEnable()
    {
        m_startBtn.onClick.AddListener(OnClickStarBtn);
        m_numInput.onValueChanged.AddListener( OnSetInputValue);
        m_resetBtn.onClick.AddListener(Reset);
    }

    private void OnDisable()
    {
        m_startBtn.onClick.RemoveListener(OnClickStarBtn);
        m_numInput.onValueChanged.RemoveListener(OnSetInputValue);
        m_resetBtn.onClick.RemoveListener(Reset);
    }

    private void OnSetInputValue(string arg0)
    {
        if (string.IsNullOrEmpty(arg0))
        {
            return;
        }
        int v;
        if (int.TryParse(arg0, out v))
        {
            //todo temp
            if (!(v < m_startIndex || v > m_pathLen))
            {
                return;
            }
        }
        //todo need tips
        m_numInput.text = "1";
    }

    private void OnClickStarBtn()
    {
        if (m_gameState != E_State.readyForPlay)
        {
            //todo not ready tips
            return;
        }

        string str = m_numInput.text;
        if (string.IsNullOrEmpty(str))
        {
            //todo need tips
            return;
        }

        uint v;
        if (uint.TryParse(m_numInput.text, out v))
        {
            if (v < 16)
            {
                PlayTurnable(v);
            }
        }
        else
        {
            //todo need tips
            return;
        }
    }

    private void PlayTurnable(uint v)
    {
        m_gameState = E_State.playing;
        StartCoroutine(IEPlayTurnable(v));
    

    }

    IEnumerator IEPlayTurnable(uint v)
    {
        //calculate path
        int max = 20;
        float maxTime = Random.Range(1f, 3f);
        int maxPath = (int)(max * maxTime);

        int min = 2;
        int minTime = 2;
        int minPath = min * minTime;

        int step = 5;// Random.Range(5, 10);
       
        float midTime = 0;
        for (int i = 0; i < step; i++)
        {
            yield return new WaitForEndOfFrame();
            midTime += Random.Range(1f, 3f);
        }

        int midPath = (int)(11f * midTime);

        int tar = (int)v;
        while (tar < minPath)
        {
            tar += m_pathLen;
        }
        tar -= minPath;

  
        int sub = tar - ((maxPath + midPath) % m_pathLen);
        if (sub < 0)
        {
            sub += m_pathLen;
        }

        midPath += sub;
        float midSpeed = ((float)(max + min)) / 2f;
        midTime = midPath /midSpeed;
        Debug.Log($"intput:{m_numInput.text}  maxOath :{maxPath} midPath:{midPath} minPath{minPath} add{(maxPath + minPath + midPath)%16}");
        float playPath = 0;
        float playTime = Time.time;
        float useTime = Time.time - playTime;
        float movePath = 0;

        //todo refactor
        while (useTime < maxTime)
        {
            yield return new WaitForEndOfFrame();

            movePath = Mathf.Lerp(0, maxPath, useTime/maxTime);
         

            useTime = Time.time - playTime;
            
            if (useTime > maxTime)
            {
                playPath = maxPath;
                LightupPathItemn((int)playPath);
                break;
            }
            else
            {
              
                LightupPathItemn((int)(playPath + movePath));
            }
        }

        playTime = Time.time;
        useTime = Time.time - playTime;
        movePath = 0;
        float a = (max - min) / midTime;
        while (useTime < midTime)
        {
            yield return new WaitForEndOfFrame();
            useTime = Time.time - playTime;
          
            float minv = max - a * useTime;
            movePath = ((float)max + minv )/ 2f * useTime;
            //Mathf.Lerp(playPath, midPath, useTime);

            //    playPath += useTime * max;
            if (useTime > midTime)
            {
                playPath = maxPath + midPath;
                LightupPathItemn((int)(playPath));
                break;
            }
            else
            {
                Debug.Log((int)(playPath + movePath));
                LightupPathItemn((int)(playPath + movePath + 1));
            }
        }

        playTime = Time.time;
        useTime = Time.time - playTime;
        movePath = 0;
        while (useTime < minTime)
        {
            yield return new WaitForEndOfFrame();

            movePath = Mathf.Lerp(0, minPath, useTime/maxTime);


            useTime = Time.time - playTime;
            if (useTime > minTime)
            {
                playPath = maxPath + midPath + minPath;
                SelectItem((int)playPath);
                break;
            }
            else
            {
                LightupPathItemn((int)(playPath + movePath));
            }
        }

        yield return new WaitForSeconds(0.1f);

        ChangeAllItemText();

        yield break;
    }

    private void ChangeAllItemText()
    {
        for (int i = 0; i < m_path.Count; i++)
        {
             m_path[i].SetText("100");
        }
    }

    private void SelectItem(int playPath)
    {
        int index = (playPath % m_pathLen) - 1;
        for (int i = 0; i < m_path.Count; i++)
        {
            if (i != index)
            {
                m_path[i].LightUp(false);
            }
            else
            {
                m_path[index].OnSelect();
            }
        }
    }

    private void LightupPathItemn(int playPath)
    {
        int index = (playPath % m_pathLen) -1;

        for (int i = 0; i < m_path.Count; i++)
        {
            m_path[i].LightUp(i == index );
        }
    }

    private void Reset()
    {
        foreach (var elem in m_path)
        {
            elem.Reset();
        }
        m_gameState = E_State.readyForPlay;
    }

    private void BuildSlde(Vector2 startPos, Vector2 offset)
    {
        for (int i = 0; i < m_sideLen; i++)
        {
            var go = Instantiate(m_item, m_root);
            (go.transform as RectTransform).anchoredPosition = startPos + offset*i;
            var pi = go.GetComponent<PathItem>();
            pi.gameObject.SetActive(true);
            m_path.Add(pi);
            pi.Init(m_path.Count);
        }
    }

    private enum E_State
    { 
        readyForPlay,
        playing,
    }
}
