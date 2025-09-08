using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorWeapen : MonoBehaviour
{
    public GameObject bulletPrefab;
    public ParticleSystem particle;
    
    public void ShootBullet()
    {
        // 在这里实现攻击逻辑（例如生成子弹）
        var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().AddForce(Vector3.right, ForceMode.Impulse);

        if(!particle.isPlaying)
            particle.Play();

        Debug.Log("ShootBullet called at frame: " + Time.frameCount);
        //Debug.Break();
        Destroy(bullet, 5.5f);
    }
}
