using Caliburn.Micro;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using WpfSmartHomeMonitoringApp.Helpers;

namespace WpfSmartHomeMonitoringApp.ViewModels
{
	public class RealTimeViewModel : Screen
	{
		// 온도
		private string livingTempVal;
		private string diningTempVal;
		private string bedTempVal;
		private string bathTempVal;

		public string LivingTempVal
		{
			get => livingTempVal;

			set
			{
				livingTempVal = value;
				NotifyOfPropertyChange(() => LivingTempVal);
			}
		}
		public string DinningTempVal
		{
			get => diningTempVal;

			set
			{
				diningTempVal = value;
				NotifyOfPropertyChange(() => DinningTempVal);

			}
		}
		public string BedTempVal
		{
			get => bedTempVal;

			set
			{
				bedTempVal = value;
				NotifyOfPropertyChange(() => BedTempVal);

			}
		}
		public string BathTempVal
		{
			get => bathTempVal;

			set
			{
				bathTempVal = value;
				NotifyOfPropertyChange(() => BathTempVal);

			}
		}

		//습도
		private string livingHumidVal;
		private string dinningHumidVal;
		private string bedHumidVal;
		private string bathHumidVal;

		public string LivingHumidVal
		{
			get => livingHumidVal; set
			{
				livingHumidVal = value;
				NotifyOfPropertyChange(() => LivingHumidVal);

			}
		}
		public string DinningHumidVal
		{
			get => dinningHumidVal; set
			{
				dinningHumidVal = value;
				NotifyOfPropertyChange(() => DinningHumidVal);

			}
		}
		public string BedHumidVal
		{
			get => bedHumidVal; set
			{
				bedHumidVal = value;
				NotifyOfPropertyChange(() => BedHumidVal);

			}
		}
		public string BathHumidVal
		{
			get => bathHumidVal; set
			{
				bathHumidVal = value;
				NotifyOfPropertyChange(() => BathHumidVal);

			}
		}

		public RealTimeViewModel()
		{
			Commons.BROKERHOST = "127.0.0.1";
			Commons.PUB_TOPIC = "home/device/#";

			LivingTempVal = DinningTempVal = BedTempVal = BathTempVal = "";
			LivingHumidVal = DinningHumidVal = BedHumidVal = BathHumidVal = "";

			if (Commons.MQTT_CLINET != null && Commons.MQTT_CLINET.IsConnected)
				Commons.MQTT_CLINET.MqttMsgPublishReceived += MQTT_CLINET_MqttMsgPublishReceived;
			else // 접속안되있으면
			{
				// MQTT Broker에 접속하는 내용
				Commons.MQTT_CLINET = new MqttClient(Commons.BROKERHOST);
				Commons.MQTT_CLINET.MqttMsgPublishReceived += MQTT_CLINET_MqttMsgPublishReceived;
				Commons.MQTT_CLINET.Connect("MONITOR");
				Commons.MQTT_CLINET.Subscribe( new string[] { Commons.PUB_TOPIC }
					, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });

				Commons.IS_CONNECT = true;
			}
		}

		// MQTT 데이터 온도계 / 습도계로 분배
		private void MQTT_CLINET_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
		{
			var message = Encoding.UTF8.GetString(e.Message);
			var currDatas = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);

			switch (currDatas["DevId"].ToString())
			{
				case "LIVING":
					LivingTempVal = double.Parse(currDatas["Temp"]).ToString("0.#");
					LivingHumidVal = double.Parse(currDatas["Temp"]).ToString("0.#");
					break;

				case "DINNING":
					DinningTempVal = double.Parse(currDatas["Temp"]).ToString("0.#");
					DinningHumidVal = double.Parse(currDatas["Temp"]).ToString("0.#");
					break;

				case "BED":
					BedTempVal = double.Parse(currDatas["Temp"]).ToString("0.#");
					BedHumidVal = double.Parse(currDatas["Temp"]).ToString("0.#");
					break;

				case "BATH":
					BathTempVal = double.Parse(currDatas["Temp"]).ToString("0.#");
					BathHumidVal = double.Parse(currDatas["Temp"]).ToString("0.#");
					break;
			}
		}
	}
}
