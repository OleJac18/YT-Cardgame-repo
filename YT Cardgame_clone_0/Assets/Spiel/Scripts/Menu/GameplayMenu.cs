using Unity.Netcode;
using UnityEngine;

public class GameplayMenu : MonoBehaviour
{
    public void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
    }
}
