using System;

namespace TourPlanner.Exceptions;

public class UtilsException : Exception
{
  public UtilsException()
  {
  }

  public UtilsException(string message)
    : base(message)
  {
  }

  public UtilsException(string message, Exception innerException)
    : base(message, innerException)
  {
  }
}