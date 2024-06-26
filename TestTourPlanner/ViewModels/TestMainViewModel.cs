using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Persistence.Repository;
using TourPlanner.ViewModels;
using Moq;
using TourPlanner.Models;
using TourPlanner.Persistence.Entities;
using System.Collections.ObjectModel;
using System.Windows;
using TourPlanner.Exceptions;
using TourPlanner.UtilsForUnittests;
using TourPlanner.ViewModels.Utils;
using System.Windows.Input;
using System.Collections;
using System.Net;
using System.Reflection;
using NUnit.Framework.Interfaces;

namespace TestTourPlanner.ViewModels
{
    public class TestMainViewModel
    {
        private Mock<ITourRepository> _mockTourRepository;
        private Mock<IMapCreator> _mockMapCreator;
        private Mock<IFileDialogService> _mockFileDialogService;
        private Mock<IMessageBoxService> _mockMessageBoxService;
        private Mock<IOpenRouteService> _mockOpenRouteService;
        private Mock<ITourListControlViewModel> _mockTourListControlViewModel;
        private Mock<ITourLogListControlViewModel> _mockTourLogListControlViewModel;
        private MainViewModel _mainViewModel;

        private readonly string _filePath = "dummy/file/path.csv";

        [SetUp]
        public void Setup()
        {
            _mockTourRepository = new Mock<ITourRepository>();

            // Mock the view models
            _mockTourListControlViewModel = new Mock<ITourListControlViewModel>();
            _mockTourLogListControlViewModel = new Mock<ITourLogListControlViewModel>();

            _mockTourRepository.Setup(x => x.GetAddressById(It.IsAny<int>())).Returns(new AddressEntity { Id = 0, City = "Test", Zip = 1234, Housenumber = "1", Street = "Test" }) ;
           
            _mockFileDialogService = new Mock<IFileDialogService>();
            _mockFileDialogService.Setup(f => f.OpenFile(It.IsAny<string>())).Returns(_filePath);
            _mockMessageBoxService = new Mock<IMessageBoxService>();
            _mockMapCreator = new Mock<IMapCreator>();
            _mockOpenRouteService = new Mock<IOpenRouteService>();

            _mainViewModel = new MainViewModel(
                _mockTourRepository.Object,
                _mockMapCreator.Object,
                _mockFileDialogService.Object,
                _mockMessageBoxService.Object,
                _mockTourListControlViewModel.Object,
                _mockTourLogListControlViewModel.Object
            );
        }

        [Test]
        public void TourList_NowExpandedTour_ShouldHandleEvent()
        {
            // Arrange
            TourModel tourModel = new()
            {
                Id = 1,
                Name = "Test",
                Description = "Test",
                FromAddress = "1234 City, Street 12, Country",
                ToAddress = "1234 City, Street 12, Country",
                TransportType = TourPlanner.Models.TransportType.car,
                Distance = 10,
                EstimatedTime = 300,
                Popularity = 0,
                IsNew = null
            };

            var tourViewModel = new TourViewModel(_mockTourRepository.Object, tourModel, _mockMapCreator.Object, _mockMessageBoxService.Object, _mockOpenRouteService.Object);
            var _mockChangeTourCommand = new Mock<ICommand>();
            var methodInfo = typeof(MainViewModel).GetMethod("TourList_NowExpandedTour", BindingFlags.NonPublic | BindingFlags.Instance);  // Refelxion to run private "TourList_NowExpandedTour", invoked by event
            _mockTourLogListControlViewModel.SetupGet(x => x.ChangeTourCommand).Returns(_mockChangeTourCommand.Object);

            // Act
            methodInfo.Invoke(_mainViewModel, new object[] { tourViewModel, EventArgs.Empty });

            // Assert
            Assert.That(_mainViewModel.ExpandedTour, Is.EqualTo(tourViewModel));
            _mockChangeTourCommand.Verify(command => command.Execute(tourViewModel), Times.Once);
        }


        [Test]
        public void SummarizeReportCommand_Executes()
        {
            // Act
            _mainViewModel.SummarizeReportCommand.Execute(null);

            // Assert
            _mockTourRepository.Verify(mock => mock.GenerateSummarizeReportAsync(), Times.Once);         
        }      

        [Test]
        public void ImportTourCommandActsExpectedWhen_Success()
        {
            // Arrange          
            var transportType = TourPlanner.Models.TransportType.car;
            var start = new double[] { 16.378317, 48.238992 };
            var end = new double[] { 12.99528, 47.82287 };
            var tourId = 1;

            _mockTourRepository.Setup(r => r.ImportTourAsync(_filePath))
                               .ReturnsAsync((transportType, start, end, tourId));

            _mockMapCreator.Setup(x => x.DownloadMapFromApi(It.IsAny<TourPlanner.Models.TransportType>(), It.IsAny<double[]>(), It.IsAny<double[]>()))
               .Returns(Task.CompletedTask);

            var mockLoadToursCommand = new Mock<ICommand>();
            _mockTourListControlViewModel.Setup(vm => vm.LoadToursCommand).Returns(mockLoadToursCommand.Object);
            // Act
            _mainViewModel.ImportTourCommand.Execute(null);

            // Assert
            _mockTourRepository.Verify(r => r.ImportTourAsync(_filePath), Times.Once);
            _mockMapCreator.Verify(m => m.DownloadMapFromApi(transportType, start, end), Times.Once);
            mockLoadToursCommand.Verify(vm => vm.Execute(null), Times.Once);
        }

        [Test]
        public void ImportTourCommandActsExpectedWhen_UtilsException()
        {
            // Arrange
            var transportType = TourPlanner.Models.TransportType.car;
            var start = new double[] { 16.378317, 48.238992 };
            var end = new double[] { 12.99528, 47.82287 };
            var tourId = 1;

            _mockTourRepository.Setup(r => r.ImportTourAsync(_filePath))
                               .ReturnsAsync((transportType, start, end, tourId));
            
            _mockMapCreator.Setup(r => r.DownloadMapFromApi(transportType, start, end))
                               .Throws(new UtilsException());

            var mockLoadToursCommand = new Mock<ICommand>();
            _mockTourListControlViewModel.Setup(vm => vm.LoadToursCommand).Returns(mockLoadToursCommand.Object);

            // Act
            _mainViewModel.ImportTourCommand.Execute(null);

            // Assert
            _mockTourRepository.Verify(r => r.ImportTourAsync(_filePath), Times.Once);
            _mockTourRepository.Verify(r => r.DeleteTourByIdAsync(tourId), Times.Once);
            mockLoadToursCommand.Verify(vm => vm.Execute(null), Times.Never);
        }
    }
}
