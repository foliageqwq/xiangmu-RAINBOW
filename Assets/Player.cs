using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;

    [Header("Dash info")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuratiuon;
    [SerializeField] private float dashCoolDown;
    private float dashTime;
    private float dashCoolDownTimer;

    [Header("Attack info")]
    [SerializeField] private float comboTime = .3f;
    [SerializeField] private float comboTimeWindow;
    private bool isAttacking;
    private int comboCounter;

    private float xInput;
    private int facingDir = 1;
    private bool isFacingRight = true;

    [Header("Collision info")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    private bool isGrounded;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        Movenment();
        CheckInput();
        CollisionChecks();

        dashTime -= Time.deltaTime;
        dashCoolDownTimer -= Time.deltaTime;
        comboTimeWindow -= Time.deltaTime;

        FlipController();
        AnimatorControllers();
    }

    //动画控制器
    private void AnimatorControllers()
    {
        bool isMoving = rb.velocity.x != 0;

        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isDashing", dashTime > 0);
        anim.SetBool("isAttacking", isAttacking);
        anim.SetInteger("comboCounter", comboCounter);
    }

    //碰撞检测器
    private void CollisionChecks()
    {
        //根据接地射线改变角色接地状态
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
    }

    //输入检测器
    private void CheckInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        //攻击
        if (Input.GetKeyDown(KeyCode.J))
        {
            StartAttackEvent();
        }
        //跳跃
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        //冲刺
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            DashAbility();
        }
    }

    //攻击动作
    private void StartAttackEvent()
    {
        //如果角色不接地或角色在冲刺状态中，禁止攻击
        if(!isGrounded ||dashTime > 0)
        {
            return;
        }
        //如果连击计时器<0，连击计数重置
        if (comboTimeWindow < 0)
        {
            comboCounter = 0;
        }

        isAttacking = true;
        comboTimeWindow = comboTime;
    }

    //攻击结束帧事件
    public void AttackOver()
    {
        isAttacking = false;

        comboCounter++;
        //如果到达第三段连击，重置连击计数器
        if (comboCounter > 2)
        {
            comboCounter = 0;
        }
    }

    //跳跃动作
    private void DashAbility()
    {
        //如果跳跃动作不在冷却状态且角色不在攻击状态中可以进行冲刺
        if (dashCoolDownTimer < 0 && !isAttacking)
        {
            dashCoolDownTimer = dashCoolDown;
            dashTime = dashDuratiuon;
        }
    }

    //移动控制器
    private void Movenment()
    {
        //如果角色在接地情况下发动攻击，停止移动
        if(isAttacking && isGrounded)
        {
            rb.velocity = new Vector2(0, 0);
        }
        //如果角色冲刺，改变y轴速度为朝向面*1*冲刺速度
        else if (dashTime > 0)
        {
            rb.velocity = new Vector2(facingDir * dashSpeed, 0);
        }
        //如果角色移动，角色x轴速度=输入方向*1*移动速度
        else
        {
            rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
        }
    }

    //跳跃动作
    private void Jump()
    {
        //如果角色接地，改变y轴速度实现跳跃
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    //翻转控制器
    private void FlipController()
    {
        //根据角色在x轴的方向改变角色朝向
        if (rb.velocity.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (rb.velocity.x < 0 && isFacingRight)
        {
            Flip();
        }
    }
    //翻转函数
    private void Flip()
    {
        facingDir = facingDir * -1;
        isFacingRight = !isFacingRight;
        transform.Rotate(0, 180, 0);
    }

    //射线接地检测
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - groundCheckDistance));
    }
}
