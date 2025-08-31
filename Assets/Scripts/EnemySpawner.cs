using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("生成设置")]
    public GameObject enemyPrefab;        // 敌人预制体
    public Transform playerTransform;     // 玩家Transform
    public Transform forwarTarget;
    public float spawnInterval = 3f;      // 生成间隔（秒）
    public int maxEnemies = 10;           // 最大敌人数

    [Header("生成范围")]
    public float forwardDistance = 2.5f;  // 玩家前方距离
    public float verticalRange = 0.5f;    // 垂直方向范围（上下各0.5m）
    public float horizontalRange = 1f;    // 水平方向范围

    [Header("调试")]
    public bool showGizmos = true;        // 显示调试范围

    private int currentEnemyCount = 0;

    void Start()
    {
        // 如果没有指定玩家，自动查找
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (playerTransform == null)
            {
                Debug.LogError("没有找到玩家！");
                return;
            }
        }

        // 开始生成协程
        StartCoroutine(SpawnRoutine());
    }

    // 生成协程
    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // 检查是否达到最大敌人数
            if (currentEnemyCount < maxEnemies)
            {
                SpawnEnemy();
            }
        }
    }

    // 生成敌人
    void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("敌人预制体未设置！");
            return;
        }

        // 计算生成位置
        Vector3 spawnPosition = CalculateSpawnPosition();

        // 创建敌人实例
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        // 设置敌人面向玩家
        if (playerTransform != null)
        {
            enemy.transform.eulerAngles = new Vector3(0, -90, 0);
        }

        currentEnemyCount++;
        Debug.Log($"生成敌人！当前位置: {spawnPosition}, 总数: {currentEnemyCount}");
    }

    // 计算生成位置（在玩家前方指定范围内）
    Vector3 CalculateSpawnPosition()
    {
        if (playerTransform == null)
            return Vector3.zero;

        var playerForward = (forwarTarget.position - playerTransform.position).normalized;
        // 玩家前方基准点
        Vector3 basePosition = playerTransform.position + playerForward * forwardDistance;

        // 随机偏移（垂直和水平方向）
        float randomY = Random.Range(-verticalRange, verticalRange);
        float randomX = Random.Range(-horizontalRange, horizontalRange);

        // 计算最终位置
        Vector3 spawnPosition = basePosition +
                               playerTransform.up * randomY +
                               playerForward * randomX;

        return spawnPosition;
    }

    // 敌人死亡回调
    void OnEnemyDied()
    {
        currentEnemyCount--;
        Debug.Log("敌人死亡，当前数量: " + currentEnemyCount);
    }

    // 可视化调试
    void OnDrawGizmos()
    {
        if (!showGizmos || playerTransform == null) return;

        Gizmos.color = Color.red;

        // 绘制生成区域
        Vector3 center = playerTransform.position + playerTransform.forward * forwardDistance;

        // 绘制前方基准点
        Gizmos.DrawWireSphere(center, 0.1f);
        Gizmos.DrawLine(playerTransform.position, center);

        // 绘制生成范围框
        Vector3 size = new Vector3(horizontalRange * 2, verticalRange * 2, 0.1f);
        Gizmos.matrix = Matrix4x4.TRS(center, playerTransform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, size);
    }

    // 手动生成敌人（用于测试）
    [ContextMenu("手动生成敌人")]
    public void ManualSpawnEnemy()
    {
        SpawnEnemy();
    }

    // 清空所有敌人（用于测试）
    [ContextMenu("清空所有敌人")]
    public void ClearAllEnemies()
    {
        EnemyAI_Collider[] enemies = FindObjectsOfType<EnemyAI_Collider>();
        foreach (EnemyAI_Collider enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
        EnemyAI_Shooter[] enemies2 = FindObjectsOfType<EnemyAI_Shooter>();
        foreach (EnemyAI_Shooter enemy in enemies2)
        {
            Destroy(enemy.gameObject);
        }
        currentEnemyCount = 0;
        Debug.Log("已清空所有敌人");
    }
}