using System.Drawing;
using System.Reflection;
using System.Windows.Media.Imaging;
using Moq;
using TourPlanner.Models;
using TourPlanner.Persistence.Entities;
using TourPlanner.Persistence.Repository;
using TourPlanner.UtilsForUnittests;
using TourPlanner.ViewModels;
using TourPlanner.ViewModels.Utils;

namespace TestTourPlanner.ViewModels;

public class TestTourViewModel
{
  private Mock<IMapCreator> _mockMapCreator;
  private Mock<IMessageBoxService> _mockMessageBoxService;
  private Mock<IOpenRouteService> _mockOpenRouteService;
  private Mock<ITourRepository> _mockTourRepository;
  private TourViewModel _tourViewModel;

  [SetUp]
  public void Setup()
  {
    _mockTourRepository = new Mock<ITourRepository>();
    _mockMessageBoxService = new Mock<IMessageBoxService>();
    _mockMapCreator = new Mock<IMapCreator>();
    _mockOpenRouteService = new Mock<IOpenRouteService>();

    _mockTourRepository.Setup(x => x.GetAddressByAttributes(
      It.IsAny<string>(),
      It.IsAny<string>(),
      It.IsAny<int>(),
      It.IsAny<string>(),
      It.IsAny<string>()
    )).Returns(new AddressEntity
      { Id = 1, Zip = 1234, City = "City", Street = "Street", Housenumber = "12", Country = "Country" });

    _mockOpenRouteService.SetupSequence(mock => mock.GetParametersFromApi(It.IsAny<string>()))
      .ReturnsAsync((new[] { 1.0, 2.0 }, true)) // Success for coordinatesStart
      .ReturnsAsync((new[] { 3.0, 4.0 }, true)); // Success for coordinatesEnd

    _mockOpenRouteService.Setup(mock => mock.GetDirectionsFromApi(
      It.IsAny<TransportType>(),
      It.IsAny<string>(),
      It.IsAny<string>()
    )).ReturnsAsync((
      new[] { new[] { 1.0, 2.0 }, new[] { 3.0, 4.0 } }, // Example coordinates
      new[] { 0.0, 0.0, 0.0, 0.0 }, // Example bbox
      1000.0, // Example distance
      600.0 // Example duration
    ));

    var expectedBitmap = new Bitmap(100, 100);
    _mockMapCreator.Setup(mock => mock.GenerateImageAsync(
      It.IsAny<double[][]>(),
      It.IsAny<double[]>(),
      It.IsAny<double[]>(),
      It.IsAny<double[]>()
    )).ReturnsAsync(expectedBitmap);

    _mockMapCreator.Setup(mock => mock.SaveMap(
      It.IsAny<Bitmap>(),
      It.IsAny<string>(),
      It.IsAny<string>(),
      It.IsAny<string>()
    )).Verifiable();

    _mockMapCreator.Setup(mock => mock.ConvertBitmapToBitmapImage(
      It.IsAny<Bitmap>()
    )).Returns(new BitmapImage());

    TourModel tourModel = new()
    {
      Name = "Test",
      Description = "Test",
      FromAddress = "1234 City, Street 12, Country",
      ToAddress = "1234 City, Street 12, Country",
      TransportType = TransportType.car,
      Distance = 10,
      EstimatedTime = 300,
      Popularity = 0
    };

    _tourViewModel = new TourViewModel(_mockTourRepository.Object, tourModel, _mockMapCreator.Object,
      _mockMessageBoxService.Object, _mockOpenRouteService.Object);

    var bitmapField = typeof(TourViewModel).GetField("_bitmap", BindingFlags.NonPublic | BindingFlags.Instance);
    bitmapField.SetValue(_tourViewModel, new Bitmap(1, 1));
  }


  [Test]
  public void Name_PropertyChangedRaised()
  {
    var propertyChangedRaised = false;
    _tourViewModel.PropertyChanged += (sender, args) =>
    {
      if (args.PropertyName == nameof(_tourViewModel.Name)) propertyChangedRaised = true;
    };

    _tourViewModel.Name = "New Tour Name";

    Assert.That(propertyChangedRaised, Is.True);
  }

  [Test]
  public void InvalidName_RaisesErrorsChangedEvent()
  {
    var propertyName = nameof(_tourViewModel.Name);
    var errorsChangedRaised = false;
    _tourViewModel.ErrorsChanged += (sender, args) =>
    {
      if (args.PropertyName == propertyName) errorsChangedRaised = true;
    };

    _tourViewModel.Name = "";

    Assert.That(errorsChangedRaised, Is.True);
  }

  [Test]
  public void InvalidAddress_HasErrorsIsTrue()
  {
    _tourViewModel.FromAddress = "Invalid Address";

    Assert.That(_tourViewModel.HasErrors, Is.True);
  }

