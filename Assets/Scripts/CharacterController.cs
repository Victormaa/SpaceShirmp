using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public bool isUseNewDir = false;
    public float moveSpeed = 5f;
    public Animator anim;

    private int idleCount = 0;
    private bool isAttacking = false; // 新增：攻击状态标志
    private bool isAvoiding = false;  // 新增：攻击状态标志
    private bool isFreezed = false;

    private Vector3 dodgeDirection = Vector3.zero;
    public AnimationCurve dodgeCurve;
    public float dodgeDistance = 3.0f;
    private float dodgeProcess = 0;
    public AnimationClip dodgeClip; // 在编辑器里把 Dodge 动画拖进来
    private float dodgeDuration;
    private Vector3 dodgeStartPosition;

    void Start()
    {
        if (dodgeClip != null)
        {
            dodgeDuration = dodgeClip.length;
        }
        dodgeDirection = Vector3.up;
    }

    public bool ISFREEZED { get { return isFreezed; } }
    void Update()
    {
        if (isFreezed)
            return;
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
        if (isAvoiding)
        {
            // 累加进度
            dodgeProcess += Time.deltaTime / dodgeDuration;
            float eval = dodgeCurve.Evaluate(Mathf.Clamp01(dodgeProcess));

            // 计算目标位置
            Vector3 targetPos = dodgeStartPosition + (dodgeDirection.normalized * eval * dodgeDistance);

            // 或者简单的 Transform 移动
            transform.position = targetPos;

            if (dodgeProcess >= 1f)
            {
                isAvoiding = false;
                dodgeProcess = 0;
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

        if (Input.GetMouseButtonUp(1))
        {
            Plane virtualPlane = new Plane(-Camera.main.transform.forward, transform.position);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float distanceToPlane;
            if (virtualPlane.Raycast(ray, out distanceToPlane))
            {
                Vector3 hitPoint = ray.GetPoint(distanceToPlane);
                Vector3 direction = hitPoint - transform.position;
                dodgeDirection = direction.normalized;
                anim.Play("Dodge");
                isAvoiding = true;
                dodgeProcess = 0;
                dodgeStartPosition = transform.position;
                return;
            }
        }

        // 获取输入
        float moveZ = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        // 检查是否有移动输入
        bool hasInput = Mathf.Abs(moveY) > 0.1f || Mathf.Abs(moveZ) > 0.1f;

        // 处理移动和动画
        if (hasInput)
        {
            idleCount = 0;
            
            Vector3 move = new Vector3(0, moveY, moveZ) * moveSpeed * Time.deltaTime;
            if (isUseNewDir)
                move = new Vector3(moveZ, moveY, 0) * moveSpeed * Time.deltaTime;

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
            }
        }
    }
    public void IsHit()
    {
        isFreezed = true;
    }
    public void UnFreezed()
    {
        isFreezed = false;
    }
    bool IsAnimComplete(string animName)
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animName) && stateInfo.normalizedTime >= 1f;
    }
    // 更好的方法：使用Animation Event在攻击动画最后一帧调用这个方法
    public void OnAttackComplete()
    {
        isAttacking = false;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision != null && collision.transform.tag == "Collider")
        {

        }
    }
}