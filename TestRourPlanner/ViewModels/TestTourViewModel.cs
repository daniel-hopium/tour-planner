using NUnit.Framework;
using System.Linq;
using TourPlanner.Models;
using TourPlanner.ViewModels;


namespace TestRourPlanner.ViewModels
{
    public class TestTourViewModel
    {
        /*[SetUp]
        public void Setup()
        {

        }*/

        [Test]
        public void Name_PropertyChangedRaised()
        {
            var tour = new TourModel();
            var viewModel = new TourViewModel(tour);
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(viewModel.Name)) {
                    propertyChangedRaised = true;
                }
            };

            viewModel.Name = "New Tour Name";

            Assert.IsTrue(propertyChangedRaised);
        }

        [Test]
        public void InvalidName_RaisesErrorsChangedEvent()
        {
            var tour = new TourModel();
            var viewModel = new TourViewModel(tour);
            string propertyName = nameof(viewModel.Name);
            bool errorsChangedRaised = false;
            viewModel.ErrorsChanged += (sender, args) =>
            {
                if (args.PropertyName == propertyName) {
                    errorsChangedRaised = true;
                }
            };

            viewModel.Name = "";

            Assert.IsTrue(errorsChangedRaised);
        }

        [Test]
        public void InvalidAddress_HasErrorsIsTrue()
        {
            var tour = new TourModel();
            var viewModel = new TourViewModel(tour);

            viewModel.FromAddress = "Invalid Address";

            Assert.IsTrue(viewModel.HasErrors);
        }
    }
}