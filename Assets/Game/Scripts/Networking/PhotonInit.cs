using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt.Matchmaking;
using Bolt.Photon;
using Bolt.Utils;
using UdpKit;
using UdpKit.platform.photon;

public class PhotonInit : Bolt.GlobalEventListener
{
    void Awake()
    {
        // Set Bolt to use Photon as transport layer
        // this will connect to Photon using config values from Bolt's settings window


        BoltLauncher.SetUdpPlatform(new PhotonPlatform());

    }


    public void ChangeMasterServer(string regionToken)
    {
        BoltLauncher.SetUdpPlatform(new PhotonPlatform(new PhotonPlatformConfig
        {
            AppId = "8e1f090e-e0ff-4166-9ebe-6693470ecc07", // your App ID
            Region = PhotonRegion.GetRegion(regionToken), // your desired region
            UsePunchThrough = true, // enable the punch through feature
            RoomUpdateRate = 5, // session update rate
            RoomCreateTimeout = 10, // timeout when creating a room
            RoomJoinTimeout = 10 // timeout when joining a room
        }));
    }
}
