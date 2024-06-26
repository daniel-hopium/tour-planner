using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.UtilsForUnittests
{
    public interface IFileDialogService
    {
        string OpenFile(string filter);
    }


    public class FileDialogService : IFileDialogService
    {
        public string OpenFile(string filter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = filter
            };
            return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : null;
        }
    }
}
