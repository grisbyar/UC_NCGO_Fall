using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class UI_NetManager : NetworkBehaviour
{
   [SerializeField] private Button _serverBttn, _clientBttn, _hostBttn, _startBttn;

   [SerializeField] private GameObject _connectionBttnGroup;
   
   [SerializeField] private SpawnController _mySpawnController;

    void Start()
    {
        _startBttn.gameObject.SetActive(false);

        if(_serverBttn != null) _serverBttn.onClick.AddListener(serverClick);

        if(_clientBttn != null) _clientBttn.onClick.AddListener(clientClick);

        if(_hostBttn != null) _hostBttn.onClick.AddListener(hostClick);

        if(_startBttn != null) _startBttn.onClick.AddListener(startClick);
    }

    private void startClick()
    {
        //hook up spawning here
        if(IsServer)
        {
             _mySpawnController.SpawnAllPlayers(); 
            _startBttn.gameObject.SetActive(false);     
        }

    }


    private void serverClick()
    {
        //starts the network manager as just a server
        NetworkManager.Singleton.StartServer(); 
        _connectionBttnGroup.SetActive(false);
         _startBttn.gameObject.SetActive(true);
    }
     private void clientClick() 
    {
        //starts the network manager as just a client
        NetworkManager.Singleton.StartClient();
        _connectionBttnGroup.SetActive(false);
    }
     private void hostClick()
    {
        //starts the network manager as both a server and a client
        NetworkManager.Singleton.StartHost(); 
        _connectionBttnGroup.SetActive(false);
         _startBttn.gameObject.SetActive(true);
    }
    
}
