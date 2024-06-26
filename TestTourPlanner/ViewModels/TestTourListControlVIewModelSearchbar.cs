using NUnit.Framework;
using Moq;
using System.Collections.ObjectModel;
using TourPlanner.ViewModels;
using TourPlanner.Models;
using System.Linq;
using System.Reflection;

namespace TestTourPlanner.ViewModels
{
    [TestFixture]
    public class TestTourListControlViewModelSearchbar
    {
        private TourListControlViewModel _viewModel;

        [SetUp]
        public void Setup()
        {
            _viewModel = new TourListControlViewModel();

            // Add sample tours
            _viewModel.Tours = new ObservableCollection<TourViewModel>
            {
                new TourViewModel(new TourModel { Name = "Tour 1", Description = "Beautiful tour", FromAddress = "Vienna", ToAddress = "Graz", Popularity = 10, ChildFriendliness = 2 }),
                new TourViewModel(new TourModel { Name = "Tour 2", Description = "Challenging tour", FromAddress = "Salzburg", ToAddress = "Linz", Popularity = 5, ChildFriendliness = 3 }),
                new TourViewModel(new TourModel { Name = "Tour 3", Description = "Relaxing tour", FromAddress = "Innsbruck", ToAddress = "Bregenz", Popularity = 7, ChildFriendliness = 4 }),
                new TourViewModel(new TourModel { Name = "Tour 4", Description = "Scenic tour", FromAddress = "Klagenfurt", ToAddress = "Villach", Popularity = 10, ChildFriendliness = 5 })
            };
            
            // Add sample tour logs
            _viewModel.Tours[0].TourLogs = new ObservableCollection<TourLogViewModel>
            {
                new TourLogViewModel(new TourLogModel { Comment = "Great weather, enjoyable tour" }),
                new TourLogViewModel(new TourLogModel { Comment = "Sunny and beautiful" })
            };

            _viewModel.Tours[1].TourLogs = new ObservableCollection<TourLogViewModel>
            {
                new TourLogViewModel(new TourLogModel { Comment = "Very challenging, lots of hills" }),
                new TourLogViewModel(new TourLogModel { Comment = "Tough but rewarding" })
            };

            _viewModel.Tours[2].TourLogs = new ObservableCollection<TourLogViewModel>
            {
                new TourLogViewModel(new TourLogModel { Comment = "Relaxing and calm" }),
                new TourLogViewModel(new TourLogModel { Comment = "Perfect for a lazy afternoon" })
            };

            _viewModel.Tours[3].TourLogs = new ObservableCollection<TourLogViewModel>
            {
                new TourLogViewModel(new TourLogModel { Comment = "Scenic views, highly recommend" }),
                new TourLogViewModel(new TourLogModel { Comment = "Absolutely stunning landscapes" })
            };
        }

        private void InvokePerformSearch(TourListControlViewModel viewModel)
        {
            MethodInfo performSearchMethod = typeof(TourListControlViewModel).GetMethod("PerformSearch", BindingFlags.NonPublic | BindingFlags.Instance);
            performSearchMethod.Invoke(viewModel, null);
        }

        [Test]
        public void PerformSearch_ShouldReturnAllTours_WhenSearchTextIsEmpty()
        {
            // Arrange
            _viewModel.SearchText = "";

            // Act
            InvokePerformSearch(_viewModel);

            // Assert
            Assert.AreEqual(4, _viewModel.FilteredTours.Count);
        }

        [Test]
        public void PerformSearch_ShouldReturnMatchingTours_ByName()
        {
            // Arrange
            _viewModel.SearchText = "Tour 1";

            // Act
            InvokePerformSearch(_viewModel);

            // Assert
            Assert.AreEqual(1, _viewModel.FilteredTours.Count);
            Assert.AreEqual("Tour 1", _viewModel.FilteredTours.First().Name);
        }

        [Test]
        public void PerformSearch_ShouldReturnMatchingTours_ByDescription()
        {
            // Arrange
            _viewModel.SearchText = "Challenging";

            // Act
            InvokePerformSearch(_viewModel);

            // Assert
            Assert.AreEqual(1, _viewModel.FilteredTours.Count);
            Assert.AreEqual("Tour 2", _viewModel.FilteredTours.First().Name);
        }

        [Test]
        public void PerformSearch_ShouldReturnMatchingTours_ByFromAddress()
        {
            // Arrange
            _viewModel.SearchText = "Salzburg";

            // Act
            InvokePerformSearch(_viewModel);

            // Assert
            Assert.AreEqual(1, _viewModel.FilteredTours.Count);
            Assert.AreEqual("Tour 2", _viewModel.FilteredTours.First().Name);
        }

        [Test]
        public void PerformSearch_ShouldReturnMatchingTours_ByToAddress()
        {
            // Arrange
            _viewModel.SearchText = "Villach";

            // Act
            InvokePerformSearch(_viewModel);

            // Assert
            Assert.AreEqual(1, _viewModel.FilteredTours.Count);
            Assert.AreEqual("Tour 4", _viewModel.FilteredTours.First().Name);
        }
        
        [Test]
        public void PerformSearch_ShouldRankFilteredToursByPopularityAndChildFriendliness()
        {
            // Arrange
            _viewModel.SearchText = "tour";

            // Act
            InvokePerformSearch(_viewModel);

            // Assert
            Assert.AreEqual(4, _viewModel.FilteredTours.Count);
            Assert.AreEqual("Tour 4", _viewModel.FilteredTours[0].Name);
            Assert.AreEqual("Tour 1", _viewModel.FilteredTours[1].Name);
            Assert.AreEqual("Tour 3", _viewModel.FilteredTours[2].Name);
            Assert.AreEqual("Tour 2", _viewModel.FilteredTours[3].Name);
        }
    
        [Test]
        public void PerformSearch_ShouldReturnEmpty_WhenNoMatch()
        {
            // Arrange
            _viewModel.SearchText = "NonExistentTour";

            // Act
            InvokePerformSearch(_viewModel);

            // Assert
            Assert.AreEqual(0, _viewModel.FilteredTours.Count);
        }

        [Test]
        public void PerformSearch_ShouldReturnMatchingTours_ByCommentInTourLog()
        {
            // Arrange
            _viewModel.SearchText = "great weather";

            // Act
            InvokePerformSearch(_viewModel);

            // Assert
            Assert.AreEqual(1, _viewModel.FilteredTours.Count);
            Assert.AreEqual("Tour 1", _viewModel.FilteredTours.First().Name);
        }

        [Test]
        public void PerformSearch_ShouldReturnMatchingTours_ByCommentInMultipleTourLogs()
        {
            // Arrange
            _viewModel.SearchText = "stunning";

            // Act
            InvokePerformSearch(_viewModel);

            // Assert
            Assert.AreEqual(1, _viewModel.FilteredTours.Count);
            Assert.AreEqual("Tour 4", _viewModel.FilteredTours.First().Name);
        }
    }
}
