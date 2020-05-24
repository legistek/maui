﻿using System;
using System.Maui;
using System.Maui.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace TestApp
{
	public partial class App : Application
	{
		public App()
		{
			System.Maui.Presentation.Framework.Initialize();
			InitializeComponent();
			MainPage = new MainPage();
			int a = 1;
			a = 5 + 5;
			Math.Max(a, 1);
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
