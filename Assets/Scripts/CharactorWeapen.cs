using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorWeapen : MonoBehaviour
{
    public GameObject bulletPrefab;
    public void ShootBullet()
    {
        // 在这里实现攻击逻辑（例如生成子弹）
        var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().AddForce(Vector3.right, ForceMode.Impulse);


        Debug.Log("ShootBullet called at frame: " + Time.frameCount);
    }
}
