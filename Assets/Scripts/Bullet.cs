using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 6f;        // 子弹速度
    public Transform target;         // 目标
    public float lifeTime = 5f;      // 子弹存活时间

    private Vector3 moveDirection;   // 移动方向
    private bool hasHitTarget = false; // 是否击中了目标
    private Transform hitTarget;      // 击中的目标


    void Start()
    {
        Destroy(gameObject, lifeTime);

        if (target != null)
        {
            moveDirection = (target.position - transform.position).normalized;
        }
    }

    void Update()
    {
        if (!hasHitTarget)
        {
            // 正常移动阶段
            transform.position += moveDirection * speed * Time.deltaTime;
        }
        else if (hitTarget != null)
        {
            // 击中后阶段：移动到目标中心
            MoveToTargetCenter();
        }
    }

    // 移动到目标中心
    void MoveToTargetCenter()
    {
        // 平滑移动到目标中心
        Vector3 targetCenter = hitTarget.Find("BubbleTargetPos").position;//hitTarget.position;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetCenter,
            speed * 2f * Time.deltaTime // 移动速度可以更快一些
        );

        // 如果已经到达目标中心，可以执行其他操作
        if (Vector3.Distance(transform.position, targetCenter) < 0.1f)
        {
            OnReachedTargetCenter();
        }
    }

    // 到达目标中心后的处理
    void OnReachedTargetCenter()
    {
        // 这里可以添加到达中心后的效果
        // 比如：播放特效、对目标造成影响等
        Debug.Log("子弹到达目标中心！");

        // 可选：销毁子弹或保持在那里
        // Destroy(gameObject);
    }

    // 触发检测r
    void OnTriggerEnter(Collider other)
    {
        // 检查是否击中了玩家（可以根据tag或layer更精确地检测）
        if (!hasHitTarget && other.CompareTag("Player"))
        {
            HitTarget(other.transform);
        }
    }

    // 击中目标处理
    void HitTarget(Transform targetTransform)
    {
        hasHitTarget = true;
        hitTarget = targetTransform;

        // 停止正常移动，开始向目标中心移动
        Debug.Log("击中玩家！开始向中心移动");

        // 可选：禁用碰撞器，避免重复触发
        Collider bulletCollider = GetComponent<Collider>();
        if (bulletCollider != null)
        {
            bulletCollider.enabled = false;
        }

        targetTransform.GetComponent<CharacterController>()?.IsHit();
    }

    // 初始化子弹
    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
        if (target != null)
        {
            moveDirection = (target.position - transform.position).normalized;
        }
    }

    private void OnDestroy()
    {
        if (hitTarget != null)
        {
            hitTarget.GetComponent<CharacterController>().UnFreezed();
        }
    }
}