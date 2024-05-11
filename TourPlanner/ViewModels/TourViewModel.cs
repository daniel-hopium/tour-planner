using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Models;

namespace TourPlanner.ViewModels
{
    public class TourViewModel : INotifyPropertyChanged
    {
        private TourModel _tour;
        private bool _isExpanded;

        public TourViewModel(TourModel tour)
        {
            _tour = tour;
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged(nameof(IsExpanded));
                }
            }
        }


        public int Id { 
            get {  return _tour.Id; }
            set {  _tour.Id = value;
                OnPropertyChanged(nameof(Id));
        }}
        public string Name { 
            get {  return _tour.Name; } 
            set {
                _tour.Name = value; 
                OnPropertyChanged(nameof(Name));
        }}
        public string Description { 
            get { return _tour.Description; }
            set {
                _tour.Description = value; 
                OnPropertyChanged(nameof(Description)); 
        }}
        public string FromAddress {
            get { return _tour.FromAddress; }
            set { 
                _tour.FromAddress = value;
                OnPropertyChanged(nameof(FromAddress));
        }}
        public string ToAddress {
            get { return _tour.ToAddress; } 
            set {  
                _tour.ToAddress = value;
                OnPropertyChanged(nameof(ToAddress));
        }}
        public string TransportType {  // TransportType { 
            get { return _tour.TransportType; } 
            set {
                _tour.TransportType = value;
                OnPropertyChanged(nameof(TransportType));
        }}
        public double Distance {
            get { return _tour.Distance; } 
            set {
                _tour.Distance = value;
                OnPropertyChanged(nameof(Distance));
        }}
        public int EstimatedTime { 
            get { return _tour.EstimatedTime; } 
            set {  
                _tour.EstimatedTime = value;
                OnPropertyChanged(nameof(EstimatedTime));
        }}
        public string Image {
            get { return _tour.Image; }
            set {
                _tour.Image = value;
                OnPropertyChanged(nameof(Image));
        }}
        public int Popularity { 
            get { return _tour.Popularity; } 
            set {  
                _tour.Popularity = value;
                OnPropertyChanged(nameof(Popularity));
        }}
        public int ChildFriendliness {
            get { return _tour.ChildFriendliness; } 
            set {  
                _tour.ChildFriendliness = value;
                OnPropertyChanged(nameof(ChildFriendliness));
        }}


        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
