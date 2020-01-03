
using UnityEngine;
using Bolt;
using System;
using UdpKit;
using UnityEngine.SceneManagement;
using Bolt.Matchmaking;
using TMPro;
using Photon;
using Photon.Realtime;
using Bolt.Photon;
using Bolt.Tokens;

public class Menu : Bolt.GlobalEventListener
{
    bool offlineMode = false;


    GameObject main;
    GameObject options;
    GameObject multiplayer;

    PhotonInit photonInit;



    private void Start()
    {

        photonInit = new PhotonInit();


        main = GameObject.Find("Main");
        options = GameObject.Find("Options");
        multiplayer = GameObject.Find("Multiplayer");

        main.SetActive(true);
        options.SetActive(false);
        multiplayer.SetActive(false);


    }

    

    public void StartServer()
    {






        BoltLauncher.StartServer();
    }

    public void StartSinglePlayer()
    {
        BoltLauncher.StartSinglePlayer();
        offlineMode = true;
    }


    public void StartClient()
    {
        BoltLauncher.StartClient();

    }


    public override void BoltStartDone()
    {


        // If we pressed join game, join a random session
        if (BoltNetwork.IsClient)
        {
            BoltMatchmaking.JoinRandomSession();
        }
        else if (offlineMode)
        {
            BoltNetwork.LoadScene(GameData.ScenesInBuild.scenes[1]);
        }
        
    }

    public void Shutdown()
    {
        BoltLauncher.Shutdown();
    }


    public void StartSession()
    {
        if (BoltNetwork.IsServer && !offlineMode)
        {

            string matchName = "Test Match";



            // Broadcast Server now ( now people can join)
            BoltMatchmaking.CreateSession(sessionID: matchName, sceneToLoad: GameData.ScenesInBuild.scenes[1]);


        }
        else
        {

            Debug.Log("First start an online server please.");
        }
    }


    public void OpenOptions()
    {
        options.SetActive(true);
        main.SetActive(false);
    }

    public void OpenMultiplayer()
    {
        main.SetActive(false);
        multiplayer.SetActive(true);
    }

    public void BackToMain()
    {
        main.SetActive(true);
        multiplayer.SetActive(false);
        options.SetActive(false);
    }

    public void ChangeRegion(int value)
    {
        if(value == 0)
        {
            photonInit.ChangeMasterServer("usw");
        }else if(value == 1)
        {
            photonInit.ChangeMasterServer("us");
        }else if (value == 2)
        {
            photonInit.ChangeMasterServer("ru");
        }else if (value == 3)
        {
            photonInit.ChangeMasterServer("asia");
        }else if (value == 4)
        {
            photonInit.ChangeMasterServer("eu");
        }
    }

}


