using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sensors
{
    public class SensorDataProvider : MonoBehaviour, IUDPDataListener
    {
        public Text text;

        private List<ushort> sensorData;

        private bool capturing;

        public void Listen(string data)
        {
            if (capturing)
            {
                sensorData.Add(SensorDataConverter.StringToUshort(data));
            }
        }

        void Awake()
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

        public ushort? GetLastData()
        {
            return sensorData.Count == 0 ? sensorData[sensorData.Count - 1] : (ushort?) null;
        }

        public void StartCapture()
        {
            capturing = true;
        }

        public void StopCapture()
        {
            capturing = false;
        }

        public List<ushort> GetAllData()
        {
            return sensorData;
        }

        public void ClearData()
        {
            sensorData.Clear();
        }

    }
}

