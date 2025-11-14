using UnityEngine;
using Photon.Pun;

public class GlobalRPC : MonoBehaviourPun
{
    public static GlobalRPC Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SendCameraRPC(string method, params object[] args)
    {
        photonView.RPC(method, RpcTarget.All, args);
    }
}
