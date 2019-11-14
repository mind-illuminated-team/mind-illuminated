using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class SensorDataProvider : MonoBehaviour, IUDPDataListener
{
    public Text text;

    private List<ushort> sensorData;

    public void Listen(string data)
    {
        ushort.TryParse(data, out ushort newData);
        sensorData.Add(newData);
    }

    void Start()
    {
        sensorData = new List<ushort>();
        UDPConnection.Instance.RegisterListener(this);
    }

    void Update()
    {
        if (text != null && sensorData.Count > 0)
        {
            text.text = sensorData[sensorData.Count - 1].ToString();
        }
    }
}
