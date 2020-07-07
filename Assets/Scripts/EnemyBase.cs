using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : PhysicsObject
{
    public int health;
    public int direction = 1;

    //for CheckLedge
    private RaycastHit2D leftLedge;
    private RaycastHit2D rightLedge;

    //enemy damage
    public int attackDamage = 1;

    //for getDamaged
    [HideInInspector]
    public float launchDirection;
    public float launchDistance = 3f;
    public float launchTime = 2f;
    public float launchPower = 5f;
    public Vector2 move;

    //for chase, idle state
    public float followRange;
    public float moveSpeed;
    public float ledgeCheckDist;
    public float playerDifference { get; private set; }

    public SpriteRenderer spriteRenderer { get; private set; }
    public Animator anim { get; private set; }

    //for statemachine
    private StateMachine<EnemyBase> stateMachine;
    private Dictionary<EnemyState, IState<EnemyBase>> dicEState = new Dictionary<EnemyState, IState<EnemyBase>>();

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        IState<EnemyBase> idle = new StateEIdle();
        IState<EnemyBase> chase = new StateEChase();
        IState<EnemyBase> attack = new StateEAttack();
        IState<EnemyBase> damaged = new StateEDamaged();

        dicEState.Add(EnemyState.Idle, idle);
        dicEState.Add(EnemyState.Chase, chase);
        dicEState.Add(EnemyState.Attack, attack);
        dicEState.Add(EnemyState.Damaged, damaged);

        stateMachine = new StateMachine<EnemyBase>(dicEState[EnemyState.Idle], this);
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, followRange);
    }

    protected override void ComputeVelocity()
    {
        move = Vector2.zero;

        playerDifference = Player.Instance.transform.position.x - transform.position.x;

        if (stateMachine.CurrentState != dicEState[EnemyState.Damaged])
        {
            if (Mathf.Abs(playerDifference) < followRange)
            {
                stateMachine.SetState(dicEState[EnemyState.Chase]);
            }
            else
            {
                stateMachine.SetState(dicEState[EnemyState.Idle]);
            }
        }


        stateMachine.DoOperateUpdate(this);
        targetVelocity = move * moveSpeed;
        spriteRenderer.flipX = (direction > 0);
    }

    public void GetDamage(int amount, int launchDir)
    {
        if (stateMachine.CurrentState != dicEState[EnemyState.Damaged])
        {
            health -= amount;
            launchDirection = launchDir;
            stateMachine.SetState(dicEState[EnemyState.Damaged]);
            if (health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void ChangeState(EnemyState nextState) {
        stateMachine.SetState(dicEState[nextState]);  
    }

    public bool TheresLedge()
    {
        if (!spriteRenderer.flipX) //왼쪽을 보고 있다면
        { 
            leftLedge = Physics2D.Raycast(new Vector2(transform.position.x - ledgeCheckDist, transform.position.y), Vector2.down);
            Debug.DrawLine(new Vector2(transform.position.x - ledgeCheckDist, transform.position.y), new Vector2(transform.position.x - ledgeCheckDist, transform.position.y - 2f), Color.red);
            return leftLedge.collider == null;
        }
        else {
            rightLedge = Physics2D.Raycast(new Vector2(transform.position.x + ledgeCheckDist, transform.position.y), Vector2.down);
            Debug.DrawLine(new Vector2(transform.position.x + ledgeCheckDist, transform.position.y), new Vector2(transform.position.x + ledgeCheckDist, transform.position.y - 2f), Color.red);
            return rightLedge.collider == null;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            //데미지 처리 
            int launchDirection = 1;
            if (transform.position.x > player.transform.position.x)
            {
                launchDirection = -1;
            }
            Debug.Log(player.name + "에게 " + attackDamage + "만큼의 피해를 주었습니다");
            player.GetDamage(attackDamage, launchDirection);
        }
    }
}
