using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class PlayerItem : MonoBehaviour
{
    [SerializeField] Image avatarImage;
    [SerializeField] TMP_Text playerName;
    [SerializeField] Sprite[] avatarSprites;

    public void Set(Photon.Realtime.Player player)
    {
        if(player.CustomProperties.TryGetValue("AvatarIndex", out var value))
            avatarImage.sprite = avatarSprites[(int)value];

        playerName.text = player.NickName;
        
        if(player == PhotonNetwork.MasterClient)
        {
            playerName.text += " (Master)";
        }
    }
}
