using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    public Slider slider;

    void Start()
    {
        slider = gameObject.GetComponent<Slider>();

        //Camera.main.ViewportToWorldPoint(transform.position);

        //Debug.Log("HP Bar Transform Pos: " + transform.position);
        //Debug.Log("HP Bar World Pos: " + Camera.main.ScreenToWorldPoint(transform.position));
    }

    public void Init()
    {
        slider = gameObject.GetComponent<Slider>();
    }

    public void SetMaxHp(int max)
    {
        slider.maxValue = max;
    }

    public void SetValue(int val)
    {
        slider.value = val;
    }
}
