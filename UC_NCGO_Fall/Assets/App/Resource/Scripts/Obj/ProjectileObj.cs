using System.Collections;
using System.Collections.Generic;
using App.Resource.Scripts.Player;
using UnityEngine;
using Unity.Netcode;

namespace App.Resource.Scripts.Obj
{


public class ProjectileObj : NetworkBehaviour
{
    [SerializeField] float _speed = 40f;
    [SerializeField] private float _damage = 10;
    [SerializeField] private float destructTime = 5f;
    
    //to do hook up projectile to pool system 

   public override void OnNetworkSpawn()
   {
        base.OnNetworkSpawn();
        GetComponent<Rigidbody>().velocity = this.transform.forward * _speed;
        StartCoroutine(AutoDestruct());
   }
   public void OnCollisionEnter(Collision other)
   {
    //make sure it's a player and that the bullet doesn't match the owner, meaning we can't do friendly fire
    if(other.gameObject.tag.Equals("Player") && other.gameObject.GetComponent<NetworkObject>().OwnerClientId != this.OwnerClientId)
    {
        other.gameObject.GetComponent<HealthNetScript>().DamageObjRpc(_damage);
    }

   }

   private IEnumerator AutoDestruct()
   {
    yield return new WaitForSeconds(destructTime);
    this.NetworkObject.Despawn();

   }
}
}