using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace TourPlanner.ViewModels.Utils
{
    public interface IMapCreator
    {
        Task<Bitmap> GenerateImageAsync(double[][] coordinates, double[] bbox, double[] coordinatesStart, double[] coordinatesEnd);
        void SaveMap(Bitmap map, string transport, string start, string end);
        Task DownloadMapFromApi(Models.TransportType transportType, double[] start, double[] end);
        BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap);
    }
}
