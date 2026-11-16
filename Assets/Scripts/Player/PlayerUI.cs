using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject playerUI;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpText;

    private void OnEnable()
    {
        PlayerHealthSystem.OnHealthChanged += UpdateHPSlider;
    }

    private void OnDisable()
    {
        PlayerHealthSystem.OnHealthChanged -= UpdateHPSlider;
    }

    //Update health slider with current values
    //Called whenever health changes
    private void UpdateHPSlider(int currentHp, int maxHp)
    {
         hpText.text = currentHp.ToString();
         hpSlider.value = (float)currentHp / (float)maxHp;
    }
}
