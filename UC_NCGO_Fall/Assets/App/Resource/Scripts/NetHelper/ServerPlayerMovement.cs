using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using App.Resource.Scripts.Obj; 
[RequireComponent(typeof(CharacterController))]
public class ServerPlayerMovement : NetworkBehaviour
{
    
    [SerializeField] private Animator _myAnimator;
    [SerializeField] private NetworkAnimator _myNetAnimator;
    [SerializeField] private float _pSpeed;
    [SerializeField] private Transform _pTransform;
    [SerializeField] private BulletSpawner _bulletSpawner;
    public CharacterController _CC;
    private MyPlayerInputActions _playerInput;
    private static readonly int IsSprinting = Animator.StringToHash("IsSprinting");
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    Vector3 _moveDirection = new Vector3(0,0f,0);


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
        if(IsOwner) return; 
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
       
        if(isPunching)
        {
            _bulletSpawner.FireProjectileRpc();
        }
        
    }
    private void Move(Vector2 _input, bool isPunching, bool isSprinting, bool isJumping)
    {
        
        _moveDirection = new Vector3(_input.x, 0f, _input.y);
        
        _myAnimator.SetBool(IsWalking, (_input.x != 0 || _input.y != 0));

// Debug.Log($"moving here {_moveDirection}" );
//         Debug.Log($"Character Position: {transform.position}");
//         Debug.Log($"Input: {_input}, Move Direction: {_moveDirection}");
        //you must use netanimator to set trigger
        if(isJumping)
        {
            _myNetAnimator.SetTrigger("JumpTrigger");
        }
        if(isPunching){
            _myNetAnimator.SetTrigger("PunchTrigger");
        }

        //any prop=erty besides that you can call the animator
        _myAnimator.SetBool(IsSprinting, isSprinting);
        
        if(_input.x == 0f && _input.y == 0f) return;

        if(isSprinting)
        {
            //move a little faster when sprinting
             _CC.Move(_moveDirection * (_pSpeed * 1.3f * Time.deltaTime));
        }
        else{
            _CC.Move(_moveDirection * (_pSpeed * Time.deltaTime));

        }

        //rotate player into the same direction we are facing
        transform.forward = _moveDirection;
    }

    [Rpc(SendTo.Server)]
    private void MoveServerRPC(Vector2 _input, bool isPunching, bool isSprinting, bool isJumping)
    {
        Move(_input, isPunching, isSprinting, isJumping);
    }
}
