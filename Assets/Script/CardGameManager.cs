using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class CardGameManager : MonoBehaviour, IOnEventCallback
{
    public GameObject netPlayerPrefab;
    public CardPlayer P1;
    public CardPlayer P2;
    public float maxHealth = 100;
    public float restoreValue = 5;
    public float damageValue = 10;
    public GameState State, NextState = GameState.NetPlayersInitialization;
    public GameObject gameOverPanel;
    public TMP_Text winnerText;
    public TMP_Text pingText;

    private CardPlayer damagedPlayer;
    private CardPlayer winner;
    public bool Online = true;

    // public List<int> syncReadyPlayers = new List<int> (2);
    HashSet<int> syncReadyPlayers = new HashSet<int>();
    public enum GameState
    {
        SyncState,
        NetPlayersInitialization,
        ChooseAttack,
        Attacks,
        Damages,
        Draw,
        GameOver,
    }
    private void Start()
    {
        gameOverPanel.SetActive(false);
        if(Online)
        {
            StartCoroutine(PingCoroutine());
            PhotonNetwork.Instantiate(netPlayerPrefab.name, Vector3.zero, Quaternion.identity);
            State = GameState.NetPlayersInitialization;
            NextState = GameState.NetPlayersInitialization;
            if(PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("MaxHealth", out var maxHealth))
            {this.maxHealth = (float) maxHealth;}
            if(PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("RestoreValue", out var restoreValue))
            {this.restoreValue = (float) restoreValue;}
            if(PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("DamageValue", out var damageValue))
            {this.damageValue = (float) damageValue;}

        }
        else
        {
            State = GameState.ChooseAttack;
        }
    }

    private void Update()
    {

        // ChooseAttack,
        switch (State)
        {
            case GameState.SyncState:
                if(syncReadyPlayers.Count == 2)
                {
                    syncReadyPlayers.Clear();
                    State = NextState;
                }
                break;
            case GameState.NetPlayersInitialization:

                if (CardNetPlayer.NetPlayers.Count == 2)
                {
                    foreach (var netPlayer in CardNetPlayer.NetPlayers)
                    {
                        if (netPlayer.photonView.IsMine) 
                        {
                            netPlayer.Set(P1); 
                        }
                        else 
                        { 
                            netPlayer.Set(P2); 
                        }
                    }
                    ChangeState(GameState.ChooseAttack);
                }
                break;

            case GameState.ChooseAttack:
               if(P1.AttackValue != null && P2.AttackValue != null)
                {
                    P1.AnimateAttack();
                    P2.AnimateAttack();
                    P1.IsClickable(false);
                    P2.IsClickable(false);
                    ChangeState(GameState.Attacks);
                }
                break;

            case GameState.Attacks:
                if (P1.IsAnimating() == false && P2.IsAnimating() == false)
                {
                    damagedPlayer = GetDamagedPlayer();
                    if (damagedPlayer != null)
                    {
                        damagedPlayer.AnimateDamage();
                        ChangeState(GameState.Damages);
                    }
                    else
                    {
                        P1.AnimateDraw();
                        P2.AnimateDraw();
                        ChangeState(GameState.Draw);
                    }
                }
                    break;

            case GameState.Damages:
                if (P1.IsAnimating() == false && P2.IsAnimating() == false)
                {
                    //Calculate Health
                    if(damagedPlayer == P1)
                    {
                        P1.ChangeHealth(-damageValue);
                        P2.ChangeHealth(restoreValue);
                    }
                    else
                    {
                        P1.ChangeHealth(restoreValue);
                        P2.ChangeHealth(-damageValue);
                    }

                    var winner = GetWinner();

                    if (winner == null)
                    {
                        ResetPlayers();
                        P1.IsClickable(true);
                        P2.IsClickable(true);
                        ChangeState(GameState.ChooseAttack);
                    }
                    else
                    {
                        Debug.Log(winner + " is win");
                        gameOverPanel.SetActive(true);
                        winnerText.text = winner == P1 ? $"{P1.NickName.text} WIN" : $"{P2.NickName.text} WIN";
                        ResetPlayers();
                        ChangeState(GameState.GameOver);
                    }
                }
                    break;
            case GameState.Draw:
                if (P1.IsAnimating() == false && P2.IsAnimating() == false)
                {
                    ResetPlayers();
                    P1.IsClickable(true);
                    P2.IsClickable(true);
                    ChangeState(GameState.ChooseAttack);
                }
                    break;
        }
    }
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private const byte playerChangeState = 1;

    private void ChangeState(GameState newState)
    {
        if(Online == false)
        {
            State = newState;
            return;
        }
        if (this.NextState == newState)
            return;

        // Kirim pesan bahwa kita ready
        var actorNum = PhotonNetwork.LocalPlayer.ActorNumber;
        var raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All
        };
        PhotonNetwork.RaiseEvent(1, actorNum, raiseEventOptions, SendOptions.SendReliable);

        this.State = GameState.SyncState;
        this.NextState = newState;
    }

    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case playerChangeState:
                var actorNum = (int)photonEvent.CustomData;
                
                // if (syncReadyPlayers.Contains(actorNum) == false)
                syncReadyPlayers.Add(actorNum);
                break;

            default:
                break;
        }
    }

    IEnumerator PingCoroutine()
    {
        var wait = new WaitForSeconds(2);
        while (true)
        {
            pingText.text = "Ping: " + PhotonNetwork.GetPing() + " ms";
            yield return wait;
        }
    }

    private void ResetPlayers()
    {
        damagedPlayer = null;
        P1.Reset();
        P2.Reset();
    }
    private CardPlayer GetDamagedPlayer()
    {
        Attack? PlayerAtk1 = P1.AttackValue;
        Attack? PlayerAtk2 = P2.AttackValue;
        if (PlayerAtk1 == Attack.Rock && PlayerAtk2 == Attack.Paper)
        {
            return P1;
        }
        else if (PlayerAtk1 == Attack.Rock && PlayerAtk2 == Attack.Scissor)
        {
            return P2;
        }
        else if (PlayerAtk1 == Attack.Paper && PlayerAtk2 == Attack.Rock)
        {
            return P2;
        }
        else if (PlayerAtk1 == Attack.Paper && PlayerAtk2 == Attack.Scissor)
        {
            return P1;
        }
        else if (PlayerAtk1 == Attack.Scissor && PlayerAtk2 == Attack.Rock)
        {
            return P1;
        }
        else if (PlayerAtk1 == Attack.Scissor && PlayerAtk2 == Attack.Paper)
        {
            return P2;
        }

        return null;
    }
    
    private CardPlayer GetWinner()
    {
        if (P1.Health == 0)
        {
            return P2;
        }

        else if (P2.Health == 0)
        {
            return P1;
        }

        else { return null; }
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
