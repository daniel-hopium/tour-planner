using System;

namespace TourPlanner.Exceptions;

public class DALException : Exception
{
  public DALException()
  {
  }

  public DALException(string message)
    : base(message)
  {
  }

  public DALException(string message, Exception innerException)
    : base(message, innerException)
  {
  }
}