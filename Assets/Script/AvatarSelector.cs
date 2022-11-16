using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;

public class AvatarSelector : MonoBehaviour
{
    [SerializeField] Image avatarImage;
    [SerializeField] Sprite[] avatarSprites;
    private int selectedIndex;

    private void Start()
    {
        // selectedIndex = PlayerPrefs.GetInt(PropertyNames.Player.AvatarIndex, 0);
        selectedIndex = PlayerPrefs.GetInt("AvatarIndex",0);
        avatarImage.sprite = avatarSprites[selectedIndex];
        SaveSelectedIndex();
    }

    public void ShiftSelectedIndex(int shift)
    {
        //shifting index ke kiri atau kanan
        selectedIndex += shift;

        while(selectedIndex >= avatarSprites.Length)
            selectedIndex -= avatarSprites.Length;

        while(selectedIndex < 0)
            selectedIndex += avatarSprites.Length;

        avatarImage.sprite = avatarSprites[selectedIndex];
        SaveSelectedIndex();
    }

    private void SaveSelectedIndex()
    {
        //simpan di lokal storage
        // PlayerPrefs.SetInt(PropertyNames.Player.AvatarIndex, selectedIndex);
        PlayerPrefs.SetInt("AvatarIndex", selectedIndex);

        //simpan di network
        var property = new Hashtable();
        property.Add("AvatarIndex", selectedIndex);
        // property.Add(PropertyNames.Player.AvatarIndex, selectedIndex);
        PhotonNetwork.LocalPlayer.SetCustomProperties(property);
    }

}
