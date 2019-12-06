using System;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

namespace Sensors
{
    public class SensorDataProvider : Singleton<SensorDataProvider>, IUDPDataListener
    {
        private List<SensorData> sensorData;

        private Stopwatch stopwatch;

        public void Listen(string data)
        {
            if (stopwatch.IsRunning)
            {
                sensorData.Add(new SensorData(SensorDataConverter.StringToUshort(data), stopwatch.ElapsedMilliseconds));
            }
        }

        void Awake()
        {
            stopwatch = new Stopwatch();
            sensorData = new List<SensorData>();
            UDPConnection.Instance.RegisterListener(this);
        }

        public ushort? GetLastData()
        {
            return sensorData.Count == 0 ? sensorData[sensorData.Count - 1].Data : (ushort?) null;
        }

        public float? GetAverageSinceLastCheckPoint(double degree)
        {
            int size = sensorData.Count;
            if (size == 0)
            {
                return null;
            }
            SensorData lastData = sensorData[size - 1];
            lastData.CheckPoint = true;
            double sum = Math.Pow(lastData.Data,degree);
            if (size == 1)
            {
                return (float)sum;
            }

            int counter = 1;
            for (int i = size - 2; i > 0; i--)
            {
                SensorData data = sensorData[i];
                if (data.CheckPoint)
                {
                    break;
                }
                sum += Math.Pow(lastData.Data, degree);
                counter++;
            }

            // Normalized
            double average = Math.Pow(sum / counter, 1 / degree);

            //double average = sum / counter;

            return (float)(average);
        }

        public float? GetAverageOfLastSeconds(float seconds)
        {
            int size = sensorData.Count;
            if (size == 0 || seconds <= 0)
            {
                return null;
            }

            SensorData lastData = sensorData[size - 1];
            int sum = lastData.Data;
            if (size == 1)
            {
                return sum;
            }

            long milliseconds = Convert.ToInt64(seconds * 1000);
            float startTime = lastData.Time;
            int counter = 1;
            for (int i = size - 2; i > 0; i--)
            {
                SensorData data = sensorData[i];
                float delta = startTime - data.Time;
                if (delta > milliseconds)
                {
                    break;
                }
                sum += data.Data;
                counter++;
            }

            return sum / counter;
        }

        public void StartCapture()
        {
            stopwatch.Start();
        }

        public void StopCapture()
        {
            stopwatch.Stop();
            stopwatch.Reset();
        }

        public List<SensorData> GetAllData()
        {
            return sensorData;
        }

        public void ClearData()
        {
            sensorData.Clear();
        }

    }
}

