using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    
    [SerializeField] BlockGen blockGen;
    PlayerInput playerInput;
    Animator animator;
    Vector2 inputVector;
    Vector2 lastMove;
    

    Rigidbody2D rb;
    Inputs inputs;
    void Awake(){
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        inputs = new Inputs();
        inputs.Player.Enable();
        inputs.Player.Walk.performed += Move;
    }

    void Start(){
        transform.position = new Vector3(BlockGen.startPos.x, BlockGen.startPos.y+0.5f, 0);
    }

    void Move(InputAction.CallbackContext _c){
        lastMove = _c.ReadValue<Vector2>();
    }

    void Update(){
        inputVector = inputs.Player.Walk.ReadValue<Vector2>();
    }


    void FixedUpdate(){
        Vector2 _movement = inputVector * moveSpeed * Time.fixedDeltaTime;
        
        rb.MovePosition(rb.position + _movement);
        animator.SetFloat("xMvt", inputVector.x);
        animator.SetFloat("yMvt", inputVector.y);
        animator.SetFloat("speed", inputVector.sqrMagnitude);
        animator.SetFloat("IdleX", lastMove.x);
        animator.SetFloat("IdleY", lastMove.y);
       
        
    }
}
