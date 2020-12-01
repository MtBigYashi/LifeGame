using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LifeGameViewModel.InteractionRequests;

namespace LifeGameViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private class Property<T>
        {
            private T _value = default;

            public T GetValue() => _value;
            public void SetValue(T value, Action<string> action, [CallerMemberName] string propertyName = "")
            {
                _value = value;
                action?.Invoke(propertyName);
            }
        }

        private Property<UpdateGenerationRequest> _updateGenerationRequest = new Property<UpdateGenerationRequest>();
        public UpdateGenerationRequest UpdateGenerationRequest
        {
            get => _updateGenerationRequest.GetValue();
            set => _updateGenerationRequest.SetValue(value, OnPropertyChanged);
        }

        private Property<ResizeCellRequest> _resizeCellRequest = new Property<ResizeCellRequest>();
        public ResizeCellRequest ResizeCellRequest
        {
            get => _resizeCellRequest.GetValue();
            set => _resizeCellRequest.SetValue(value, OnPropertyChanged);
        }

        private Property<ToggleCurrentCellAliveRequest> _toggleCurrentCellAliceRequest = new Property<ToggleCurrentCellAliveRequest>();
        public ToggleCurrentCellAliveRequest ToggleCurrentCellAliveRequest
        {
            get => _toggleCurrentCellAliceRequest.GetValue();
            set => _toggleCurrentCellAliceRequest.SetValue(value, OnPropertyChanged);
        }
    }
}
