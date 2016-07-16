using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using DevDaysSpeakers.Model;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using AppServiceHelpers.Abstractions;
using System.Runtime.CompilerServices;

namespace DevDaysSpeakers.ViewModel
{
	public class SpeakersViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged([CallerMemberName] string name = null)
		{
			var changed = PropertyChanged;
			if (changed == null)
				return;

			changed.Invoke (this, new PropertyChangedEventArgs(name));
		}

		bool busy;

		public bool IsBusy
		{
			get { return busy; }
			set
			{
				busy = value;
				OnPropertyChanged ();

				GetSpeakersCommand.ChangeCanExecute ();
			}
		}

		public ObservableCollection<Speaker> Speakers { get; set; }

		public Command GetSpeakersCommand { get; set; }


		public SpeakersViewModel()
		{
			Speakers = new ObservableCollection<Speaker> ();

			GetSpeakersCommand = new Command(
				async () => await GetSpeakers (),
				() => !IsBusy);
		}

		async Task GetSpeakers()
		{
			if (IsBusy)
				return;

			Exception error = null;

			try {
				IsBusy = true;

				using (var client  = new HttpClient())
				{
					var json = await client.GetStringAsync ("http://demo4404797.mockable.io/speakers");

					var items = JsonConvert.DeserializeObject<List<Speaker>> (json);

					Speakers.Clear ();

					foreach (var item in items) {
						Speakers.Add (item);
					}
				}
			}
			catch (Exception ex) {
				error = ex;
			}
			finally {
				IsBusy = false;
			}

			if (error != null)
				await Application.Current.MainPage.DisplayAlert ("Error!", error.Message, "OK");
		}
	}
}
