public class SensorData
{
    // measurement from the sensor
    public ushort Data { get; set; }

    // time passed in milliseconds since capturing started
    public long Time { get; set; }

    public SensorData(ushort data, long time)
    {
        Data = data;
        Time = time;
    }
}
