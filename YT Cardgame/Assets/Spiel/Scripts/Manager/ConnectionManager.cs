using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectionManager : MonoBehaviour
{
    public static event Action<ulong> ClientConnectedEvent;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectedCallback;
        NetworkManager.Singleton.OnServerStopped += OnServerStopped;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectedCallback;
            NetworkManager.Singleton.OnServerStopped -= OnServerStopped;
        }
    }

    private void OnClientConnectedCallback(ulong clientId)
    {
        Debug.Log("Client" + clientId + "connected");

        if (!NetworkManager.Singleton.IsServer) return;
        ClientConnectedEvent?.Invoke(clientId);
    }

    private void OnClientDisconnectedCallback(ulong clientId)
    {
        Debug.Log("Client" + clientId + "disconnected");

        if (!NetworkManager.Singleton.IsServer)
        {
            SceneManager.LoadScene("MainMenu");
        }

    }

    private void OnServerStopped(bool wasClient)
    {
        Debug.Log("Server stopped");
        SceneManager.LoadScene("MainMenu");
    }

}
