using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    public Slider slider;

    public void Init()
    {
        slider = gameObject.GetComponent<Slider>();
        // transform.SetAsFirstSibling();
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
