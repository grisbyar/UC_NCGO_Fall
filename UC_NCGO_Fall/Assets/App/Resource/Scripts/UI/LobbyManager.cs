using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkBehaviour
{
 [SerializeField] private Button _startBttn, _leaveBttn, _readyBttn;
 [SerializeField] private GameObject _panelPrefab; //the prefab we place inside of contents
 [SerializeField] private GameObject _ContentGO; //where we are spawning panelPrefabs to
 [SerializeField] private TMP_Text rdyTxt; // update status to user


///list of network players
[SerializeField] private NetworkedPlayerData _networkPlayers;

private List<GameObject> _PlayerPanels = new List<GameObject>();
private ulong _myServerID;
private bool isReady = false;


private void Start()
{
    _myServerID = NetworkManager.ServerClientId;
    if(IsServer)
    {
        //Server
        rdyTxt.text = "Waiting for Players";
        _readyBttn.gameObject.SetActive(false);
    }
    else
    {
        //client
        rdyTxt.text = "Not Ready";
        _readyBttn.gameObject.SetActive(true);
    }
    _networkPlayers._allConnectedPlayers.OnListChanged += NetPlayersChanged;
    _leaveBttn.onClick.AddListener(LeaveBttnClick);
    _readyBttn.onClick.AddListener(ClientRdyBttnToggle);
}

private void ClientRdyBttnToggle()
{
    if(IsServer){return;}
    isReady =!isReady;
    if(isReady)
    {
        rdyTxt.text = "Ready";
    }
    else
    {
        rdyTxt.text = "Not ready";
    }
    RdyBttnToggleServerRpc(isReady);
}
//RPC call for when bttn is clicked
[Rpc(SendTo.Server, RequireOwnership = false)]
private void RdyBttnToggleServerRpc(bool readyStatus, RpcParams rpcParams = default)
{
    Debug.Log("From rdy bttn rpc");
    _networkPlayers.UpdateReadyClient(rpcParams.Receive.SenderClientId, readyStatus);
}

private void LeaveBttnClick()
{
    if(!IsServer)
    {
        QuitLobbyServerRpc();
        
    }
    else
    {
        foreach(PlayerInfoData playerData in _networkPlayers._allConnectedPlayers)
        {
            if(playerData._clientId != _myServerID)
            {
                KickUserBttn(playerData._clientId);
            }
        }
        NetworkManager.Shutdown();
        SceneManager.LoadScene(0);
    }
}
[Rpc(SendTo.Server, RequireOwnership = false)]
private void QuitLobbyServerRpc(RpcParams rpcParams=default)
{
    KickUserBttn(rpcParams.Receive.SenderClientId);
}
private void NetPlayersChanged(NetworkListEvent<PlayerInfoData> changeevent)
{
    Debug.Log("Net POlayers has changed event fired");
    PopulateLabels();
}

//populate panels
[ContextMenu("PopulateLabels")]
private void PopulateLabels()
{
    //ClearPanels
    ClearPlayerPanel();

    //loop all player info from allnetworked players and then create new panels
    bool allReady = true; //used for logic on server
    foreach (PlayerInfoData playerData in _networkPlayers._allConnectedPlayers)
    {
        //instantiate
        GameObject newPlayerPanel = Instantiate(_panelPrefab, _ContentGO.transform);
        PlayerLabel _playerLabel = newPlayerPanel.GetComponent<PlayerLabel>();
        
        //subscribe to kick events on the panels
        _playerLabel.onKickClicked += KickUserBttn;

        //depending on client vs server we are going to show/hide kick bttns
        if(IsServer && playerData._clientId != _myServerID)
        {
            //ensure that we are the host and set active kick buttons
            _playerLabel.SetKickActive(true);
            //ensure servers ready button is hidden, we assume server is always ready
            _readyBttn.gameObject.SetActive(false);
        }
        else
        {
            //ensure clients don't have set kicked buttons visible, but readybttn is visible
            _playerLabel.SetKickActive(false);
            //_readyBttn.gameObject.SetActive(true);
        }
        //send info to UI
        _playerLabel.SetPlayerLabelName(playerData._clientId);
        _playerLabel.SetReady(playerData._isPlayerReady);
        _playerLabel.SetPlayerColor(playerData._colorId);
        _PlayerPanels.Add(newPlayerPanel);

        if(playerData._isPlayerReady == false)
        {
            allReady = false;
        }

        //Check if everyone is ready, host should see if it's ready or not
        if(allReady)
        {
            if(_networkPlayers._allConnectedPlayers.Count > 1)
            {
                rdyTxt.text = "Ready to start";
                _startBttn.gameObject.SetActive(true);
            }
            else
            {
                rdyTxt.text = "Empty Lobby";
            }
        }
        else
        {
            _startBttn.gameObject.SetActive(false);
            rdyTxt.text = "waiting for ready players";
        }
    }
}
private void KickUserBttn(ulong kickTarget)
{

    if(!IsServer || !IsHost) return; //cannot kick the host/server

    foreach(PlayerInfoData playerData in _networkPlayers._allConnectedPlayers)
    {
        if(playerData._clientId == kickTarget)
        {
            //remove player from the list
            _networkPlayers._allConnectedPlayers.Remove(playerData);
            
            KickedClientRpc(RpcTarget.Single(kickTarget, RpcTargetUse.Temp));

            //remove from network
            NetworkManager.Singleton.DisconnectClient(kickTarget);
        }
    }
}
[Rpc(SendTo.SpecifiedInParams)]
private void KickedClientRpc(RpcParams rpcParams)
{
    SceneManager.LoadScene(0);
}
private void ClearPlayerPanel()
{
    foreach (GameObject panel in _PlayerPanels)
    {
        Destroy(panel);
    }
    _PlayerPanels.Clear();
}


}
