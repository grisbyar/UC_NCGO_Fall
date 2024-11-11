using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkedPlayerData : NetworkBehaviour
{
   public NetworkList<PlayerInfoData> _allConnectedPlayers; // current players in the game
   private int _players = -1;
   private ulong _serverLocalID;

   private Color[] _PlayerColors = new Color[]
   {
    Color.blue, Color.red, Color.magenta, Color.yellow, Color.white, Color.black, Color.green
   };

   private void Awake()
   {
    //avoid mem leaks by ini network list here
    _allConnectedPlayers = new NetworkList<PlayerInfoData>(readPerm: NetworkVariableReadPermission.Everyone);
   }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if(IsServer)
        {
            NetworkManager.Singleton.OnConnectionEvent += OnConnectionEvents;
            _serverLocalID = NetworkManager.LocalClientId;
    
        }
    }

    public override void OnNetworkDespawn()
    {
        if(IsServer)
        {
            NetworkManager.Singleton.OnConnectionEvent -= OnConnectionEvents;
        }
        base.OnNetworkDespawn();
    }

    public void OnConnectionEvents(NetworkManager netManager, ConnectionEventData eventData)
    {
        if(eventData.EventType == ConnectionEvent.ClientConnected)
        {
            //when client connects create data
            CreateNewClientData(eventData.ClientId);
        }
        if(eventData.EventType == ConnectionEvent.ClientDisconnected)
        {
            RemovePlayerData(FindPlayerInfoData(eventData.ClientId));
            _players--;
        }
    }
    
    

   private void CreateNewClientData(ulong clientID){
    // creating player info
    PlayerInfoData _playerInfoData = new PlayerInfoData(clientID);

    //add or modify name
    //check to see if server matches parameter client id

    
    if(_serverLocalID == clientID)
    {
        _playerInfoData._isPlayerReady = false;
    }
    _players++;

    _playerInfoData._colorId = _PlayerColors[_players];
//add to netlist
    _allConnectedPlayers.Add(_playerInfoData);
   }

   public void RemovePlayerData(PlayerInfoData playerData)
   {
     _allConnectedPlayers.Remove(playerData);
   }

   public PlayerInfoData FindPlayerInfoData(ulong clientID)
   {
    
    return _allConnectedPlayers[FindPlayerIndex(clientID)];
   }

   private int FindPlayerIndex(ulong clientID)
   {
    int myMatch = -1;
    for(int i=0; i< _allConnectedPlayers.Count; i++){
        if(clientID == _allConnectedPlayers[i]._clientId)
        {
            myMatch = i;
        }
    }
        return myMatch;
   }


   public void UpdateReadyClient(ulong clientID, bool isReady)
   {
    int idx = FindPlayerIndex(clientID);

    if(idx == -1){ return;}

    //grab info, change it, and pass it back to network list
    PlayerInfoData playerInfo = new PlayerInfoData();
    //copy data
    playerInfo = _allConnectedPlayers[idx];
    //change status
    playerInfo._isPlayerReady = isReady;
    //update new status to list
    _allConnectedPlayers[idx] = playerInfo;
   
   }
}
