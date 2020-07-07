using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    public float minGroundNormalY = 0.65f;
    public float gravityModifier = 1f;

    [SerializeField]
    public bool grounded { get; protected set; }
    protected Vector2 groundNormal;
    protected Vector2 targetVelocity;

    protected Rigidbody2D rb;
    [SerializeField]
    public Vector2 velocity;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

    protected const float minMoveDistablce = 0.001f;
    protected const float shellRadius = 0.01f;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
    }

    private void Update()
    {
        targetVelocity = Vector2.zero;
        ComputeVelocity();
    }

    protected virtual void ComputeVelocity()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
        velocity.x = targetVelocity.x;

        grounded = false;

        Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x); //경사면을 고려한 벡터, groundNormal과 수직, 경사면에서 좌 우가 아닌 경사면을 따라 움직이므로 이에 대한 코드 필요

        Debug.DrawRay(transform.position, moveAlongGround, Color.red);

        Vector2 deltaPosition = velocity * Time.deltaTime;

        Vector2 move = moveAlongGround * deltaPosition.x;

        //x축 이동
        Movement(move, false);

        move = Vector2.up * deltaPosition.y;

        //y축 이동
        Movement(move, true);
    }

    void Movement(Vector2 move, bool yMovement)
    {
        float distance = move.magnitude;

        if (distance > minMoveDistablce)
        {
            contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
            int count = rb.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
            hitBufferList.Clear();
            for(int i = 0; i < count; i++) { 
                hitBufferList.Add(hitBuffer[i]);
            }

            for(int i = 0; i < hitBufferList.Count; i++)
            {
                Vector2 currentNormal = hitBufferList[i].normal;
                if(currentNormal.y > minGroundNormalY)
                {
                    grounded = true;
                    if (yMovement){
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(velocity, currentNormal);
                if(projection < 0)
                {
                    velocity = velocity - projection * currentNormal;
                }

                float modifyDistance = hitBufferList[i].distance - shellRadius;
                distance = modifyDistance < distance ? modifyDistance : distance;
            }

        }

        rb.position = rb.position + move.normalized * distance;
    }
}
