using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;

public class TackleEnemy : Enemy
{
    [Header("Attack Settings")]
    [SerializeField] private float warningLaserSpeed;

    [SerializeField] private float warningLaserWidth;

    [SerializeField] private float tackleSpeed;

    [Header("Warning Laser Settings")]
    [SerializeField] private WarningLaser warningLaser;

    [SerializeField] private LayerMask warningLaserBlockingLayer;

    private PolygonCollider2D polygonCollider;
    private Rigidbody2D rigid;
    private EnemyController enemyController;
    private JumpChecker jumpChecker;

    private bool isTackling = false;
    private bool hasTackled = false;

    protected override void Awake()
    {
        base.Awake();
        rigid = GetComponent<Rigidbody2D>();
        polygonCollider = GetComponent<PolygonCollider2D>();
        enemyController = GetComponent<EnemyController>();

        jumpChecker = GetComponentInChildren<JumpChecker>();
    }

    public override bool Attack(Transform playerTransform)
    {
        if (isCool)
        {
            return false;
        }

        StartCoroutine(TackleRoutine(playerTransform));

        enemyController.PauseMove(coolTime);

        StartCoroutine(CoolDownRoutine(coolTime));
        isCool = true;

        return true;
    }

    private IEnumerator TackleRoutine(Transform playerTransform)
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;

        float laserDistance = 100f;

        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, direction, laserDistance, warningLaserBlockingLayer);

        Vector2 tacklePosition = (Vector2)transform.position + direction * laserDistance;

        if (rayHit)
        {
            tacklePosition = rayHit.point;
        }

        float tackleDistance = (tacklePosition - (Vector2)transform.position).magnitude;

        warningLaser.SetLaserWidth(warningLaserWidth);

        warningLaser.StartStretch(transform.position, tacklePosition, warningLaserSpeed);

        float warningTime = tackleDistance / warningLaserSpeed;

        yield return new WaitForSeconds(warningTime);

        #region Rush to Player

        RigidbodyType2D originalBodyType = rigid.bodyType;
        rigid.bodyType = RigidbodyType2D.Kinematic;
        polygonCollider.isTrigger = true;
        isTackling = true;
        hasTackled = false;

        warningLaser.SetLaserWidth(0f);

        rigid.velocity = direction * tackleSpeed;

        float safeOffset = 1f;

        yield return new WaitForSeconds((tackleDistance - safeOffset) / tackleSpeed);

        rigid.velocity = Vector2.zero;

        rigid.bodyType = originalBodyType;
        polygonCollider.isTrigger = false;
        isTackling = false;

        #endregion
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Settings.playerTag))
        {
            bool isPlayer = collision.transform.TryGetComponent(out PlayerController playerController);

            if (!isPlayer | !isTackling | hasTackled)
            {
                return;
            }

            hasTackled = true;

            float horizontalVelocity = Mathf.Abs(rigid.velocity.x);
            float verticalVelocity = Mathf.Abs(rigid.velocity.y);

            Vector2 direction;

            float tacklePower = 0.002f;

            if (horizontalVelocity > verticalVelocity)
            {
                if (rigid.velocity.x > 0) direction = new Vector2(1f, 1f);
                else direction = new Vector2(-1f, 1f);
            }
            else
            {
                float xGap = (collision.transform.position - transform.position).x;

                if (xGap > 0f) direction = new Vector2(1f, 1f);
                else direction = new Vector2(-1f, 1f);
            }

            playerController.KnockBack(direction * tacklePower, 0.5f);

            collision.transform.GetComponent<Health>().TakeDamage(damageValue);
        }
    }
}
