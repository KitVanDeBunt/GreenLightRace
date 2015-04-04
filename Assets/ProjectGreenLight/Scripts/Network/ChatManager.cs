
namespace Noir.Network
{
    public class ChatManager
    {
        private UnityEngine.MonoBehaviour monoBehaviour_;
        private NetworkPlayerList playerManager_;
        private static System.Collections.Generic.List<Message> messages_;

        public static Message[] messages
        {
            get
            {
                return messages_.ToArray();
            }
        }

        public ChatManager(UnityEngine.MonoBehaviour monoBehaviour, NetworkPlayerList playerManager)
        {
            messages_ = new System.Collections.Generic.List<Message>();
            monoBehaviour_ = monoBehaviour;
            playerManager_ = playerManager;
        }

        public void SendChatMessage(string message)
        {
            RPCChatMessage(message);
            monoBehaviour_.GetComponent<UnityEngine.NetworkView>().RPC("RPCChatMessage", UnityEngine.RPCMode.Others, message);
        }

        public void RPCChatMessage(string message, UnityEngine.NetworkMessageInfo info)
        {
            messages_.Add(new Message(message,playerManager_.GetNoirNetworkPlayer(info.sender).name,false));
            EventManager.callOnNetEvent(Events.Net.NEW_MESSAGE);
        }

        public void RPCChatMessage(string message)
        {
            messages_.Add(new Message(message, Settings.Player.name,true));
            EventManager.callOnNetEvent(Events.Net.NEW_MESSAGE);
        }
    }
}
