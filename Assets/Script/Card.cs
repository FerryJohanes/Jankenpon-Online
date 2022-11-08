using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public Attack attack;
    public Player player;
    public void Onclick()
    {
        player.SetAttack(attack);
    }
}
