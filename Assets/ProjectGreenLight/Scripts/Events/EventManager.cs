public class EventManager
{
    //EventManager.callOnGuiInput(guiData[currentActive].buttons[i].message);

    public delegate void NetEvent(Events.Net message);
    public static event NetEvent OnNetEvent;

    public static void callOnNetEvent(Events.Net message)
    {
        if (OnNetEvent != null)
        {
            Console.Log("[Net Event]:" + message.ToString());
            OnNetEvent(message);
        }
    }


    public delegate void GuiEvent(Events.GUI message);
    public static event GuiEvent OnGuiEvent;

    public static void callOnGuiEvent(Events.GUI message)
    {
        if (OnGuiEvent != null)
        {
            Console.Log("[Gui Event]:"+message.ToString());
            OnGuiEvent(message);
        }
    }
}