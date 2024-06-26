using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TourPlanner.Models;
using TourPlanner.Persistence.Entities;
using TourPlanner.Persistence.Repository;
using TourPlanner.UtilsForUnittests;
using TourPlanner.ViewModels;
using TourPlanner.ViewModels.Utils;

namespace TestRourPlanner.ViewModels
{
    [Apartment(ApartmentState.STA)] // to be able to process WPF-Componente (TreeViewItem)
    public class TestTourListControlViewModel
    {
        private TourListControlViewModel _tourListControlViewModel;
        private Mock<ITourRepository> _mockTourRepository;
        private Mock<IMessageBoxService> _mockMessageBoxService;
        private TourViewModel _tourViewModel;
        private TreeViewItem _treeViewItem;

        [SetUp]
        public void Setup()
        {
            _mockTourRepository = new Mock<ITourRepository>();
            _mockMessageBoxService = new Mock<IMessageBoxService>();
            _tourListControlViewModel = new TourListControlViewModel(_mockTourRepository.Object, _mockMessageBoxService.Object);

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

            _tourViewModel = new TourViewModel(_mockTourRepository.Object, tourModel, new Mock<IMapCreator>().Object, _mockMessageBoxService.Object, new Mock<IOpenRouteService>().Object);
            _tourListControlViewModel.Tours.Add(_tourViewModel);

            _treeViewItem = new TreeViewItem
            {
                DataContext = _tourViewModel
            };
        }


        [Test]  
        public void DeleteTour_CallsDeleteTourByIdAsyncFromRepository()
        {
            // Act
            _tourListControlViewModel.TourDeleteCommand.Execute(_treeViewItem);

            // Assert
            _mockTourRepository.Verify(mock => mock.DeleteTourByIdAsync(_tourViewModel.Id), Times.Once);
        }

        [Test]
        public void LoadToursCommand_LoadsTours_Successfully()
        {
            // Arrange
            var address = new AddressEntity() { Zip = 1234, City = "City", Street = "Street", Housenumber = "12", Country = "Country" };
            var mockTourEntity = new TourEntity() { Id = 1, Name = "Test Tour", Description = "Test", FromAddress = address, ToAddress = address, Distance = 10, EstimatedTime = 300, TransportType = "car", Popularity = 0};
            
            _mockTourRepository.Setup(r => r.GetTours()).Returns(new List<TourEntity> { mockTourEntity });            

            // Act
            _tourListControlViewModel.LoadToursCommand.Execute(null);

            // Assert
            Assert.That(_tourListControlViewModel.Tours.Count, Is.EqualTo(1));
            Assert.That(_tourListControlViewModel.Tours.First().Name, Is.EqualTo(mockTourEntity.Name));
        }

        [Test]
        public void ExpandTourCommand_ExpandsOneTour_Successfully()
        {
            // Arrange
            TourModel tourModel2 = new()
            {
                Id = 2,
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
            var tourViewModel2 = new TourViewModel(_mockTourRepository.Object, tourModel2, new Mock<IMapCreator>().Object, _mockMessageBoxService.Object, new Mock<IOpenRouteService>().Object); 
            
            _tourListControlViewModel.Tours.Add(tourViewModel2);

            tourViewModel2.IsExpanded = true;

            // Act
            _tourListControlViewModel.ExpandedCommand.Execute(_treeViewItem);

            // Assert
            Assert.That(_tourListControlViewModel.ExpandedTour, Is.EqualTo(_tourViewModel));
            Assert.That(tourViewModel2.IsExpanded, Is.False);
        }

        [Test]
        public void EditTourCommand_SetTourToEdit_Successfully()
        {
            // Arrange
            var address = new AddressEntity() { Zip = 1234, City = "City", Street = "Street", Housenumber = "12", Country = "Country" };
            var mockTourEntity = new TourEntity() { Id = 1, Name = "Test Tour", Description = "Test", FromAddress = address, ToAddress = address, Distance = 10, EstimatedTime = 300, TransportType = "car", Popularity = 0 };

            _mockTourRepository.Setup(r => r.GetTourByIdAsync(It.IsAny<int>())).ReturnsAsync(mockTourEntity);

            var eventRaised = false;
            _tourListControlViewModel.TourToEditSelected += (sender, args) => eventRaised = true;

            // Act
            _tourListControlViewModel.TourEditCommand.Execute(_treeViewItem);

            // Assert
            Assert.That(eventRaised, Is.True);
        }

    }
}
