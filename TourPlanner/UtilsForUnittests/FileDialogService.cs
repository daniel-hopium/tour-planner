using Microsoft.Win32;

namespace TourPlanner.UtilsForUnittests;

public interface IFileDialogService
{
  string OpenFile(string filter);
}

public class FileDialogService : IFileDialogService
{
  public string OpenFile(string filter)
  {
    var openFileDialog = new OpenFileDialog
    {
      Filter = filter
    };
    return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : null;
  }
}