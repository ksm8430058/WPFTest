using Caliburn.Micro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using WpfSmartHomeMonitoringApp.Helpers;
using WpfSmartHomeMonitoringApp.Models;

namespace WpfSmartHomeMonitoringApp.ViewModels
{
	public class DataBaseViewModel : Screen
	{
		private string brokerUrl;
		public string BrokerUrl
		{
			get { return brokerUrl; }
			set
			{
				brokerUrl = value;
				NotifyOfPropertyChange(() => BrokerUrl);
			}
		}

		private string topic;
		public string Topic
		{
			get { return topic; }
			set
			{
				topic = value;
				NotifyOfPropertyChange(() => Topic);
			}
		}

		private string connString;
		public string ConnString
		{
			get { return connString; }
			set
			{
				connString = value;
				NotifyOfPropertyChange(() => ConnString);
			}
		}

		private string dbLog;
		public string DbLog
		{
			get { return dbLog; }
			set
			{
				dbLog = value;
				NotifyOfPropertyChange(() => DbLog);
			}
		}


		private bool isConnected;
		public bool IsConnected
		{
			get { return isConnected; }

			set
			{
				isConnected = value;
				NotifyOfPropertyChange(() => IsConnected);
			}
		}


		public DataBaseViewModel()
		{
			BrokerUrl = Commons.BROKERHOST = "127.0.0.1";   // MQTT Broker IP설정
			Topic = Commons.PUB_TOPIC = "home/device/#";
			ConnString = Commons.CONNSTRING = "Data Source=PC01;Initial Catalog=OpenApiLab;Integrated Security=True";

			if (Commons.IS_CONNECT)
			{
				IsConnected = true;
				ConnectDb();
			}
		}

		/// <summary>
		/// DB연결 + MQTT Broker 접속
		/// </summary>
		public void ConnectDb()
		{
			if (IsConnected)
			{
				Commons.MQTT_CLINET = new MqttClient(BrokerUrl);

				try
				{
					if (Commons.MQTT_CLINET.IsConnected != true)
					{
						Commons.MQTT_CLINET.MqttMsgPublishReceived += MQTT_CLINET_MqttMsgPublishReceived;
						Commons.MQTT_CLINET.Connect("MONITOR");
						Commons.MQTT_CLINET.Subscribe(
							new string[] { Commons.PUB_TOPIC }
							, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });

						UpdateText(">>> MQTT Broker Connected");
						IsConnected = Commons.IS_CONNECT = true;
					}
				}
				catch (Exception ex)
				{

					//pass
				}

			}
			else    // 연결헤제
			{
				try
				{
					if (Commons.MQTT_CLINET.IsConnected)
					{
						Commons.MQTT_CLINET.MqttMsgPublishReceived -= MQTT_CLINET_MqttMsgPublishReceived;   // 이벤트연결 삭제
						Commons.MQTT_CLINET.Disconnect();
						UpdateText(">>>MQTT Broker Disconnected...");
						IsConnected = Commons.IS_CONNECT = false;
					}
				}
				catch (Exception ex)
				{

					//pass
				}
			}
		}

		private void UpdateText(string message)
		{
			DbLog += $"{message}\n";
		}


		/// <summary>
		/// Subscribe한 메시지 처리해주는 이벤트핸들러
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MQTT_CLINET_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
		{
			var message = Encoding.UTF8.GetString(e.Message);
			UpdateText(message);    // 센서데이터 출력
			SetDataBase(message, e.Topic);  // DB저장
		}

		private void SetDataBase(string message, string topic)
		{
			var currDatas = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);
			//
			var smartHomeModel = new SmartHomeModel();


			Debug.WriteLine(currDatas);

			smartHomeModel.DevId = currDatas["DevId"];
			smartHomeModel.CurrTime = DateTime.Parse(currDatas["CurrTime"]);
			smartHomeModel.Temp = double.Parse(currDatas["Temp"]);
			smartHomeModel.Humid = double.Parse(currDatas["Humid"]);


			using (SqlConnection conn = new SqlConnection(ConnString))
			{
				conn.Open();
				// verbatim string c#
				string strInQuery = @"INSERT INTO TblSmartHome
											   (DevId
											   , CurrTime
											   , Temp
											   , Humid)
										 VALUES
											   (@DevId
											   , @CurrTime
											   , @Temp
											   , @Humid)";

				try
				{
					SqlCommand cmd = new SqlCommand(strInQuery, conn);
					SqlParameter parmDevId = new SqlParameter("@DevId", smartHomeModel.DevId);
					cmd.Parameters.Add(parmDevId);
					SqlParameter parmCurrTime = new SqlParameter("@CurrTime", smartHomeModel.CurrTime);
					cmd.Parameters.Add(parmCurrTime);
					SqlParameter parmTemp = new SqlParameter("@Temp", smartHomeModel.Temp);
					cmd.Parameters.Add(parmTemp);
					SqlParameter parmHumid = new SqlParameter("@Humid", smartHomeModel.Humid);
					cmd.Parameters.Add(parmHumid);

					if (cmd.ExecuteNonQuery() == 1)
						UpdateText(">>> DB Inserted."); // 저장성공
					else
						UpdateText(">>> DB Failed!!!"); // 저장실패
				}
				catch (Exception ex)
				{
					UpdateText($">>> DB Error!!! : {ex.Message}"); // 예외
																   //Pass
				}
			}
		}
	}
}
