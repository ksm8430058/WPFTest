using System;

namespace WpfSmartHomeMonitoringApp.Models
{
	class SmartHomeModel
	{
		public string DevId { get; set; }
		public DateTime CurrTime { get; set; }
		public double Temp { get; set; }
		public double Humid { get; set; }
	}
}
