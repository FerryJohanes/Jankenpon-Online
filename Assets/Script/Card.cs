using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Attack AttackValue;
    public CardPlayer player;
    public Vector2 OriginalPosition;
    Vector2 originalScale;
    Color originalcolor;

    bool isClickable = true;

    private void Start()
    {
        OriginalPosition = this.transform.position;
        originalScale = this.transform.localScale;
        originalcolor = GetComponent<Image>().color;
    }

    public void Onclick()
    {
        player.SetChosenCard(this);
    }


    internal void Reset()
    {
        transform.position = OriginalPosition;
        transform.localScale = originalScale;
        GetComponent<Image>().color = originalcolor;
    }

    public void SetClickable(bool value)
    {
        isClickable = value;
    }
}
