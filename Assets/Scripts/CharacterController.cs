using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float moveSpeed = 5f; // 移动速度

    void Update()
    {
        // 获取输入
        float moveZ = Input.GetAxis("Horizontal"); // A/D 控制 Z
        float moveY = Input.GetAxis("Vertical");   // W/S 控制 Y

        // 组合移动向量 (X=0, Y由WS控制, Z由AD控制)
        Vector3 move = new Vector3(0, moveY, moveZ) * moveSpeed * Time.deltaTime;

        // 应用移动
        transform.Translate(move, Space.World);
    }
}
