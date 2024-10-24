using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerLabel : MonoBehaviour
{
    [SerializeField] private TMP_Text _playerText;
    [SerializeField] private Button _kickBttn;
    [SerializeField] private RawImage _ReadyStatusImg, _PlayerColorImg;

    

    public event Action<ulong> onKickClicked;
    private ulong _clientId;
    //methods to populate information
    private void OnEnable()
    {
        _kickBttn.onClick.AddListener((BttnKick_Clicked));
    }
    public void SetPlayerLabelName(ulong playerName)
    {
        _clientId = playerName;
        _playerText.text = "Player" + playerName.ToString();
    }

    private void BttnKick_Clicked()
    {
        onKickClicked?.Invoke(_clientId);
    }
    public void SetKickActive(bool isOn)
    {
        _kickBttn.gameObject.SetActive(isOn);
    }
    public void SetReady(bool ready)
    {
        if (ready)
        {
            _ReadyStatusImg.color = Color.green;
        }
        else
        {
            _ReadyStatusImg.color = Color.red;

        }
    }
    public void SetPlayerColor(Color color)
    {
        _PlayerColorImg.color = color;
    }
}
