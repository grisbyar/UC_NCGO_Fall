using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace App.Resource.Scripts.Obj
{
    public class BulletSpawner : NetworkBehaviour
    {
        [SerializeField] public NetworkVariable<int> _ammo = new NetworkVariable<int>(4);
        [SerializeField] private Transform _startingPoint;
        [SerializeField] private NetworkObject _ProjectilePrefab;

        //could optimized by creating pooled projectiles
        //pooling object pooling

        [Rpc(SendTo.Server, RequireOwnership = false)]
        public void FireProjectileRpc(RpcParams rpcParams = default)
        {
            Debug.Log("Projectile firing");
            if (_ammo.Value > 0)
            {
                Debug.Log($"222 Projectile firing: ammo left: {_ammo}");
                //spawn our bullet at your starting point position and use spawn with ownership so we can own the projectile
                NetworkObject newProjectile = NetworkManager.Instantiate(_ProjectilePrefab, _startingPoint.position, _startingPoint.rotation);

                newProjectile.SpawnWithOwnership(rpcParams.Receive.SenderClientId);

                //despawn to prevent memory leak
                StartCoroutine(DespawnAfterTime(newProjectile, 7f)); 
                //minus one ammo
                _ammo.Value--;
            }
        }

        private IEnumerator DespawnAfterTime(NetworkObject projectile, float time)
        {   
            yield return new WaitForSeconds(time);

            if (projectile != null && projectile.IsSpawned)
            {
                projectile.Despawn();
            }
        }
    }
}

