using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Maui;
using System.Maui.Xaml;

namespace TestApp
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
			this.BindingContext = this;
		}

		Command _AddPersonCommand;
		public Command AddPersonCommand
		{
			get => _AddPersonCommand ?? (_AddPersonCommand = new Command(() =>
			{
				this.People.Add("New Person");
			}));
		}

		Command _RemovePersonCommand;
		public Command RemovePersonCommand
		{
			get => _RemovePersonCommand ?? (_RemovePersonCommand = new Command(() =>
			{
				this.People.RemoveAt(0);
			}));
		}

		public ObservableCollection<string> People { get; } = new ObservableCollection<string>
		{
			"Larry",
			"Curly",
			"Moe"
		};
	}
}
