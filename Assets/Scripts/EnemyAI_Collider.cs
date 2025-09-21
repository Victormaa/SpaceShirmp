using UnityEngine;
using System.Collections;
public enum AIState
{
    Idle,       // 空闲等待
    Charging,   // 准备冲刺（可选）
    Dashing,    // 冲刺中
    Braking     // 刹车中
}
// 1. 入场 - 待机 - 冲刺 - 冲出场外 Destory()
// 2. 入场 - 待机 - 冲刺 - 击中虾 游戏结束() - 

public class EnemyAI_Collider : MonoBehaviour
{
    [Header("冲刺设置")]
    public float dashSpeed = 15f;        // 冲刺速度
    public float brakeForce = 25f;       // 刹车力度
    public float minDashInterval = 5f;   // 最小冲刺间隔
    public float maxDashInterval = 8f;   // 最大冲刺间隔

    [Header("目标设置")]
    public Transform target;             // 目标点（可以是玩家）
    public float targetDistance = 10f;   // 冲刺目标距离

    private Rigidbody rb;
    private AIState currentState = AIState.Idle;
    private Vector3 dashDirection;       // 冲刺方向
    private Vector3 targetPosition;      // 目标位置

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (target == null)
        {
            // 如果没有指定目标，可以在这里设置默认目标
            // target = GameObject.FindGameObjectWithTag("Player").transform;
        }

        StartCoroutine(DashRoutine());
    }

    void Update()
    {
        switch (currentState)
        {
            case AIState.Dashing:
                CheckDashCompletion();
                break;

            case AIState.Braking:
                ApplyBraking();
                break;
        }
    }

    // 冲刺协程
    IEnumerator DashRoutine()
    {
        while (true)
        {
            // 等待随机时间
            float waitTime = Random.Range(minDashInterval, maxDashInterval);
            yield return new WaitForSeconds(waitTime);

            // 开始冲刺
            StartDash();
        }
    }

    // 开始冲刺
    void StartDash()
    {
        if (currentState != AIState.Idle) return;

        target = GameObject.FindGameObjectWithTag("Player").transform;

        // 计算冲刺方向
        if (target != null)
        {
            // 朝目标方向冲刺
            dashDirection = (target.position - transform.position).normalized;
            targetDistance = (target.position - transform.position).magnitude;
            targetPosition = transform.position + dashDirection * targetDistance;
        }
        else
        {
            // 随机方向冲刺（如果没有目标）
            dashDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            targetDistance = (target.position - transform.position).magnitude;
            targetPosition = transform.position + dashDirection * targetDistance;
        }

        // 设置冲刺速度
        rb.velocity = dashDirection * dashSpeed;
        currentState = AIState.Dashing;

        Debug.Log("开始冲刺！方向: " + dashDirection);
    }

    // 检查冲刺是否完成（是否冲过目标点）
    void CheckDashCompletion()
    {
        // 计算当前位置到目标点的向量
        Vector3 toTarget = targetPosition - transform.position;

        // 如果已经冲过目标点（点积为负表示方向相反）
        if (Vector3.Dot(toTarget, dashDirection) < 0)
        {
            Destroy(gameObject, 3);
            StartBraking();
        }
    }

    // 开始刹车
    void StartBraking()
    {
        //currentState = AIState.Braking;
    }

    // 应用刹车力
    void ApplyBraking()
    {
        // 施加反向力来刹车
        rb.AddForce(-rb.velocity.normalized * brakeForce, ForceMode.Acceleration);

        // 检查速度是否足够小，可以回到空闲状态
        if (rb.velocity.magnitude < 1f)
        {
            rb.velocity = Vector3.zero;
            currentState = AIState.Idle;
            Debug.Log("刹车完成，回到空闲状态");
        }
    }

    // 可视化调试（在Scene视图中显示）
    void OnDrawGizmos()
    {
        if (Application.isPlaying && currentState == AIState.Dashing)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, targetPosition);
            Gizmos.DrawWireSphere(targetPosition, 0.5f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, rb.velocity.normalized * 2f);
        }
    }

    // 可选：添加物理碰撞检测
    void OnCollisionEnter(Collision collision)
    {
        // 如果冲刺中撞到东西，提前开始刹车
        if (currentState == AIState.Dashing)
        {
            StartBraking();
        }
    }
}