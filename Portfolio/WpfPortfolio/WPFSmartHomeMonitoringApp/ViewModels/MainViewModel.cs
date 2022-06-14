using Caliburn.Micro;
using System;
using System.Threading;
using System.Threading.Tasks;
using WpfSmartHomeMonitoringApp.Helpers;

namespace WpfSmartHomeMonitoringApp.ViewModels
{
	public class MainViewModel : Conductor<object> // Screen에는 ActivateItem[Async] 메서드 없음!
	{
		protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
		{
			if(Commons.MQTT_CLINET.IsConnected)
			{
				Commons.MQTT_CLINET.Disconnect();
				Commons.MQTT_CLINET = null;
			} // 비활성화 처리

			return base.OnDeactivateAsync(close, cancellationToken);
		}
		public MainViewModel()
		{
			DisplayName = "SmartHome Mpnitoring v2.0";  // 프로그램 타이틀, 이름
		}

		public void LoadDataBaseView()
		{
			ActivateItemAsync(new DataBaseViewModel());
		}

		public void LoadRealTimeView()
		{
			ActivateItemAsync(new RealTimeViewModel());
		}

		public void LoadHistoryView()
		{
			ActivateItemAsync(new HistoryViewModel());
		}

		public void Exit_Menu()
		{
			ExitProgram();
		}
		public void Exit_Toolbar()
		{
			ExitProgram();
		}

		private void ExitProgram()
		{
			Environment.Exit(0); // 프로그램 종료
		}


		// Start메뉴, 아이콘 눌렀을때 처리할 이벤트
		public void PopInfoDialog()
		{
			TaskPopup();
		}

		public void StartSubscribe()
		{
			TaskPopup();
		}

		private void TaskPopup()
		{
			//CustomPopupView
			var winManger = new WindowManager();
			var result = winManger.ShowDialogAsync(new CustomPopupViewModel("New Broker"));

			if (result.Result == true)
			{
				ActivateItemAsync(new DataBaseViewModel());
			}
		}
		public void PopInfoView()
        {
			var winManger = new WindowManager();
			winManger.ShowDialogAsync(new CustomInfoViewModel("About"));
        }
	}
}
