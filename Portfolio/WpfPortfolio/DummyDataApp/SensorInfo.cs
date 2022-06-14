using System;

namespace DummyDataApp
{
    class SensorInfo
    {
        public string DevId { get; set; } //기기 ID
        public DateTime CurrTime { get; set; } //현재 일시
        public float Temp { get; set; } //온도 
        public float Humid { get; set; } //습도 
    }
}
