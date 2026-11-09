
using UnityEngine;
using UnityEngine.UI;

public class StaminaBarUI : MonoBehaviour
{
    [SerializeField] private PlayerStamina stamina;
    [SerializeField] private Slider staminaSlider;

    private void Start()
    {
        if (stamina == null)
            stamina = PlayerStamina.Instance;

        staminaSlider.minValue = 0f;
        staminaSlider.maxValue = stamina.maxStamina;
    }

    private void Update()
    {
        if (stamina == null)
            stamina = PlayerStamina.Instance;

        if (stamina == null || staminaSlider == null)
            return;

        staminaSlider.maxValue = stamina.maxStamina;
        staminaSlider.value = stamina.CurrentStamina;
    }
}
