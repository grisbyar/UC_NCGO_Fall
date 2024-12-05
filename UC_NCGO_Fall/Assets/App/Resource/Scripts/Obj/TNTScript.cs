using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace App.Resource.Scripts.Obj
{
public class TNTScript : NetworkBehaviour
{
   
   [SerializeField] public float _fuseTimer = 4f;
   public NetworkObject ExplosionPrefab;
   //[SerializeField] public Image _fuseTimerImage;


   public override void OnNetworkSpawn()
   {
    base.OnNetworkSpawn();

    if(IsServer)
    {
        StartCoroutine("TriggerFuse");
    }
   }

   private IEnumerator TriggerFuse()
   {
    Debug.Log("Fuse Lit!");
    yield return new WaitForSeconds(_fuseTimer);
    TriggerExplosionRpc();
   }
   
   [Rpc(SendTo.Server)]
   public void TriggerExplosionRpc()
   {
    NetworkObject explosive = NetworkManager.Instantiate(ExplosionPrefab, transform.position, transform.rotation);
    explosive.Spawn(true);
    this.NetworkObject.Despawn();
   }

   private void OnCollisionEnter(Collision other)
   {
    if(other.gameObject.tag.Equals("Projectile") || other.gameObject.tag.Equals("Explosion"))
    {
        TriggerExplosionRpc();
    }
   }
}
}