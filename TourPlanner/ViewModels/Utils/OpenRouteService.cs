using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.IO;
using TourPlanner.Models;

namespace TourPlanner.ViewModels.Utils
{
    public class Tile
    {
        public int X { get; }
        public int Y { get; }

        public Tile(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public static class OpenRouteService
    {
        private static readonly HttpClient client = new HttpClient();

        public static (double[] coordinates, double[] bbox, bool success) GetParametersFromApi(string api_key, string text)
        {
            try
            {
                string apiUrl = $"https://api.openrouteservice.org/geocode/search?api_key={api_key}&text={Uri.EscapeDataString(text)}";
                
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                //using HttpResponseMessage response = client.Get(apiUrl);

                var response = client.Send(request);
                response.EnsureSuccessStatusCode();
                string responseBody = response.Content.ReadAsStringAsync().Result; ;

                dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);

                double[] coordinates = ((JArray)jsonResponse.features.First.geometry.coordinates).Select(jv => (double)jv).ToArray();
                double[] bbox = ((JArray)jsonResponse.bbox).Select(jv => (double)jv).ToArray();

                return (coordinates, bbox, true);
            }
            catch (HttpRequestException e)
            {
                double[] coordinates = { 0.0, 0.0 };
                double[] bbox = { 0.0, 0.0, 0.0, 0.0 };
                return (coordinates, bbox, false);
            }
        }      

        public static Bitmap GetTile(int zoom, double x_tile, double y_tile) 
        {
            string tileUrl = $"https://tile.openstreetmap.org/{zoom}/{x_tile}/{y_tile}.png";

            var request = new HttpRequestMessage(HttpMethod.Get, tileUrl);
            request.Headers.Add("User-Agent", "TourPlanner/1.0");

            var response = client.Send(request);
            if (response.IsSuccessStatusCode)
            {
                using (Stream responseStream = response.Content.ReadAsStream())
                {
                    return new Bitmap(responseStream);
                }
            }
            else
            {
                return CreateDefaultBitmap();
            }
        }

        private static Bitmap CreateDefaultBitmap()
        {
            int width = 256;
            int height = 256;
            Bitmap defaultBitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(defaultBitmap))
            {
                g.Clear(Color.Gray); // Set background color to gray
                g.DrawString("No Image", new Font("Arial", 16), Brushes.Red, new PointF(50, 100));
            }
            return defaultBitmap;
        }

        public static (double[][] coordinates, double[] bbox, double distance, double duration) GetDirectionsFromApi(string api_key, TransportType transportType, string start, string end)
        {
            string transport = string.Empty;
            switch (transportType)
            {
                case TransportType.bike: transport = "cycling-regular"; break;
                case TransportType.hike: transport = "foot-hiking"; break;
                case TransportType.running: transport = "foot-walking"; break;
                case TransportType.car: transport = "driving-car"; break;
            }

            // Beispiel-URL der API, die die Parameter liefert
            string apiUrl = $"https://api.openrouteservice.org/v2/directions/{transport}?api_key={api_key}&start={Uri.EscapeDataString(start)}&end={Uri.EscapeDataString(end)}";
            
            /*var response = await client.GetStringAsync(apiUrl);
             
            dynamic jsonResponse = JsonConvert.DeserializeObject(response);*/

            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            //using HttpResponseMessage response = client.Get(apiUrl);

            var response = client.Send(request);
            response.EnsureSuccessStatusCode();
            string responseBody = response.Content.ReadAsStringAsync().Result; ;

            /*using HttpResponseMessage response = await client.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();*/

            dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);

            double[][] coordinates = ((JArray)jsonResponse.features.First.geometry.coordinates).Select(jv => jv.Select(coord => (double)coord).ToArray()).ToArray();
            double[] bbox = ((JArray)jsonResponse.bbox).Select(jv => (double)jv).ToArray();
            double distance = jsonResponse.features.First.properties.segments.First.distance;
            double duration = jsonResponse.features.First.properties.segments.First.duration; 

            return (coordinates, bbox, distance, duration);
        }
    }
}
