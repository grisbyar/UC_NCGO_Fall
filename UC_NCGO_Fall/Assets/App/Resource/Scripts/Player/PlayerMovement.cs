using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace App.Resource.Scripts.Player
{
    
public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private Animator _myAnimator;
    [SerializeField] private OwnerNetworkAnimator _ownerAnimator;
    
    
    [SerializeField] float defaultMoveSpeed = 7f;
    [SerializeField] float movementModifier = 1.5f;
    //extras
    // [SerializeField] float defaultHealth = 300f;
    // [SerializeField] float punchStrength = 100f;

    private void Awake()
    {
        if(_myAnimator == null)
        {
            _myAnimator = gameObject.GetComponent<Animator>();
        }

        if(_ownerAnimator == null)
        {
            _ownerAnimator = gameObject.GetComponent<OwnerNetworkAnimator>();
        }
    }

    private void FixedUpdate()
    {
        if(!IsOwner) return;

        
        float moveSpeed = defaultMoveSpeed;

        Vector3 moveDirection = new Vector3(0,0,0);
        if(Input.GetKey(KeyCode.W)) moveDirection.z = +1f;
        if(Input.GetKey(KeyCode.S)) moveDirection.z = -1f;
        if(Input.GetKey(KeyCode.A)) moveDirection.x = -1f;
        if(Input.GetKey(KeyCode.D)) moveDirection.x = +1f;        
        if(Input.GetKey(KeyCode.Space)) _ownerAnimator.SetTrigger("JumpTrigger");
        //was z
        if(Input.GetKey(KeyCode.Q)) _ownerAnimator.SetTrigger("PunchTrigger");
        
        if(Input.GetKey(KeyCode.LeftShift)){
            moveSpeed = defaultMoveSpeed * movementModifier;
            _myAnimator.SetBool("IsSprinting", true);
        }
        else
        {
            moveSpeed = defaultMoveSpeed;
            _myAnimator.SetBool("IsSprinting", false);
        }

        _myAnimator.SetBool("IsWalking", moveDirection.z !=0 || moveDirection.x != 0);
        
        transform.position += moveDirection * (moveSpeed * Time.deltaTime);


        // //punch
        // if(Input.GetKey(KeyCode.Q)){}
        // //jumping
        // if(Input.GetKey(KeyCode.Spacebar)){}
        // //running
        // if(Input.GetKey(KeyCode.Shift)){}
    }
}
}