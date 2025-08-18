using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Animator anim;

    private int idleCount = 0;
    private bool isAttacking = false; // 新增：攻击状态标志

    public GameObject bulletPrefab;

    void Update()
    {
        // 如果正在攻击，则直接返回，不处理移动输入
        if (isAttacking)
        {
            // 可以在这里添加攻击动画是否完成的检测
            if (IsAnimComplete("Attack"))
            {
                isAttacking = false;
            }
            return;
        }

        // 攻击输入检测（只有在非攻击状态时才能触发）
        if (Input.GetMouseButtonDown(0))
        {
            anim.Play("Attack");
            isAttacking = true;
            return; // 立即返回，确保攻击动画开始时就不处理移动
        }

        // 获取输入
        float moveZ = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        // 检查是否有移动输入
        bool hasInput = Mathf.Abs(moveY) > 0.1f || Mathf.Abs(moveZ) > 0.1f;

        // 处理移动和动画
        if (hasInput)
        {
            //isMoving = true;
            //idleTimer = 0f;
            idleCount = 0;

            Vector3 move = new Vector3(0, moveY, moveZ) * moveSpeed * Time.deltaTime;
            transform.Translate(move, Space.World);

            if (moveY > 0.1f || moveZ > 0.1f)
            {
                anim.Play("MoveForward");
            }
            else if (moveY < -0.1f || moveZ < -0.1f)
            {
                anim.Play("MoveBack");
            }
        }
        else
        {
            //if (idleCount >= 15)
            //{
            //    anim.Play("Idle2");
            //    if (IsAnimComplete("Idle2"))
            //        idleCount = 0;
            //}
            //else
            //{
            //    anim.Play("Idle1");
            //    if (IsAnimComplete("Idle1"))
            //        idleCount++;
            //}
            // 如果当前没有播放动画或者上一个动画已经播放完成
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Idle1") &&
                !anim.GetCurrentAnimatorStateInfo(0).IsName("Idle2"))
            {
                if (idleCount < 3) // 播放3次Idle1
                {
                    anim.Play("Idle1");
                }
                else // 第4次播放Idle2
                {
                    anim.Play("Idle2");
                    idleCount = 0; // 重置计数器
                }
            }

            // 检测动画是否完成
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle1") &&
                IsAnimComplete("Idle1"))
            {
                idleCount++;
            }
            else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle2") &&
                     IsAnimComplete("Idle2"))
            {
                // Idle2播放完成后不需要额外处理，计数器已在上面重置
            }
        }
    }

    bool IsAnimComplete(string animName)
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animName) && stateInfo.normalizedTime >= 1f;
    }

    public void ShootTheShrimpShell()
    {
        Instantiate(bulletPrefab,transform.position,Quaternion.identity);
    }

    // 更好的方法：使用Animation Event在攻击动画最后一帧调用这个方法
    public void OnAttackComplete()
    {
        isAttacking = false;
    }
}