using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    public static PlayerStamina Instance { get; private set; }

    [Header("Stamina")]
    [Min(1f)] public float maxStamina = 100f;
    [SerializeField] private float currentStamina;

    [Header("Regeneration")]
    [Min(0.01f)] public float regenAmount = 1f;
    [Min(0.001f)] public float regenInterval = 0.05f;
    [Min(0f)] public float regenDelay = 0.75f;
    [Min(0f)] public float emptyRegenDelay = 2f;

    [Header("Movement Costs")]
    [Min(0f)] public float sprintCost = 1f;
    [Min(0.001f)] public float sprintInterval = 0.125f;
    [Min(0f)] public float wallRunCost = 1f;
    [Min(0.001f)] public float wallRunInterval = 0.1f;
    [Min(0f)] public float jumpCost = 2f;
    [Min(0f)] public float wallJumpCost = 0f;

    [Header("Combat Costs")]
    [Min(0f)] public float heavyAttackCost = 10f;
    [Min(0f)] public float knifeSpecialCost = 10f;
    [Min(0f)] public float hammerSpecialCost = 20f;

    public float CurrentStamina => currentStamina;

    private float lastSpendTime;
    private float regenTimer;
    private float sprintTimer;
    private float wallRunTimer;
    private bool exhausted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            return;

        Instance = this;
        currentStamina = maxStamina;
        lastSpendTime = Time.time;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Update()
    {
        if (currentStamina >= maxStamina)
        {
            regenTimer = 0f;
            return;
        }

        float delay = exhausted ? emptyRegenDelay : regenDelay;

        if (Time.time - lastSpendTime < delay)
            return;

        regenTimer += Time.deltaTime;
        float interval = Mathf.Max(0.001f, regenInterval);

        while (regenTimer >= interval && currentStamina < maxStamina)
        {
            currentStamina = Mathf.Min(
                maxStamina,
                currentStamina + regenAmount
            );

            regenTimer -= interval;
            exhausted = false;
        }
    }

    public bool CanSpend(float amount)
    {
        return amount <= 0f || currentStamina >= amount;
    }

    public bool TrySpend(float amount)
    {
        if (amount <= 0f)
            return true;

        if (!CanSpend(amount))
            return false;

        currentStamina = Mathf.Max(0f, currentStamina - amount);
        lastSpendTime = Time.time;
        regenTimer = 0f;
        exhausted = currentStamina <= 0f;

        return true;
    }

    public bool TryDrainSprint(float deltaTime)
    {
        return TryDrainTimed(
            ref sprintTimer,
            sprintCost,
            sprintInterval,
            deltaTime
        );
    }

    public bool TryDrainWallRun(float deltaTime)
    {
        return TryDrainTimed(
            ref wallRunTimer,
            wallRunCost,
            wallRunInterval,
            deltaTime
        );
    }

    public void ResetSprintDrain()
    {
        sprintTimer = 0f;
    }

    public void ResetWallRunDrain()
    {
        wallRunTimer = 0f;
    }

    private bool TryDrainTimed(
        ref float timer,
        float cost,
        float interval,
        float deltaTime)
    {
        if (cost <= 0f)
            return true;

        if (!CanSpend(cost))
        {
            timer = 0f;
            return false;
        }

        lastSpendTime = Time.time;
        regenTimer = 0f;

        timer += deltaTime;
        interval = Mathf.Max(0.001f, interval);

        while (timer >= interval)
        {
            if (!TrySpend(cost))
            {
                timer = 0f;
                return false;
            }

            timer -= interval;
        }

        return true;
    }
}
