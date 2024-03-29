﻿namespace CardioMapper
{
    using System;
    using System.Windows;
    using Microsoft.Phone.Controls;

    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        private void NewWorkoutButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/WorkoutPage.xaml", UriKind.Relative));
        }
    }
}