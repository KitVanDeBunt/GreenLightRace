#if !UNITY_WEBGL
using UnityEngine;

class Client : NetInstance
{
    public Client(NetworkStateManager nsm, MonoBehaviour monoBehaviour)
        : base(nsm, monoBehaviour)
    {

    }

    public override void Init()
    {
        base.Init();
    }

    public override void Close()
    {
        base.Close();
    }
}
#endif
