
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class NetworkManager : MonobehaviourSingleton<NetworkManager>
{
    [SerializeField] private NetworkSceneManager m_NetworkSceneManager;
    [SerializeField] private ConnectionController controller;
    [SerializeField] private string sceneName = "GameMenu";
    [field: SerializeField] public RegionsRegistry RegionsRegistry { get; private set; }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == sceneName)
        {
            controller.m_IsTestConnection = true;
            controller.m_IsJoiningRoom = false;
            print("Values reset");
        }
    }


    [ContextMenu("Load Gameplay")]
    public void LoadGameplay(string sceneName)
    {
        m_NetworkSceneManager.LoadGameplayScene(sceneName,1f);
    }

    public void SetStatus(string status)
    {
        GameEvents.NetworkEvents.NetworkStatus.Raise(status);
    }
}
