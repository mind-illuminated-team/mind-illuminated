

using System.Collections.Generic;
using System.Text;

namespace Sensors {

    public static class SensorDataConverter
    {

        public static ushort StringToUshort(string data)
        {
            ushort.TryParse(data, out ushort newData);
            return newData;
        }

        
        // Converts a list of uint16 values to a new line separated list in a string
        public static string SensorDataListToString(List<ushort> sensorData)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Sensor data\n");

            sensorData.ForEach(d => sb.Append(d).Append("\n"));

            return sb.ToString();
        }

    }

}

