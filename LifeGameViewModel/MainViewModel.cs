﻿
namespace LifeGameViewModel
{
    using System;
    using System.Data;
    using System.Diagnostics;
    using System.Windows.Input;
    using LifeGameModel;
    using LifeGameViewModel.InteractionRequests;

    public class MainViewModel : ViewModelBase
    {
        private DataTable _dataTable = new DataTable();
        public MainViewModel()
            => LifeGameManager.Instance.LayoutUpdated += LayoutUpdate;

        private void InitializeLifeGameTable()
        {
            _dataTable.Columns.Clear();
            _dataTable.Rows.Clear();
            if (_lifeGameView != null)
            {
                _lifeGameView.Dispose();
                _lifeGameView = null;
            }

            LifeGameManager.Instance.InitializeGeneration();
            for (int i = 0; i < LifeGameManager.Instance.Rank; i++)
            {
                _dataTable.Columns.Add();
                _dataTable.Rows.Add();
            }
            LifeGameView = new DataView(_dataTable);
        }

        private void LayoutUpdate(object sender, EventArgs e)
        {
            UpdateGenerationRequest = new UpdateGenerationRequest()
            {
                Rank = LifeGameManager.Instance.Rank,
                GenerationInformation = LifeGameManager.Instance.GetCurrentGeneration()
            };
        }

        private DataView _lifeGameView = null;
        public DataView LifeGameView
        {
            get => _lifeGameView;
            set
            {
                _lifeGameView = value;
                OnPropertyChanged();
            }
        }

        private ICommand _mouseUp = null;
        public ICommand MouseUp => _mouseUp ?? (_mouseUp = new RelayCommand(o =>
        {
            ToggleCurrentCellAliveRequest = new ToggleCurrentCellAliveRequest();
            int row = ToggleCurrentCellAliveRequest.Row;
            int col = ToggleCurrentCellAliveRequest.Column;
            int rank = LifeGameManager.Instance.Rank;
            if (row < 0 || rank <= row || col < 0 || rank <= col)
            {
                Trace.WriteLine($"[ERR] Invalid index is specified [Row{row},Column{col}");
                return;
            }

            LifeGameManager.Instance.ToggleAlive(row, col);
        }));

        private ICommand _refresh = null;
        public ICommand Refresh => _refresh ?? (_refresh = new RelayCommand(o =>
        {
            InitializeLifeGameTable();
            ResizeCellRequest = new ResizeCellRequest()
            {
                Rank = LifeGameManager.Instance.Rank
            };
        }));

        private ICommand _random = null;
        public ICommand Random => _random ?? (_random = new RelayCommand(o =>
        {
            LifeGameManager.Instance.RandomInitializeGeneration();
            LayoutUpdate(null, null);
        }));

        private ICommand _start = null;
        public ICommand Start => _start ?? (_start = new RelayCommand(o => LifeGameManager.Instance.StartGame()));

        private ICommand _stop = null;
        public ICommand Stop => _stop ?? (_stop = new RelayCommand(o => LifeGameManager.Instance.StopGame()));
    }
}
