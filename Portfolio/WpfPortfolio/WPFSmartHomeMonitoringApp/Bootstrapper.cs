using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using WpfSmartHomeMonitoringApp.ViewModels;

namespace WpfSmartHomeMonitoringApp
{
	public class Bootstrapper : BootstrapperBase
	{
		private SimpleContainer container;
		public Bootstrapper()
		{
			Initialize();
		}

		protected override void BuildUp(object instance)
		{
			//base.BuildUp(instance);
			container.BuildUp(instance);
		}

		/// <summary>
		/// 초기 정의
		/// </summary>
		/// <param name="service"></param>
		/// <returns></returns>
		protected override void Configure()
		{
			//base.Configure();
			container = new SimpleContainer();

			container.Singleton<IWindowManager, WindowManager>();
			container.Singleton<IEventAggregator, EventAggregator>();

			container.PerRequest<MainViewModel>();

		}

		protected override IEnumerable<object> GetAllInstances(Type service)
		{
			return container.GetAllInstances(service);
		}

		protected override object GetInstance(Type service, string key)
		{
			return container.GetInstance(service, key);
		}

		protected override void OnStartup(object sender, StartupEventArgs e)
		{
			//base.OnStartup(sender, e);
			DisplayRootViewFor<MainViewModel>();
		}
	}
}
