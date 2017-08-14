using UnityEngine;

public class EventDispatcher : MonoBehaviour
{
    public delegate void EventHandler(GameObject gameObject);
    public event EventHandler MouseDown;

    void OnMouseDown()
    {
        if (MouseDown != null)
        {
            MouseDown(this.gameObject);
        }
    }
}

