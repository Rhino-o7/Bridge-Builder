using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    public bool canMove = false;
    
    [SerializeField] BlockGen blockGen;
    PlayerInput playerInput;
    Animator animator;
    Vector2 inputVector;
    Vector2 lastMove;
    public AudioSource audioSource;
    

    Rigidbody2D rb;
    Inputs inputs;
    void Awake(){
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        inputs = new Inputs();
        inputs.Player.Enable();
        inputs.Player.Walk.performed += Move;
    }

    public void StartPos(){
        transform.position = new Vector3(BlockGen.startPos.x+0.5f, BlockGen.startPos.y + 1.2f, 0);
    }

    void Move(InputAction.CallbackContext _c){
        lastMove = _c.ReadValue<Vector2>();
    }

    void Update(){
        inputVector = inputs.Player.Walk.ReadValue<Vector2>();
        
    }


    void FixedUpdate(){
        if (canMove){
           Vector2 _movement = inputVector * moveSpeed * Time.fixedDeltaTime;
        
            rb.MovePosition(rb.position + _movement);
            animator.SetFloat("xMvt", inputVector.x);
            animator.SetFloat("yMvt", inputVector.y);
            animator.SetFloat("speed", inputVector.sqrMagnitude);
            animator.SetFloat("IdleX", lastMove.x);
            animator.SetFloat("IdleY", lastMove.y);
            if (_movement != Vector2.zero && !audioSource.isPlaying){
                audioSource.Play();
            }else if (_movement == Vector2.zero && audioSource.isPlaying){
                audioSource.Stop();
            } 
        }else{
            if (audioSource.isPlaying){
                audioSource.Stop();
                animator.SetFloat("xMvt", 0);
                animator.SetFloat("yMvt", 0);
                animator.SetFloat("speed", 0);
                animator.SetFloat("IdleX", 0);
                animator.SetFloat("IdleY", 0);
            }
        }
        
       
        
    }
}
