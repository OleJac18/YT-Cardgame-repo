using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectionManager : NetworkBehaviour
{
    public static event Action AlLClientsConnectedAndSceneLoadedEvent;
    private string gameplaySceneName = "Gameplay";
    private int requiredClients = 2;

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
        GameManager.RestartGameEvent += LoadGameplayScene;
        MainMenu.HostSuccessfullyStartedEvent += SubscribeToSceneEvent;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectedCallback;
            NetworkManager.Singleton.OnServerStopped -= OnServerStopped;
        }

        GameManager.RestartGameEvent -= LoadGameplayScene;
        MainMenu.HostSuccessfullyStartedEvent -= SubscribeToSceneEvent;
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnSceneEvent;
    }

    private void OnClientConnectedCallback(ulong clientId)
    {
        Debug.Log("Client" + clientId + "connected");

        if (!NetworkManager.Singleton.IsServer) return;
        
        int connectedClients = NetworkManager.Singleton.ConnectedClients.Count;

        if (connectedClients >= requiredClients)
        {
            Debug.Log("Genügend Clients verbunden. Starte Szenenwechsel...");
            LoadGameplayScene();
        }
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

    private void LoadGameplayScene()
    {
        NetworkManager.Singleton.SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);
    }

    private void SubscribeToSceneEvent()
    {
        NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;
    }

    private void OnSceneEvent(SceneEvent  sceneEvent)
    {
        if(sceneEvent.SceneName == gameplaySceneName
            && sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted)
        {
            StartCoroutine(StartInitializationDelayed());
        }
    }

    private IEnumerator StartInitializationDelayed()
    {
        yield return new WaitForSeconds(0.5f);
        AlLClientsConnectedAndSceneLoadedEvent?.Invoke();
    }

}
