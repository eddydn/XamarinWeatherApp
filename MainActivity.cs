using Android.App;
using Android.Widget;
using Android.OS;
using Android.Locations;
using XamarinWeatherApp.Model;
using Android.Runtime;
using System;
using Newtonsoft.Json;
using Square.Picasso;
using Android.Content;

namespace XamarinWeatherApp
{
    [Activity(Label = "XamarinWeatherApp", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class MainActivity : Activity,ILocationListener
    {
        TextView txtCity, txtLastUpdate, txtDescription, txtHumidity, txtTime, txtCelsius;
        ImageView imgView;

        LocationManager locationManager;
        string provider;
        static double lat, lng;
        OpenWeatherMap openWeatherMap = new OpenWeatherMap();
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            locationManager = (LocationManager)GetSystemService(Context.LocationService);
            provider = locationManager.GetBestProvider(new Criteria(), false);

            Location location = locationManager.GetLastKnownLocation(provider);
            if (location == null)
                System.Diagnostics.Debug.WriteLine("No location");
        }


        protected override void OnResume()
        {
            base.OnResume();
            locationManager.RequestLocationUpdates(provider, 400, 1, this);
        }

        protected override void OnPause()
        {
            base.OnPause();
            locationManager.RemoveUpdates(this);
        }

        public void OnLocationChanged(Location location)
        {
            lat = Math.Round(location.Latitude, 4);
            lng = Math.Round(location.Longitude, 4);

            new GetWeather(this, openWeatherMap).Execute(Common.Common.APIRequest(lat.ToString(), lng.ToString()));
        }

        public void OnProviderDisabled(string provider)
        {
           
        }

        public void OnProviderEnabled(string provider)
        {
            
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
          
        }

        private class GetWeather : AsyncTask<String, Java.Lang.Void, String>
        {
            private ProgressDialog pd = new ProgressDialog(Application.Context);
            private MainActivity activity;
            OpenWeatherMap openWeatherMap;

            public GetWeather(MainActivity activity,OpenWeatherMap openWeatherMap)
            {
                this.activity = activity;
                this.openWeatherMap = openWeatherMap;
            }

            protected override void OnPreExecute()
            {
                base.OnPreExecute();
                pd.Window.SetType(Android.Views.WindowManagerTypes.SystemAlert);
                pd.SetTitle("Please wait...");
                pd.Show();
            }
            protected override string RunInBackground(params string[] @params)
            {
                string stream = null;
                string urlString = @params[0];

                Helper.Helper http = new Helper.Helper();
                //urlString = Common.Common.APIRequest(lat.ToString(), lng.ToString()
                stream = http.GetHTTPData(urlString);
                return stream;
            }

            protected override void OnPostExecute(string result)
            {
                base.OnPostExecute(result);
                if(result.Contains("Error: Not found city"))
                {
                    pd.Dismiss();
                    return;
                }
                openWeatherMap = JsonConvert.DeserializeObject<OpenWeatherMap>(result);
                pd.Dismiss();

                //Control
                activity.txtCity = activity.FindViewById<TextView>(Resource.Id.txtCity);
                activity.txtLastUpdate = activity.FindViewById<TextView>(Resource.Id.txtLastUpdate);
                activity.txtDescription = activity.FindViewById<TextView>(Resource.Id.txtDescription);
                activity.txtHumidity = activity.FindViewById<TextView>(Resource.Id.txtHumidity);
                activity.txtTime = activity.FindViewById<TextView>(Resource.Id.txtTime);
                activity.txtCelsius = activity.FindViewById<TextView>(Resource.Id.txtCelsius);
                
                activity.imgView = activity.FindViewById<ImageView>(Resource.Id.imageView);

                //Add data
                activity.txtCity.Text = $"{openWeatherMap.name},{openWeatherMap.sys.country}";
                activity.txtLastUpdate.Text = $"Last Updated: {DateTime.Now.ToString("dd MMMM yyyy HH:mm")}";
                activity.txtDescription.Text = $"{openWeatherMap.weather[0].description}";
                activity.txtHumidity.Text = $"Humidity: {openWeatherMap.main.humidity} %";
                activity.txtTime.Text = $"{Common.Common.UnixTimeStampToDateTime(openWeatherMap.sys.sunrise).ToString("HH:mm")}/{Common.Common.UnixTimeStampToDateTime(openWeatherMap.sys.sunset).ToString("HH:mm")}";
                activity.txtCelsius.Text = $"{openWeatherMap.main.temp}°C";

                if (!String.IsNullOrEmpty(openWeatherMap.weather[0].icon))
                {
                    Picasso.With(activity.ApplicationContext)
                        .Load(Common.Common.GetImage(openWeatherMap.weather[0].icon))
                        .Into(activity.imgView);
                }
            }
        }

    }
}

