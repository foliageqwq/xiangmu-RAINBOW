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

    //����������
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

    //��ײ�����
    private void CollisionChecks()
    {
        //���ݽӵ����߸ı��ɫ�ӵ�״̬
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
    }

    //��������
    private void CheckInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        //����
        if (Input.GetKeyDown(KeyCode.J))
        {
            StartAttackEvent();
        }
        //��Ծ
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        //���
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            DashAbility();
        }
    }

    //��������
    private void StartAttackEvent()
    {
        //�����ɫ���ӵػ��ɫ�ڳ��״̬�У���ֹ����
        if(!isGrounded ||dashTime > 0)
        {
            return;
        }
        //���������ʱ��<0��������������
        if (comboTimeWindow < 0)
        {
            comboCounter = 0;
        }

        isAttacking = true;
        comboTimeWindow = comboTime;
    }

    //��������֡�¼�
    public void AttackOver()
    {
        isAttacking = false;

        comboCounter++;
        //��������������������������������
        if (comboCounter > 2)
        {
            comboCounter = 0;
        }
    }

    //��Ծ����
    private void DashAbility()
    {
        //�����Ծ����������ȴ״̬�ҽ�ɫ���ڹ���״̬�п��Խ��г��
        if (dashCoolDownTimer < 0 && !isAttacking)
        {
            dashCoolDownTimer = dashCoolDown;
            dashTime = dashDuratiuon;
        }
    }

    //�ƶ�������
    private void Movenment()
    {
        //�����ɫ�ڽӵ�����·���������ֹͣ�ƶ�
        if(isAttacking && isGrounded)
        {
            rb.velocity = new Vector2(0, 0);
        }
        //�����ɫ��̣��ı�y���ٶ�Ϊ������*1*����ٶ�
        else if (dashTime > 0)
        {
            rb.velocity = new Vector2(facingDir * dashSpeed, 0);
        }
        //�����ɫ�ƶ�����ɫx���ٶ�=���뷽��*1*�ƶ��ٶ�
        else
        {
            rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
        }
    }

    //��Ծ����
    private void Jump()
    {
        //�����ɫ�ӵأ��ı�y���ٶ�ʵ����Ծ
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    //��ת������
    private void FlipController()
    {
        //���ݽ�ɫ��x��ķ���ı��ɫ����
        if (rb.velocity.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (rb.velocity.x < 0 && isFacingRight)
        {
            Flip();
        }
    }
    //��ת����
    private void Flip()
    {
        facingDir = facingDir * -1;
        isFacingRight = !isFacingRight;
        transform.Rotate(0, 180, 0);
    }

    //���߽ӵؼ��
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - groundCheckDistance));
    }
}
