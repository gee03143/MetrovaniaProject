using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator anim;

    [SerializeField]
    private string defaultAnimName;
    [SerializeField]
    private string[] attacks;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableNextInput()
    {
        
    }

    public void DoNextAction(string next)
    {
        anim.Play(next);
    }
}
