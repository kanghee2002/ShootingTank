using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUtility : PlayerUtility
{
    [Header("Move")]
    [SerializeField] private int jumpCountBonus;
    [SerializeField] private float jumpPowerBonus;
    [SerializeField] private float moveSpeedBonus;
    [SerializeField] private float downFallCoolTimeBonus;

    [Header("Core")]
    [SerializeField] private float coreTakeDamageMultiplierBonus;

    [Header("Health")]
    [SerializeField] private float healPercentage;
    [SerializeField] private float maxHealthBonusPercentage;

    [Header("Etc")]
    [SerializeField] private float playerScaleBonus;
    [SerializeField] private bool canJumpInfinitely;
    [SerializeField] private bool canSetCoreInactive;

    private PlayerController playerController;
    private Health health;
    private CoreHealth coreHealth;

    private void Awake()
    {
        playerController = GameManager.Instance.playerObject.GetComponent<PlayerController>();
        health = GameManager.Instance.playerObject.GetComponent<Health>();
        coreHealth = GameManager.Instance.playerObject.GetComponentInChildren<CoreHealth>();
    }

    public override void GetAbility()
    {
        playerController.AddMaxJumpCount(jumpCountBonus);

        playerController.AddJumpPowerValue(jumpPowerBonus);

        playerController.AddMoveSpeedValue(moveSpeedBonus);

        playerController.MinusDownFallCoolTime(downFallCoolTimeBonus);

        coreHealth.MinusCoreDamageMultiplier(coreTakeDamageMultiplierBonus);

        health.HealByPercentage(healPercentage);

        health.IncreaseMaxHealthByPercentage(maxHealthBonusPercentage);

        if (playerScaleBonus > 0f)
        {
            ReducePlayerScale(playerScaleBonus);
        }

        playerController.AllowInfiniteJump(canJumpInfinitely);

        if (canSetCoreInactive)
        {
            coreHealth.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        /*playerController.AddMaxJumpCount(jumpCountBonus);

        playerController.AddJumpPowerValue(jumpPowerBonus);

        playerController.AddMoveSpeedValue(moveSpeedBonus);

        playerController.MinusDownFallCoolTime(downFallCoolTimeBonus);

        coreHealth.MinusCoreDamageMultiplier(coreTakeDamageMultiplierBonus);

        health.HealByPercentage(healPercentage);

        health.IncreaseMaxHealthByPercentage(maxHealthBonusPercentage);

        if (playerScaleBonus > 0f)
        {
            ReducePlayerScale(playerScaleBonus);
        }

        playerController.AllowInfiniteJump(canJumpInfinitely);

        if (canSetCoreInactive)
        {
            coreHealth.gameObject.SetActive(false);
        }*/
    }

    private void OnDisable()
    {
        /*playerController.AddMaxJumpCount(-jumpCountBonus);

        playerController.AddJumpPowerValue(-jumpPowerBonus);

        playerController.AddMoveSpeedValue(-moveSpeedBonus);

        playerController.MinusDownFallCoolTime(-downFallCoolTimeBonus);

        coreHealth.MinusCoreDamageMultiplier(-coreTakeDamageMultiplierBonus);

        health.IncreaseMaxHealthByPercentage(-maxHealthBonusPercentage);

        if (playerScaleBonus > 0f)
        {
            ReducePlayerScale(-playerScaleBonus);
        }

        playerController.AllowInfiniteJump(!canJumpInfinitely);

        if (canSetCoreInactive)
        {
            coreHealth.gameObject.SetActive(true);
        }*/
    }

    private void ReducePlayerScale(float playerScaleBonus)
    {
        Transform playerTransform = GameManager.Instance.playerObject.transform;

        float currentPlayerScale = Mathf.Abs(playerTransform.localScale.x);

        float newPlayerScale = currentPlayerScale - playerScaleBonus;

        if (playerTransform.localScale.x > 0f)
        {
            playerTransform.localScale = new Vector2(newPlayerScale, newPlayerScale);
        }
        else
        {
            playerTransform.localScale = new Vector2(-newPlayerScale, newPlayerScale);
        }
    }
}
