using System.Collections.Generic;

namespace Sensors
{
    public class SensorDataProvider : Singleton<SensorDataProvider>, IUDPDataListener
    {

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

