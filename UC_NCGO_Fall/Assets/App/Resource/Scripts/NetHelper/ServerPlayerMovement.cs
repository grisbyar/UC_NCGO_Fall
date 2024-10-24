using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

[RequireComponent(typeof(CharacterController))]
public class ServerPlayerMovement : NetworkBehaviour
{
    
    [SerializeField] private Animator _myAnimator;
    [SerializeField] private NetworkAnimator _myNetAnimator;
    [SerializeField] private float _pSpeed;
    [SerializeField] private Transform _pTransform;
    // [SerializeField] float defaultHealth = 300f;
    // [SerializeField] float punchStrength = 100f;

    public CharacterController _CC;
    private MyPlayerInputActions _playerInput;

    // Start is called before the first frame update
    void Start()
    {

        if(_myAnimator == null){
            _myAnimator = gameObject.GetComponent<Animator>();
        }
        if(_myNetAnimator == null){
            _myNetAnimator = gameObject.GetComponent<NetworkAnimator>();
        }
        _playerInput = new MyPlayerInputActions();
        _playerInput.Enable();    
    }

    // Update is called once per frame
    void Update()
    {
        if(IsOwner) return; //wouldn't this be better reverse?
        //read our player input from new system\
        Vector2 moveInput = _playerInput.Player.Movement.ReadValue<Vector2>();

        bool isJumping = _playerInput.Player.Jumping.triggered;
        bool isPunching = _playerInput.Player.Punching.triggered;
        bool isSprinting =  _playerInput.Player.Sprinting.triggered;


        //determine if we are a server or a player
        if(IsServer)
        {
             //move if server
            Move(moveInput, isPunching, isSprinting, isJumping);
        }
        else if(IsClient && !IsHost)
        {
            //send a move request rpc to move the player
            MoveServerRPC(moveInput, isPunching, isSprinting, isJumping);
        }
       

        
    }
    private void Move(Vector2 _input, bool isPunching, bool isSprinting, bool isJumping)
    {
        Vector3 _moveDirection = _input.x * _pTransform.right + _input.y * _pTransform.forward;
        
        _myAnimator.SetBool("IsWalking", _input.x != 0 || _input.y != 0);
        
        //you must use netanimator to set trigger
        if(isJumping) _myNetAnimator.SetTrigger("JumpTrigger");
        if(isPunching) _myNetAnimator.SetTrigger("PunchTrigger");
        
        //any property besides that you can call the animator
        _myAnimator.SetBool("IsSprinting", isSprinting);

        if(isSprinting) {
            _CC.Move(_moveDirection * (_pSpeed * 1.3f) * Time.deltaTime);

        }
        else{
            _CC.Move(_moveDirection * _pSpeed *Time.deltaTime);

        }

    }

    [Rpc(SendTo.Server)]
    private void MoveServerRPC(Vector2 _input, bool isPunching, bool isSprinting, bool isJumping)
    {
        Move(_input, isPunching, isSprinting, isJumping);
    }
}
