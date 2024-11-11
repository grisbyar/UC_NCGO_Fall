using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class ChatManager : NetworkBehaviour
{
    [SerializeField] private Button _sendBttn;
    [SerializeField] private TMP_InputField  _chatInput;
    [SerializeField] public Text _chatHistory;
    [SerializeField] private RectTransform _contentPanel;
    [SerializeField] private Scrollbar _scrollbar;  
    //[SerializeField] private Viewport _chatHistory;
    
    //send message
    private void sendChatMessage()
    {
        //Debug.Log("SendChatMessage worked");
        if (!string.IsNullOrEmpty(_chatInput.text)){
            Debug.Log("SendChatMessage is in the if");
            
            string chatTime = this.GetCurrentTime();
            string message = $"{chatTime} : {NetworkManager.Singleton.LocalClientId}: {_chatInput.text}";
            SubmitChatMessageToServerRpc(message); // Send the message to the server
            Debug.Log($"{chatTime} worked");
            Debug.Log($"{message} worked");
            _chatInput.text = ""; // clear
            
            
        }
    
    }

    // ServerRpc to recieve message
    [ServerRpc(RequireOwnership = false)]
    private void SubmitChatMessageToServerRpc(string message, ServerRpcParams serverRpcParams = default)
    {
        DisplayChatMessageClientRpc(message); // display message on clients
    }

     // ClientRpc to display the message on clientz
    [ClientRpc]
    private void DisplayChatMessageClientRpc(string message)
    {
        _chatHistory.text += $"{message}\n"; // show message
        
        // adjust scroll position to go up like a normal chat would
        Canvas.ForceUpdateCanvases(); //update layout
        _scrollbar.value = 0;
    }

    public string GetCurrentTime()
    {   
        DateTime timeNow = DateTime.Now;
        string _time = $"Time: {timeNow.ToString("HH:mm:ss")}";; // this needs to be reset after every message
        return _time;
    }

    void Start()
    {
        _sendBttn.onClick.AddListener(sendChatMessage);
    }

    
}
