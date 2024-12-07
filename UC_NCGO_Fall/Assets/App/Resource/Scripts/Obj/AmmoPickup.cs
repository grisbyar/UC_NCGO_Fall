using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace App.Resource.Scripts.Obj
{
    public class AmmoPickup : NetworkBehaviour
    {
        [SerializeField] public float _ammoTimer = 10f;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if(IsServer)
            {
                StartCoroutine("OnTimerDestroy");
            }
        }

        private IEnumerator OnTimerDestroy()
        {
            //Debug.Log("Despawn Ammo Timer in 10 secs!");
            yield return new WaitForSeconds(_ammoTimer);
            Destroy(gameObject);
        }
        
        private void OnCollisionEnter(Collision other)
        {
            if (!IsServer) return;
            if (other.gameObject.tag == "Player")
            {
                //Debug.Log($"Ammo wwent up: {other.gameObject.GetComponent<BulletSpawner>()._ammo.Value}");
                other.gameObject.GetComponent<BulletSpawner>()._ammo.Value++;
                Destroy(gameObject);
            }
        }
    }
}