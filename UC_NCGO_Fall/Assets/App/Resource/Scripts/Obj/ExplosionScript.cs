using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using App.Resource.Scripts.Player;
namespace App.Resource.Scripts.Obj
{
public class ExplosionScript : NetworkBehaviour
{
    [SerializeField] public float _damage = 40;


    public void EndAnimEvent()
    {
        //Debug.Log("animation event triggered");
        if (IsServer)
        {
            EndAnimRpc();
        } 
    }
    //triggered by animation
    [Rpc(SendTo.Server)]
    public void EndAnimRpc()
    {
        this.NetworkObject.Despawn();
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag.Equals("Player"))
        {
            Debug.Log("Player hit!");
            other.gameObject.GetComponent<HealthNetScript>().DamageObjRpc(_damage);
        }
    }
}
}