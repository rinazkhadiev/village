using UnityEngine;

public class Analytics : MonoBehaviour
{
    public static Analytics Singleton { get; private set; }

    private void Start()
    {
        Singleton = this;
    }

    public void OnEvent(string message)
    {
        AppMetrica.Instance.ReportEvent(message);
    }
}
