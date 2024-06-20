using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static TourPlanner.ViewModels.Utils.MapCreator;
using TourPlanner.ViewModels.Utils;
using System.Collections;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace TourPlanner.ViewModels.Utils
{  
    public static class MapCreator
    {
        public enum Marker
        {
            PIN_RED_16px,
            PIN_RED_32px,
            MARKER_RED_16px,
            MARKER_RED_32px
        }

        public record GeoCoordinate(double lon, double lat) { }

        private static string _uri_project = $"C:\\Users\\anste\\Documents\\Informatik_Bachelor_2022-2025\\Informatik-4.SemesterSS24\\SWEN2\\tour_planer_da\\TourPlanner";

        public static int Zoom { get; set; } = 18;

        private static Tile Latlon2Tile(double lat_deg, double lon_deg, int zoom)  // bbox[3], bbox[0], zoom
        {
            double lat_rad = lat_deg * Math.PI / 180.0;
            double n = Math.Pow(2.0, zoom);
            int xTile = (int)Math.Floor((lon_deg + 180.0) / 360.0 * n);
            int yTile = (int)Math.Floor((1.0 - Math.Log(Math.Tan(lat_rad) + 1 / Math.Cos(lat_rad)) / Math.PI) / 2.0 * n);

            return new Tile(xTile, yTile);
        }

        private static System.Drawing.Point LatLonToPixel(double lat, double lon, int zoom)
        {
            double lat_rad = lat * Math.PI / 180.0;
            double n = Math.Pow(2.0, zoom);
            int x_pixel = (int)Math.Floor((lon + 180.0) / 360.0 * n * 256);
            int y_pixel = (int)Math.Floor((1.0 - Math.Log(Math.Tan(lat_rad) + 1 / Math.Cos(lat_rad)) / Math.PI) / 2.0 * n * 256);

            return new System.Drawing.Point(x_pixel, y_pixel);
        }


        public static Bitmap GenerateImage(double[][] coordinates, double[] bbox, double[] coordinatesStart, double[] coordinatesEnd)
        {
            List<GeoCoordinate> markers = new List<GeoCoordinate>();

            GeoCoordinate startMarker = new GeoCoordinate(coordinatesStart[0], coordinatesStart[1]);
            GeoCoordinate endMarker = new GeoCoordinate(coordinatesEnd[0], coordinatesEnd[1]); 
            markers.Add(startMarker);
            markers.Add(endMarker);

            int zoom = Zoom;

            const int MaxBitmapWidth = 3624;
            const int MaxBitmapHeight = 2048;
            const int TileSize = 256;

            double aspectRatio = 16.0 / 9.0;
            // current aspect ratio of the bounding box
            double currentAspectRatio = (bbox[2] - bbox[0]) / (bbox[3] - bbox[1]);

            double mal = 0.0; 
            double var3 = 0.0; 
            double z = 0.0; 

            // Calculate the tile numbers for each corner of the bounding box
            Tile topLeftTile = Latlon2Tile(bbox[3], bbox[0], Zoom); 
            Tile bottomRightTile = Latlon2Tile(bbox[1], bbox[2], Zoom); 

            // Determine the number of tiles to fetch in each dimension
            int tilesX = bottomRightTile.X - topLeftTile.X + 1;
            int tilesY = bottomRightTile.Y - topLeftTile.Y + 1;

            int currentWidth = tilesX * TileSize;
            int currentHeight = tilesY * TileSize;


            if (currentAspectRatio > aspectRatio)
            {
                mal = ((bbox[2] - bbox[0]) / aspectRatio) / (bbox[3] - bbox[1]);
                var3 = (bbox[3] - bbox[1]) * mal;
                z = var3 - (bbox[3] - bbox[1]);

                while (true)
                {
                    // Berechne die Kachelkoordinaten für den aktuellen Zoom-Level
                    topLeftTile = Latlon2Tile(bbox[3] + (z / 4), bbox[0], zoom);
                    bottomRightTile = Latlon2Tile(bbox[1] - (z / 2), bbox[2], zoom);

                    // Bestimme die Anzahl der Kacheln in X- und Y-Richtung
                    tilesX = bottomRightTile.X - topLeftTile.X + 1;
                    tilesY = bottomRightTile.Y - topLeftTile.Y + 1;

                    // Berechne die aktuelle Breite und Höhe der Bitmap
                    currentWidth = tilesX * 256;
                    currentHeight = tilesY * 256;

                    // Überprüfe, ob die aktuelle Breite und Höhe die maximal erlaubten Werte überschreiten
                    if (currentWidth <= MaxBitmapWidth && currentHeight <= MaxBitmapHeight)
                    {
                        break;  // Wenn die Abmessungen passen, brich die Schleife
                    }

                    zoom--;

                    if (zoom < 0)
                    {
                        throw new InvalidOperationException("Zoom level cannot be reduced further.");
                    }
                }              
            }
            else if(currentAspectRatio < aspectRatio) 
            {
                mal = (aspectRatio * (bbox[3] - bbox[1])) / (bbox[2] - bbox[0]);
                var3 = (bbox[2] - bbox[0]) * mal;
                z = var3 - (bbox[2] - bbox[0]);

                while (true)
                {
                    // Berechne die Kachelkoordinaten für den aktuellen Zoom-Level
                    topLeftTile = Latlon2Tile(bbox[3], bbox[0] - (z / 2), zoom);
                    bottomRightTile = Latlon2Tile(bbox[1], bbox[2] + z, zoom);

                    // Bestimme die Anzahl der Kacheln in X- und Y-Richtung
                    tilesX = bottomRightTile.X - topLeftTile.X + 1;
                    tilesY = bottomRightTile.Y - topLeftTile.Y + 1;

                    // Berechne die aktuelle Breite und Höhe der Bitmap
                    currentWidth = tilesX * 256;
                    currentHeight = tilesY * 256;

                    // Überprüfe, ob die aktuelle Breite und Höhe die maximal erlaubten Werte überschreiten
                    if (currentWidth <= MaxBitmapWidth && currentHeight <= MaxBitmapHeight)
                    {                       
                        break;  // Wenn die Abmessungen passen, brich die Schleife
                    }

                    zoom--;

                    if (zoom < 0)
                    {
                        throw new InvalidOperationException("Zoom level cannot be reduced further.");
                    }
                }            
            }


            System.Drawing.Point topLeftTilePixel = new System.Drawing.Point(topLeftTile.X * 256, topLeftTile.Y * 256);

            List<System.Drawing.Point> routePoints = new List<System.Drawing.Point>();

            foreach (var coord in coordinates)
            {
                double lon = coord[0];
                double lat = coord[1];
                var tile = Latlon2Tile(lat, lon, zoom);

                System.Drawing.Point globalPos = LatLonToPixel(lat, lon, zoom);
                System.Drawing.Point relativePos = new System.Drawing.Point(globalPos.X - topLeftTilePixel.X, globalPos.Y - topLeftTilePixel.Y);
                routePoints.Add(new System.Drawing.Point(relativePos.X, relativePos.Y));
            }

            Bitmap map = new Bitmap(tilesX * 256, tilesY * 256, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(map))
            {
                // Fetch and draw each tile
                for (int x = topLeftTile.X; x <= bottomRightTile.X; x++)
                {
                    for (int y = topLeftTile.Y; y <= bottomRightTile.Y; y++)
                    {
                        Bitmap tileImage = OpenRouteService.GetTile(zoom, x, y);
                        int xPos = (x - topLeftTile.X) * 256;
                        int yPos = (y - topLeftTile.Y) * 256;
                        g.DrawImage(tileImage, xPos, yPos);
                        //tileImage.Dispose();
                    }
                }

                // Draw route
                if (routePoints.Count > 1)
                {
                    System.Drawing.Pen redPen = new System.Drawing.Pen(System.Drawing.Color.Red);
                    redPen.Width = 7;

                    g.DrawLines(redPen, routePoints.ToArray());
                    redPen.Dispose();
                }


                // Draw markers
                foreach (var marker in markers)
                {
                    Bitmap markerIcon = null;
                    if (marker == startMarker)
                    {
                        markerIcon = LoadMarkerIcon(Marker.MARKER_RED_16px);
                    }
                    else
                    {
                        markerIcon = LoadMarkerIcon(Marker.MARKER_RED_32px);
                    }
                    System.Drawing.Point globalPos = LatLonToPixel(marker.lat, marker.lon, zoom);
                    System.Drawing.Point relativePos = new System.Drawing.Point(globalPos.X - topLeftTilePixel.X, globalPos.Y - topLeftTilePixel.Y);
                    g.DrawImage(markerIcon, relativePos);
                    markerIcon.Dispose();
                }
            }
            return map;
        }


        private static Bitmap LoadMarkerIcon(Marker marker)
        {
            string filename = marker switch
            {
                Marker.PIN_RED_16px => "pin-red_16px",
                Marker.PIN_RED_32px => "pin-red_32px",
                Marker.MARKER_RED_16px => "marker-red_16px",
                Marker.MARKER_RED_32px => "marker-red_32px",
                _ => throw new ArgumentException("Unknown marker type"),
            };

            string resourcePath = $"{_uri_project}\\Persistence\\Resources\\{filename}.png"; // Adjust this path accordingly

            return new Bitmap(resourcePath);
        }

        public static void SaveMap(Bitmap map, string transport, string start, string end)
        {
            string fileName = $"{_uri_project}\\Persistence\\Images\\map_{transport}_{start}_{end}.png";
            
            if (map != null && !File.Exists(fileName))
            {
                    map.Save(fileName, ImageFormat.Png);             
            }          
        }

        public static BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap) 
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                //memory.Seek(0, SeekOrigin.Begin);
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                bitmapImage.Freeze();
                return bitmapImage;
            }
        }
    }
}
