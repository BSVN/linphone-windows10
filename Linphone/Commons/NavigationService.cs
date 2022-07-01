using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace BelledonneCommunications.Linphone.Commons
{
	public interface INavigationService
	{
		bool CanGoBack { get; }
		void GoBack();
		void Navigate<T>(object args = null);
	}

	internal class NavigationService : INavigationService
	{
		private readonly Frame frame;

		public NavigationService(Frame frame)
		{
			this.frame = frame;
		}

		public bool CanGoBack => this.frame.CanGoBack;

		public void GoBack() => this.frame.GoBack();

		public void Navigate<T>(object args)
		{
			this.frame.Navigate(typeof(T), args);
		}
	}
}
