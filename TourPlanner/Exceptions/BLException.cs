using System;

namespace TourPlanner.Exceptions;

public class BLException : Exception
{
  public BLException()
  {
  }

  public BLException(string message)
    : base(message)
  {
  }

  public BLException(string message, Exception innerException)
    : base(message, innerException)
  {
  }
}