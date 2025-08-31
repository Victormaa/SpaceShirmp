using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI_Shooter : MonoBehaviour
{
    public enum ShooterAIState
    {
        InToScene,       // 进场
        Idle,           // 待机
        Shooting,       // 射击
        Destroy         // 销毁
    }

    [Header("发射设置")]
    public GameObject bulletPrefab;      // 子弹预制体
    public Transform targetTransform;    // 目标（比如玩家）
    public Transform firePoint;          // 发射点

    [Header("时间设置")]
    public float minShootInterval = 5f;  // 最小发射间隔
    public float maxShootInterval = 8f;  // 最大发射间隔

    [Header("子弹参数")]
    public float bulletSpeed = 8f;       // 子弹速度

    void Start()
    {
        // 如果没有指定发射点，使用敌人自身位置
        if (firePoint == null)
            firePoint = transform;

        // 开始发射协程
        //StartCoroutine(ShootRoutine());
    }

    // 发射协程 - 每隔随机时间发射一次
    IEnumerator ShootRoutine()
    {
        while (true)
        {
            // 等待随机时间（5-8秒）
            float waitTime = Random.Range(minShootInterval, maxShootInterval);
            yield return new WaitForSeconds(waitTime);

            // 发射子弹
            ShootBullet();
        }
    }

    // 发射子弹的方法
    void ShootBullet()
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("子弹预制体未设置！");
            return;
        }

        if (targetTransform == null)
        {
            // 如果没有设置目标，尝试查找玩家
            targetTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (targetTransform == null)
            {
                Debug.LogWarning("没有找到目标！");
                return;
            }
        }

        // 创建子弹实例
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // 获取子弹脚本并设置参数
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.speed = bulletSpeed;
            bulletScript.target = targetTransform;
        }

        Debug.Log("发射子弹！目标: " + targetTransform.name);

        // 简单视觉效果
        StartCoroutine(FlashMuzzle());
        Debug.Log("发射子弹！目标: " + targetTransform.name);
    }
    // 简单的枪口闪光效果
    IEnumerator FlashMuzzle()
    {
        if (firePoint != null)
        {
            // 创建一个简单的闪光效果
            GameObject flash = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            flash.transform.position = firePoint.position;
            flash.transform.localScale = Vector3.one * 0.3f;
            flash.GetComponent<Collider>().enabled = false;

            // 0.1秒后消失
            yield return new WaitForSeconds(0.1f);
            Destroy(flash);
        }
    }

    // 可视化调试（在Scene视图中显示）
    void OnDrawGizmosSelected()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(firePoint.position, 0.2f);
        }
    }

}
