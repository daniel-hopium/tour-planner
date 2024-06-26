using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Models;

namespace TourPlanner.ViewModels.Utils
{
    public interface IOpenRouteService
    {
        Task<(double[] coordinates, bool success)> GetParametersFromApi(string text);
        Task<Bitmap> GetTileAsync(int zoom, double x_tile, double y_tile);
        Task<(double[][] coordinates, double[] bbox, double distance, double duration)> GetDirectionsFromApi(TransportType transportType, string start, string end);
    }
}
