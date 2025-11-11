using System.Collections;
using UnityEngine;

public class EnemyAI_Shooter : MonoBehaviour
{
    public enum ShooterAIState
    {
        IDLE,       // 进场、待机
        FOLLOW,     // 移动、追踪玩家
        ATTACK,     // 射击
        DEAD        // 死亡、销毁
    }

    [Header("状态设置")]
    public ShooterAIState currentState = ShooterAIState.IDLE; // 当前状态
    public float followRange = 10f;    // 开始追踪玩家的距离
    public float attackRange = 7f;     // 开始攻击的距离

    [Header("发射设置")]
    public GameObject bulletPrefab;      // 子弹预制体
    public Transform targetTransform;    // 目标（比如玩家）
    public Transform firePoint;          // 发射点

    [Header("时间设置")]
    public float minShootInterval = 5f;  // 最小发射间隔
    public float maxShootInterval = 8f;  // 最大发射间隔

    [Header("移动设置")]
    public float moveSpeed = 0.03f;         // 移动速度

    [Header("子弹参数")]
    public float bulletSpeed = 8f;       // 子弹速度

    // 内部变量
    private Coroutine shootCoroutine;    // 存储射击协程的引用

    public float originalZ;

    void Start()
    {
        originalZ = this.transform.position.z;

        // 初始化
        if (firePoint == null)
            firePoint = transform;


        // 初始状态设置为IDLE
        ChangeState(ShooterAIState.IDLE);
    }
    void Update()
    {
        // 如果没有目标，尝试查找玩家
        if (targetTransform == null)
        {
            targetTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (targetTransform == null) return; // 如果还是找不到，跳过这一帧
        }

        // 计算与目标的距离
        float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);

        // 状态机逻辑
        switch (currentState)
        {
            case ShooterAIState.IDLE:
                // IDLE状态逻辑：检查是否应该开始追踪或攻击
                if (distanceToTarget <= followRange)
                {
                    // 玩家进入追踪范围，切换到FOLLOW状态
                    ChangeState(ShooterAIState.FOLLOW);
                }
                break;

            case ShooterAIState.FOLLOW:
                // FOLLOW状态逻辑：移动朝向玩家，并在合适的时候攻击
                if (distanceToTarget <= attackRange)
                {
                    // 玩家进入攻击范围，切换到ATTACK状态
                    ChangeState(ShooterAIState.ATTACK);
                }
                else if (distanceToTarget > followRange)
                {
                    // 玩家超出追踪范围，返回IDLE状态
                    ChangeState(ShooterAIState.IDLE);
                }
                else
                {
                    // 追踪玩家
                    MoveTowardsTarget();
                    FaceTarget(); // 在移动时也面向目标
                }
                break;

            case ShooterAIState.ATTACK:
                // ATTACK状态逻辑：攻击玩家，并在玩家逃跑时追踪
                if (distanceToTarget > attackRange * 1.2f) // 添加缓冲范围，避免频繁切换
                {
                    // 玩家超出攻击范围+缓冲，切换回FOLLOW状态
                    ChangeState(ShooterAIState.FOLLOW);
                }
                else if (distanceToTarget > followRange)
                {
                    // 如果玩家跑出追踪范围，直接回到IDLE
                    ChangeState(ShooterAIState.IDLE);
                }
                else
                {
                    // 注意：射击由协程处理，这里只需要面向玩家
                    FaceTarget();
                }
                break;

            case ShooterAIState.DEAD:
                // DEAD状态逻辑：通常不需要做任何事情，或者播放死亡动画
                break;
        }
        this.transform.position = new Vector3(transform.position.x, transform.position.y, originalZ);
    }
    // 改变状态的方法
    void ChangeState(ShooterAIState newState)
    {
        // 退出当前状态
        ExitState(currentState);

        // 设置新状态
        currentState = newState;
        Debug.Log("Enemy state changed to: " + newState);

        // 进入新状态
        EnterState(newState);
    }
    // 进入新状态时的处理
    void EnterState(ShooterAIState newState)
    {
        switch (newState)
        {
            case ShooterAIState.IDLE:
                // 停止射击协程
                if (shootCoroutine != null)
                {
                    StopCoroutine(shootCoroutine);
                    shootCoroutine = null;
                }
                break;
            case ShooterAIState.FOLLOW:
                break;
            case ShooterAIState.ATTACK:
                // 启动射击协程
                if (shootCoroutine == null)
                    shootCoroutine = StartCoroutine(ShootRoutine());
                break;
            case ShooterAIState.DEAD:
                // 停止所有协程
                if (shootCoroutine != null)
                    StopCoroutine(shootCoroutine);
                // 播放死亡动画，销毁对象等
                // StartCoroutine(DieRoutine());
                break;
        }
    }
    // 退出当前状态时的处理
    void ExitState(ShooterAIState oldState)
    {
        switch (oldState)
        {
            case ShooterAIState.ATTACK:
                //// 停止射击协程
                //if (shootCoroutine != null)
                //{
                //    StopCoroutine(shootCoroutine);
                //    shootCoroutine = null;
                //}
                break;
        }
    }
    // 移动朝向目标
    void MoveTowardsTarget()
    {
        if (targetTransform == null) return;

        // 使用Lerp实现平滑移动
        Vector3 targetPosition = new Vector3(
            targetTransform.position.x,
            targetTransform.position.y,
            transform.position.z // 保持当前Y轴高度          
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // 同时面向目标
        FaceTarget();
    }
    // 面向目标
    void FaceTarget()
    {
        if (targetTransform == null) return;

        Vector3 direction = (targetTransform.position - transform.position).normalized;
        direction.y = 0; // 保持水平旋转
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
    // 发射协程 - 只在ATTACK状态下运行
    IEnumerator ShootRoutine()
    {
        // 等待随机时间
        float waitTime = Random.Range(minShootInterval, maxShootInterval);
        yield return new WaitForSeconds(waitTime);
        // 如果仍在攻击状态，发射子弹
        if(!targetTransform.GetComponent<CharacterController>().ISFREEZED)
            ShootBullet();
        
        shootCoroutine = StartCoroutine(ShootRoutine());
    }
    // 发射子弹的方法
    void ShootBullet()
    {
        if (bulletPrefab == null || targetTransform == null) return;

        // 创建子弹实例
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // 获取子弹脚本并设置参数
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.speed = bulletSpeed;
            bulletScript.target = targetTransform;
        }
    }
    // 简单的枪口闪光效果
    IEnumerator FlashMuzzle()
    {
        if (firePoint != null)
        {
            GameObject flash = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            flash.transform.position = firePoint.position;
            flash.transform.localScale = Vector3.one * 0.3f;
            flash.GetComponent<Collider>().enabled = false;

            yield return new WaitForSeconds(0.1f);
            Destroy(flash);
        }
    }
    // 可视化调试
    void OnDrawGizmosSelected()
    {
        // 显示攻击和追踪范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, followRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // 显示发射点
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(firePoint.position, 0.02f);
        }
    }
    // 示例：被攻击时调用
    public void TakeDamage(int damage)
    {
        // 减少生命值等逻辑...
        // 如果生命值 <= 0，切换到DEAD状态
        // ChangeState(ShooterAIState.DEAD);
    }
}