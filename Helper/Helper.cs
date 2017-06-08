using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Net;
using Java.IO;

namespace XamarinWeatherApp.Helper
{
    public class Helper
    {
        static String stream = null;
        public Helper() { }

        public String GetHTTPData(String urlString)
        {
            try{
                URL url = new URL(urlString);
                using(var urlConnection = (HttpURLConnection)url.OpenConnection())
                {
                    if(urlConnection.ResponseCode == HttpStatus.Ok)
                    {
                        BufferedReader r = new BufferedReader(new InputStreamReader(urlConnection.InputStream));
                        StringBuilder sb = new StringBuilder();
                        String line;
                        while ((line = r.ReadLine()) != null)
                            sb.Append(line);
                        stream = sb.ToString();
                        urlConnection.Disconnect();
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return stream;
        }

    }
}