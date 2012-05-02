/////////////////////////////////////////////////////////////////////////////////////////
// define GPS EMULATOR when working with Windows Phone GPS Emulator to simulate location 
#define GPS_EMULATOR
////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Device.Location;
using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;

namespace CardioMapper
{
    public partial class WorkoutPage : PhoneApplicationPage
    {
#if GPS_EMULATOR
        private GpsEmulatorClient.GeoCoordinateWatcher watcher;
#else
        private System.Device.Location.GeoCoordinateWatcher watcher;
#endif

        private bool isWatcherReady;

        private Pushpin youAreHerePin;

        private MapPolyline polyline;

        private Workout workout;

        public WorkoutPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Initialize map controls.
            Map.ZoomLevel = 18.0;
            Map.Children.Clear();

            this.youAreHerePin = new Pushpin();
            this.youAreHerePin.Content = "You are here";
            this.youAreHerePin.Visibility = Visibility.Collapsed;
            this.youAreHerePin.MouseLeftButtonUp += new MouseButtonEventHandler(youAreHerePin_MouseLeftButtonUp);
            Map.Children.Add(this.youAreHerePin);

            this.polyline = new MapPolyline();
            this.polyline.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
            this.polyline.StrokeThickness = 5;
            this.polyline.Opacity = 0.7;
            this.polyline.Locations = new LocationCollection();
            Map.Children.Add(this.polyline);

            // Initialize workout object.
            this.workout = new Workout();

            // Initialize geo watcher.
            InitGeoWatcher();

            // Start geo watcher.
            this.watcher.Start();

            // Mark the geo-watcher as not being ready.
            this.isWatcherReady = false;
        }

        private void InitGeoWatcher()
        {
#if GPS_EMULATOR
            this.watcher = new GpsEmulatorClient.GeoCoordinateWatcher(GeoPositionAccuracy.High);
#else
            this.watcher = new System.Device.Location.GeoCoordinateWatcher(GeoPositionAccuracy.High);
#endif
            this.watcher.MovementThreshold = 20;
            this.watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
            this.watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
        }

        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:

                    GPSStatus.Text = "Disabled";

                    // The Location Service is disabled or unsupported.
                    // Check to see whether the user has disabled the Location Service.
                    if (watcher.Permission == GeoPositionPermission.Denied)
                    {
                        // The user has disabled the Location Service on their device.
                        // TODO display error message and turn watcher off
                    }
                    else
                    {
                        // Location is not functioning on this device.
                        // TODO display error message and turn watcher off
                    }

                    this.isWatcherReady = false;

                    break;

                case GeoPositionStatus.Initializing:

                    // The Location Service is initializing.
                    GPSStatus.Text = "Initializing";

                    this.isWatcherReady = false;

                    break;

                case GeoPositionStatus.NoData:

                    // The Location Service is working, but it cannot get location data.
                    GPSStatus.Text = "NoData";

                    this.isWatcherReady = false;

                    break;

                case GeoPositionStatus.Ready:

                    // The Location Service is working and is receiving location data.
                    GPSStatus.Text = "Ready";

                    // Position the "You are here" pin.
                    this.youAreHerePin.Location = Map.Center;
                    this.youAreHerePin.Visibility = Visibility.Visible;

                    this.isWatcherReady = true;

                    // Update the position if it's available.
                    if (!this.watcher.Position.Location.IsUnknown)
                    {
                        this.UpdatePosition(this.watcher.Position);
                    }

                    break;
            }
        }

        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            this.UpdatePosition(e.Position);
        }

        private void UpdatePosition(GeoPosition<GeoCoordinate> position)
        {
            this.UpdatePosition(position, DateTimeOffset.MinValue);
        }

        private void UpdatePosition(GeoPosition<GeoCoordinate> position, DateTimeOffset forcedTimestamp)
        {
            //// NOTE if a lot of work needs to be done, fire off another thread, b/c this runs the UI thread

            if (this.isWatcherReady)
            {
                // Get the current coordinates, altitude, speed, and timestamp.
                double latitude = position.Location.Latitude;
                double longitude = position.Location.Longitude;
                double altitude = position.Location.Altitude;
                double speed = position.Location.Speed;
                DateTimeOffset timestamp;
                if (forcedTimestamp != DateTimeOffset.MinValue)
                {
                    timestamp = forcedTimestamp;
                }
                else
                {
                    timestamp = position.Timestamp;
                }

                if (!latitude.Equals(Double.NaN) && !longitude.Equals(Double.NaN))
                {
                    // Center the map and move the push pin to the current location.
                    GeoCoordinate coord = new GeoCoordinate(latitude, longitude);
                    Map.Center = coord;
                    this.youAreHerePin.Location = coord;
                    this.polyline.Locations.Add(coord);
                }

                // Update the workout object.
                Location.Text = string.Format("Location: ({0}, {1})", latitude, longitude);
                Altitude.Text = string.Format("Altitude: {0} meters", altitude);
                Timestamp.Text = string.Format("Timestamp: {0}", timestamp);
                CurrentTime.Text = string.Format("Current time: {0}", this.workout.ComputeCurrentTime(timestamp));
                LastTime.Text = string.Format("Last time: {0}", this.workout.LastTime);

                double d = this.workout.Update(latitude, longitude, altitude, speed, timestamp);

                Time.Text = string.Format("Time: {0}", this.workout.Time);
                CurrentDistance.Text = string.Format("Current distance: {0}", d);
                Distance.Text = string.Format("Distance: {0}", this.workout.Distance);
                MaxSpeed.Text = string.Format("Max speed: {0}", this.workout.MaxSpeed);
                Speed.Text = string.Format("Speed: {0}", this.workout.Speed);
                AverageSpeed.Text = string.Format("Average speed: {0}", this.workout.AverageSpeed);
            }
        }

        void youAreHerePin_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // do nothing for now
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            // Update the position if it's available.
            if (!this.watcher.Position.Location.IsUnknown)
            {
                this.UpdatePosition(this.watcher.Position, DateTimeOffset.Now);
            }

            // Save the current workout.
            App.CurrentWorkout = this.workout;

            // Stop the geo watcher.
            this.watcher.Stop();

            // Disable the stop button.
            this.StopButton.IsEnabled = false;
        }
    }
}