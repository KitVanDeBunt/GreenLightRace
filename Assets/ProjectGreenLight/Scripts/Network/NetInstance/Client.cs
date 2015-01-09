using UnityEngine;

class Client : NetInstance
{
    public Client(NetworkStateManager nsm): base(nsm)
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
