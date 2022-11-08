using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public State state = State.ChooseAttack;

    public enum State
    {
        ChooseAttack,
        Attack,
        Damages,
        Draw,
        GameOver,
    }
    private void Start()
    {

    }
    private void Update()
    {

        // ChooseAttack,
        if (state == State.ChooseAttack)
        {

        }
        // Attack,
        else if (state == State.Attack)
        {

        }

        // Damages,
        else if (state == State.Damages)
        {

        }

        // Draw,
        else if (state == State.Draw)
        {

        }

        // GameOver,
        else if (state == State.GameOver)
        {

        }
    }
}
