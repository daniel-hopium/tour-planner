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

namespace TestRourPlanner.ViewModels
{
    public class TestMainViewModel
    {
        private Mock<ITourRepository> _mockTourRepository;
        private MainViewModel _mainViewModel;

        [SetUp]
        public void Setup()
        {
            _mockTourRepository = new Mock<ITourRepository>();
            _mockTourRepository.Setup(x => x.GetAddressById(It.IsAny<int>())).Returns(new AddressEntity { Id = 0, City = "Test", Zip = 1234, Housenumber = "1", Street = "Test" }) ;
            _mainViewModel = new MainViewModel(_mockTourRepository.Object);
        }

        [Test]
        public void CreateTour_SetsFormTourToNewTourViewModel()
        {
            _mainViewModel.TourCreateCommand.Execute(null);

            Assert.IsNotNull(_mainViewModel.FormTour);
            Assert.IsTrue(_mainViewModel.FormTour.Tour.IsNew);
        }

        [Test]
        public void SaveTour_WhenFormTourHasErrors_DoesNotCallRepository()
        {
            var tourViewModel = new TourViewModel(new TourModel());
            tourViewModel.Name = ""; 
            _mainViewModel.FormTour = tourViewModel;

            _mainViewModel.SaveCommand.Execute(null);

            _mockTourRepository.Verify(mock => mock.CreateTourAsync(It.IsAny<TourEntity>()), Times.Never);
            _mockTourRepository.Verify(mock => mock.UpdateTourAsync(It.IsAny<TourEntity>()), Times.Never);
        }

        [Test]
        public void SaveTour_WhenFormTourIsNew_CallsCreateTourAsyncFromRepository()
        {
            TourModel tourModel = new TourModel();
            tourModel.Name = "Test";
            tourModel.Description = "Test";
            tourModel.FromAddress = "Test 1, 1234 Test";
            tourModel.ToAddress = "Test 1, 1234 Test";
            tourModel.IsNew = true;

            var tourViewModel = new TourViewModel(tourModel);

            _mainViewModel.FormTour = tourViewModel;

            _mainViewModel.SaveCommand.Execute(null);

            _mockTourRepository.Verify(mock => mock.CreateTourAsync(It.IsAny<TourEntity>()), Times.Once);
        }

        [Test]
        public void SaveTour_WhenFormTourIsNotNew_CallsUpdateTourAsyncFromRepository()
        {
            TourModel tourModel = new TourModel();
            tourModel.Name = "Test";
            tourModel.Description = "Test";
            tourModel.FromAddress = "Test 1, 1234 Test";
            tourModel.ToAddress = "Test 1, 1234 Test";
            tourModel.IsNew = null;

            var tourViewModel = new TourViewModel(tourModel);
            _mainViewModel.FormTour = tourViewModel;

            _mainViewModel.SaveCommand.Execute(null);

            _mockTourRepository.Verify(mock => mock.UpdateTourAsync(It.IsAny<TourEntity>()), Times.Once);
        }

        [Test]
        public void DeleteTour_CallsDeleteTourByIdAsyncFromRepository()
        {
            TourModel tourModel = new TourModel();
            tourModel.Name = "Test";
            tourModel.Description = "Test";
            tourModel.FromAddress = "Test 1, 1234 Test";
            tourModel.ToAddress = "Test 1, 1234 Test";
            tourModel.IsNew = null;

            var tourViewModel = new TourViewModel(tourModel);
            //_mainViewModel.Tours = new ObservableCollection<TourViewModel> { tourViewModel };

            _mainViewModel.TourDeleteCommand.Execute(tourViewModel);

            _mockTourRepository.Verify(mock => mock.DeleteTourByIdAsync(tourViewModel.Id), Times.Once);
        }
    }
}
