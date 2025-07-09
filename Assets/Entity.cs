using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    protected Animator anim;
    protected Rigidbody2D rb;

    

    protected virtual void Start()
    {
        anim.GetComponentInChildren<Animator>();
        rb.GetComponentInChildren<Rigidbody2D>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
       // CollisionChecks();
    }

    
}
