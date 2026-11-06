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


    private void UpdateHPSlider(int currentHp, int maxHp)
    {
         hpText.text = currentHp.ToString();
         hpSlider.value = (float)currentHp / (float)maxHp;
    }
}
