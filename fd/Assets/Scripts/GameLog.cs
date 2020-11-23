using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogData
{
    public float timer;
    public string str;
    public LogData(float timer, string str)
    {
        this.timer = timer;
        this.str = str;
    }
}

public class GameLog : MonoBehaviour
{
    List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>();
    List<Image> textBGs = new List<Image>();
    List<LogData> logs = new List<LogData>();
    int maximumLogStack;
    float autoRemoveDelay = 2f;
    void Start()
    {
        maximumLogStack = transform.childCount;
        for (int i = 0; i < maximumLogStack; i++)
        {
            texts.Add(transform.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>());
            textBGs.Add(transform.GetChild(i).GetChild(0).GetComponent<Image>());
            transform.GetChild(i).gameObject.SetActive(true);
            texts[i].color = new Color(1, 1, 1, 0);
            textBGs[i].color = new Color(0, 0, 0, 0);
        }
        UpdateUI();
    }
    void Update()
    {
        for (int i = 0; i < logs.Count; i++)
        {
            logs[i].timer -= Time.deltaTime;
            //0.5초 남았을 때 부터 사라짐
            if (logs[i].timer <= 0)
            {
                logs.RemoveAt(i);
            }
        }
        UpdateUI();
    }
    public void Add(string str)
    {
        if (logs.Count >= maximumLogStack)
        {
            if (logs.Count == 0) return;
            logs.RemoveAt(0);
        }
        logs.Add(new LogData(autoRemoveDelay, str));
        UpdateUI();
    }
    private void UpdateUI()
    {
        for (int i = 0; i < maximumLogStack; i++)
        {
            texts[i].text = "";
            texts[i].color = new Color(1, 1, 1, 0);
            textBGs[i].color = new Color(0, 0, 0, 0);
        }
        for (int i = 0; i < logs.Count; i++)
        {
            texts[i].text = logs[i].str;
            texts[i].color = new Color(1, 1, 1, logs[i].timer / 0.5f);
            float halfTransp = logs[i].timer / 0.5f > 0.5f ? 0.5f : logs[i].timer / 0.5f;
            textBGs[i].color = new Color(0, 0, 0, halfTransp);
        }
    }
}