  [Test]
  public void SaveCommand_WhenTourHasErrors_DoesNotCallRepository()
  {
    _tourViewModel.Name = "";

    _tourViewModel.SaveCommand.Execute(null);

    _mockTourRepository.Verify(mock => mock.CreateTourAsync(It.IsAny<TourEntity>()), Times.Never);
    _mockTourRepository.Verify(mock => mock.UpdateTourAsync(It.IsAny<TourEntity>()), Times.Never);
  }

  [Test]
  public void SaveTour_WhenTourIsNew_CallsCreateTourAsyncFromRepository()
  {
    _tourViewModel.IsNew = true;

    _tourViewModel.SaveCommand.Execute(null);

    _mockTourRepository.Verify(mock => mock.CreateTourAsync(It.IsAny<TourEntity>()), Times.Once);
    _mockTourRepository.Verify(mock => mock.UpdateTourAsync(It.IsAny<TourEntity>()), Times.Never);
  }

  [Test]
  public void GetErrors_NameIsEmpty_ReturnsError()
  {
    // Arrange
    _tourViewModel.Name = "";

    // Act
    var errors = _tourViewModel.GetErrors("Name").Cast<string>().ToList();

    // Assert
    Assert.That(errors.Count, Is.EqualTo(1));
    Assert.That(errors[0], Is.EqualTo("Name can not be empty"));
  }

  [Test]
  public void GetErrors_DescriptionIsEmpty_ReturnsError()
  {
    // Arrange
    _tourViewModel.Description = "";

    // Act
    var errors = _tourViewModel.GetErrors("Description").Cast<string>().ToList();

    // Assert
    Assert.That(errors.Count, Is.EqualTo(1));
    Assert.That(errors[0], Is.EqualTo("Description can not be empty"));
  }

  [Test]
  public void GetErrors_FromAddressIsEmpty_ReturnsError()
  {
    // Arrange
    _tourViewModel.FromAddress = "";

    // Act
    var errors = _tourViewModel.GetErrors("FromAddress").Cast<string>().ToList();

    // Assert
    Assert.That(errors.Count, Is.EqualTo(1));
    Assert.That(errors[0], Is.EqualTo("From-Address can not be empty"));
  }

  [Test]
  public void GetErrors_ToAddressIsEmpty_ReturnsError()
  {
    // Arrange
    _tourViewModel.ToAddress = "";

    // Act
    var errors = _tourViewModel.GetErrors("ToAddress").Cast<string>().ToList();

    // Assert
    Assert.That(errors.Count, Is.EqualTo(1));
    Assert.That(errors[0], Is.EqualTo("To-Address can not be empty"));
  }

  [Test]
  public void GetErrors_FromAddressIsInvalid_ReturnsError()
  {
    // Arrange
    _tourViewModel.FromAddress = "Invalid Address";

    // Act
    var errors = _tourViewModel.GetErrors("FromAddress").Cast<string>().ToList();

    // Assert
    Assert.That(errors.Count, Is.EqualTo(1));
    Assert.That(
      errors[0],
      Is.EqualTo("'1234 City, Street 12, Country' (street&country or street&zip and streetnumber can be omitted)"));
  }

  [Test]
  public void GetErrors_ToAddressIsInvalid_ReturnsError()
  {
    // Arrange
    _tourViewModel.ToAddress = "Invalid Address";

    // Act
    var errors = _tourViewModel.GetErrors("ToAddress").Cast<string>().ToList();

    // Assert
    Assert.That(errors.Count, Is.EqualTo(1));
    Assert.That(
      errors[0],
      Is.EqualTo("'1234 City, Street 12, Country' (street&country or street&zip and streetnumber can be omitted)"));
  }

  [Test]
  public void GetErrors_ValidFromAddress_ReturnsNoError()
  {
    // Arrange
    _tourViewModel.FromAddress = "1234 City, Street 12, Country";

    // Act
    var errors = _tourViewModel.GetErrors("FromAddress").Cast<string>().ToList();

    // Assert
    Assert.That(errors.Count, Is.EqualTo(0));
  }

  [Test]
  public void GetErrors_ValidToAddress_ReturnsNoError()
  {
    // Arrange
    _tourViewModel.ToAddress = "1234 City, Street 12, Country";

    // Act
    var errors = _tourViewModel.GetErrors("ToAddress").Cast<string>().ToList();

    // Assert
    Assert.That(errors.Count, Is.EqualTo(0));
  }

  [Test]
  public void GetErrors_UnknownProperty_ReturnsNoError()
  {
    // Act
    var errors = _tourViewModel.GetErrors("UnknownProperty").Cast<string>().ToList();

    // Assert
    Assert.That(errors.Count, Is.EqualTo(0));
  }
}