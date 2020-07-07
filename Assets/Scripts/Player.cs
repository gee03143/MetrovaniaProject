using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : PhysicsObject
{
    [SerializeField]
    private string defaultAnimName;

    private bool facingright = true;

    public float health = 5f;
    public float maxSpeed;
    public float attackLength;
    public int attackDamage;
    public float jumpTakeOffSpeed = 15f;

    [SerializeField]
    private float dashSpeed = 1f;

    public bool canMove = true;
    public bool waitingNextInput = false;      //다음 공격이 가능한지를 나타내는 트리거
    public bool doNextAttack = false;   //다음 공격을 할 경우 true

    public SpriteRenderer spriteRenderer { get; private set; }
    public Animator anim { get; private set;}
    private Vector2 move;

    //for InputInterface
    private List<InputInterface> inputButtons = new List<InputInterface>();

    //for stateMachine
    public StateMachine<Player> stateMachine { get; private set; }
    public Dictionary<PlayerState, IState<Player>> dicState { get; private set; } = new Dictionary<PlayerState, IState<Player>>();

    //for bow attack
    public GameObject arrowPrefab;
    public Transform firepoint;

    //for getDamaged
    [HideInInspector]
    public float launchDirection;
    public float launchDistance = 3f;
    public float launchTime = 2f;
    public float launchPower = 5f;
    public float undamagableTime = 3f;


    private static Player instance;
    public static Player Instance
    {
        get
        {
            if (instance == null) instance = GameObject.FindObjectOfType<Player>();
            return instance;
        }
    }

    private void Start()
    {
        //firepoint 초기화
        firepoint = transform.GetChild(0).transform;

        //statemachine 초기화
        IState<Player> run = new StateRun();
        IState<Player> attack = new StateAttack();
        IState<Player> jump = new StateJump();
        IState<Player> dash = new StateDash();
        IState<Player> dashAttack = new StateDashAttack();
        IState<Player> fall = new StateFall();
        IState<Player> shoot = new StateShoot();
        IState<Player> damaged = new StateDamaged();

        dicState.Add(PlayerState.Run, run);
        dicState.Add(PlayerState.Attack, attack);
        dicState.Add(PlayerState.Jump, jump);
        dicState.Add(PlayerState.Dash, dash);
        dicState.Add(PlayerState.DashAttack, dashAttack);
        dicState.Add(PlayerState.Fall, fall);
        dicState.Add(PlayerState.Shoot, shoot);
        dicState.Add(PlayerState.Damaged, damaged);

        //inputinterface 초기화
        InputInterface jumpInput = new JumpInput();
        InputInterface attackInput = new AttackInput();
        InputInterface dashInput = new DashInput();
        InputInterface shootInput = new ShootInput();

        inputButtons.Add(jumpInput);
        inputButtons.Add(attackInput);
        inputButtons.Add(dashInput);
        inputButtons.Add(shootInput);

        //초기 상태
        stateMachine = new StateMachine<Player>(dicState[PlayerState.Run], this);
    }

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    protected override void ComputeVelocity()
    {
        //Update 이후 움직일 상대 벡터
        move = Vector2.zero;

        GetInput();
        //stateMachine의 Update동작 수행
        stateMachine.DoOperateUpdate(this);
        anim.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
        targetVelocity = move * maxSpeed;
    }

    private void GetInput()
    {
        //점프 중 땅에 닿으면 달리기 상태로 돌아옴
        if (grounded)
        {
            if (stateMachine.CurrentState == dicState[PlayerState.Jump] || stateMachine.CurrentState == dicState[PlayerState.Fall])
            {
                stateMachine.SetState(dicState[PlayerState.Run]);
            }
        }
        else
        {
            if(stateMachine.CurrentState != dicState[PlayerState.Jump] && stateMachine.CurrentState != dicState[PlayerState.Damaged])
                stateMachine.SetState(dicState[PlayerState.Fall]);
        }


        for (int i = 0; i < inputButtons.Count; i++)
        {
            if (inputButtons[i].Support())
            {
                inputButtons[i].Execute();
            }
        }
    }

    public void GetHorizontalMovement()
    {
        move.x = Input.GetAxis("Horizontal");
        bool flipSprite = facingright ? (move.x < 0.0f) : (move.x > 0.0f);
        if (flipSprite)
        {
            FilpFacing();
        }
    }

    public void ChangeState(PlayerState nextState)
    {
        stateMachine.SetState(dicState[nextState]);
    }

    public void GetDamage(int amount, int launchDir)
    {
        if (stateMachine.CurrentState != dicState[PlayerState.Damaged])
        {
            health -= amount;
            launchDirection = launchDir;
            stateMachine.SetState(dicState[PlayerState.Damaged]);
            if (health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void ToggleUndamagable()
    {
        if (gameObject.layer == 10)
            gameObject.layer = 9;
        else
            gameObject.layer = 10;
    }

    public void FilpFacing()
    {
        facingright = !facingright;
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }

    public void DashMovement()
    {
        if (facingright)
            move.x = dashSpeed;
        else
            move.x = -dashSpeed;
    }

    public void HitReaction()
    {
        if (launchDirection == -1)   //왼쪽으로 날아가야 한다면
            move.x = -launchDistance / launchTime;
        else
            move.x = launchDistance / launchTime;
    }

    //animation events;
    public void FinishDash()
    {
        stateMachine.SetState(dicState[PlayerState.Run]);
    }


    public void EnableNextInput()
    {
        waitingNextInput = true;
    }

    public void DoNextAction(string next)
    {
        if (!doNextAttack)
        {
            stateMachine.SetState(dicState[PlayerState.Run]);
        }
        else
        {
            anim.Play(next);
        }
        waitingNextInput = false;
        doNextAttack = false;
    }

    public void FinishAttack()
    {
        waitingNextInput = false;
        doNextAttack = false;
        stateMachine.SetState(dicState[PlayerState.Run]);
    }

    public void DoAttack()
    {
        Debug.Log("DoAttack() 호출");
        Vector2 targetPos;
        if (facingright)
        {
            targetPos = new Vector2 (1,0); 
        }
        else
        {
            targetPos = new Vector2(-1, 0);
        }

        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, targetPos, attackLength);
        Debug.DrawLine(transform.position, (Vector2)transform.position + targetPos * attackLength, Color.red);
        for(int i = 0; i < hit.Length; i++)
        {
            int launchDirection = 1;
            if(transform.position.x > hit[i].collider.transform.position.x)
            {
                launchDirection = -1;
            }
            EnemyBase target = hit[i].collider.GetComponent<EnemyBase>();
            if (target != null)
            {
                Debug.Log(hit[i].collider.name + "에게 " + attackDamage + "만큼의 피해를 주었습니다");
                target.GetDamage(attackDamage, launchDirection);
            }
        }
    }

    public void DoRangeAttack()
    {
        GameObject arrow = Instantiate(arrowPrefab);
        arrow.transform.position = firepoint.position;
        if (facingright)
        {
            arrow.GetComponent<Projectile>().Launch(new Vector3(1, 0, 0));
        }
        else
        {
            arrow.GetComponent<Projectile>().Launch(new Vector3(-1, 0, 0));
        }

    }
}
