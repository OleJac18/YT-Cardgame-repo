using System;
using Unity.Netcode;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public static event Action HostSuccessfullyStartedEvent;


    public void StartHost()
    {
        bool success = false;

        success = NetworkManager.Singleton.StartHost();

        if (success)
        {
            HostSuccessfullyStartedEvent?.Invoke();
        }
    }

    public void StartServer()
    {
        bool success = false;

        success = NetworkManager.Singleton.StartServer();

        if (success)
        {
            HostSuccessfullyStartedEvent?.Invoke();
        }
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
