using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class UI_NetManager : NetworkBehaviour
{
   [SerializeField] private Button _serverBttn, _clientBttn, _hostBttn;


    void Start()
    {
        _serverBttn.onClick.AddListener(serverClick);

        _clientBttn.onClick.AddListener(clientClick);

        _hostBttn.onClick.AddListener(hostClick);
    }


    private void serverClick()
    {
        //starts the network manager as just a server
        NetworkManager.Singleton.StartServer(); 
        this.gameObject.SetActive(false);
    }
     private void clientClick() 
    {
        //starts the network manager as just a client
        NetworkManager.Singleton.StartClient();
        this.gameObject.SetActive(false);
    }
     private void hostClick()
    {
        //starts the network manager as both a server and a client
        NetworkManager.Singleton.StartHost(); 
        this.gameObject.SetActive(false);
    }
}
