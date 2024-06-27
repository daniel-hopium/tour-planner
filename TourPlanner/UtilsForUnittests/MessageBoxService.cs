using System.Windows;

namespace TourPlanner.UtilsForUnittests;

public interface IMessageBoxService
{
  void Show(string message);
}

public class MessageBoxService : IMessageBoxService
{
  public void Show(string message)
  {
    MessageBox.Show(message);
  }
}