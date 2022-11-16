using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;

public class PropertySetting : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Slider slider;

    [SerializeField]
    TMP_InputField inputField;

    [SerializeField]
    string propertyKey;

    [SerializeField]
    float initialValue = 50;

    [SerializeField]
    float minlValue = 0;

    [SerializeField]
    float maxValue = 100;

    [SerializeField]
    bool wholeNumbers = true;

    private void Start()
    {
        slider.minValue = minlValue;
        slider.maxValue = maxValue;
        slider.wholeNumbers = wholeNumbers;
        inputField.contentType = wholeNumbers
            ? TMP_InputField.ContentType.IntegerNumber
            : TMP_InputField.ContentType.DecimalNumber;

        if (PhotonNetwork.IsMasterClient == false)
        {
            slider.interactable = false;
            inputField.interactable = false;
        }

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(propertyKey, out var value))
        {
            UpdateSliderInputField((float)value);
        }
        else
        {
            UpdateSliderInputField(initialValue);
        }
    }

    public void InputFromSlider(float value)
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;
        UpdateSliderInputField(value);
        SetCustomProperty(value);
    }

    public void InputFromField(string stringValue)
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;
        if (float.TryParse(stringValue, out var floatValue))
        {
            floatValue = Mathf.Clamp(floatValue, slider.minValue, slider.maxValue);
            UpdateSliderInputField(floatValue);
            SetCustomProperty(floatValue);
        }
    }

    private void SetCustomProperty(float value)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        var property = new Hashtable();
        property.Add(propertyKey, value);
        PhotonNetwork.CurrentRoom.SetCustomProperties(property);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (
            propertiesThatChanged.TryGetValue(propertyKey, out var value)
            && PhotonNetwork.IsMasterClient == false
        )
        {
            UpdateSliderInputField((float)value);
        }
    }

    private void UpdateSliderInputField(object value)
    {
        var floatValue = (float)value;
        slider.value = floatValue;

        if (wholeNumbers)
            inputField.text = (Mathf.RoundToInt(floatValue)).ToString("D");
        else
            inputField.text = (floatValue).ToString("F2");
    }
}
