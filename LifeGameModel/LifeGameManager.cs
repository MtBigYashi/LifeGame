
namespace LifeGameModel
{
    using System;
    using System.Linq;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public class LifeGameManager
    {
        private Dictionary<int, List<bool>> _preGeneration = new Dictionary<int, List<bool>>();

        private Dictionary<int, List<bool>> _curGeneration = new Dictionary<int, List<bool>>();

        private Task _task = null;

        private CancellationTokenSource _tokenSource = null;

        public event EventHandler LayoutUpdated = null;

        private LifeGameManager() { }

        public static LifeGameManager Instance { get; } = new LifeGameManager();

        public int Rank { get; set; } = 30;

        private double _density = 0.25;
        public double Density
        {
            get => _density;
            set
            {
                if (value <= 0 || 1.0 <= value) return;
                _density = value;
            }
        }

        public void InitializeGeneration()
        {
            _preGeneration.Clear();
            _curGeneration.Clear();
            _preGeneration = Enumerable.Range(0, Rank).ToDictionary(_ => _, _ => Enumerable.Repeat(false, Rank).ToList());
            _curGeneration = Enumerable.Range(0, Rank).ToDictionary(_ => _, _ => Enumerable.Repeat(false, Rank).ToList());
        }

        public Dictionary<int, List<bool>> GetCurrentGeneration()
            // コピーを渡す
            => new Dictionary<int, List<bool>>(_curGeneration);

        public void ToggleAlive(int i, int j)
            // 指定された部分を真偽反転させる
            => _curGeneration[i][j] = !_curGeneration[i][j];

        public void StartGame()
        {
            if (_task?.Status == TaskStatus.Running)
            {
                Trace.WriteLine("[INF] LifeGame is already running");
                return;
            }

            _tokenSource = new CancellationTokenSource();
            _task = Task.Factory.StartNew(() =>
            {
                while (!_tokenSource.IsCancellationRequested)
                {
                    UpdateGeneration();
                    LayoutUpdated?.Invoke(null, null);

                    Thread.Sleep(500);
                }
            }, _tokenSource.Token);

            Trace.WriteLine("[INF] LifeGame is started");
        }

        public void StopGame()
        {
            if (_task?.Status != TaskStatus.Running || _tokenSource == null)
            {
                Trace.WriteLine("[INF] LifeGame is NOT running");
                return;
            }

            _tokenSource.Cancel();
            _task.Wait();

            _tokenSource.Dispose();
            _task.Dispose();

            _tokenSource = null;
            _task = null;

            Trace.WriteLine("[INF] LifeGame is stopped");
        }

        private void UpdateGeneration()
        {
            // 世代交代
            _preGeneration = (from pair in _curGeneration select pair)
                .ToDictionary(_p => _p.Key, _p => new List<bool>(_p.Value));
            // 現世代の算出
            for (int i = 0; i < Rank; i++)
            {
                for (int j = 0; j < Rank; j++)
                {
                    int aliveCell = GetAliveSurroundCellCount(i, j);

                    if (Birth(i, j, aliveCell))
                    {
                        // 誕生
                        _curGeneration[i][j] = true;
                    }
                    else if (Depopulation(i, j, aliveCell))
                    {
                        // 過疎
                        _curGeneration[i][j] = false;
                    }
                    else if (Survive(i, j, aliveCell))
                    {
                        // 生存
                        _curGeneration[i][j] = true;
                    }
                    else if (Overcrowded(i, j, aliveCell))
                    {
                        // 過密
                        _curGeneration[i][j] = false;
                    }
                }
            }
        }

        private int GetAliveSurroundCellCount(int i, int j)
        {
            int above = i == 0 ? Rank - 1 : i - 1;
            int below = i == Rank - 1 ? 0 : i + 1;
            int left = j == 0 ? Rank - 1 : j - 1;
            int right = j == Rank - 1 ? 0 : j + 1;

            int aliveCell = 0;
            if (_preGeneration[above][left]) aliveCell++;
            if (_preGeneration[above][j]) aliveCell++;
            if (_preGeneration[above][right]) aliveCell++;
            if (_preGeneration[i][left]) aliveCell++;

            if (_preGeneration[i][right]) aliveCell++;
            if (_preGeneration[below][left]) aliveCell++;
            if (_preGeneration[below][j]) aliveCell++;
            if (_preGeneration[below][right]) aliveCell++;

            return aliveCell;
        }

        private bool Birth(int i, int j, int aliveSurroundCells)
            => !_preGeneration[i][j] && aliveSurroundCells == 3;

        private bool Depopulation(int i, int j, int aliveSurroundCells)
            => _preGeneration[i][j] && aliveSurroundCells <= 1;

        private bool Survive(int i, int j, int aliveSurroundCells)
            => _preGeneration[i][j] && (aliveSurroundCells == 2 || aliveSurroundCells == 3);

        private bool Overcrowded(int i, int j, int aliveSurroundCells)
            => _preGeneration[i][j] && aliveSurroundCells >= 4;

        public void RandomInitializeGeneration()
        {
            _preGeneration.Clear();
            _preGeneration = Enumerable.Range(0, Rank).ToDictionary(_ => _, _ => Enumerable.Repeat(false, Rank).ToList());

            int totalCellCount = Rank * Rank;
            var randomList = GetRandomRange(0, totalCellCount, (int)(totalCellCount * Density));
            for (int i = 0; i < Rank; i++)
            {
                for (int j = 0; j < Rank; j++)
                {
                    if (randomList.Any(_r => _r / Rank == i && _r % Rank == j))
                        _curGeneration[i][j] = true;
                    else
                        _curGeneration[i][j] = false;
                }
            }
        }

        private List<int> GetRandomRange(int min, int max, int count)
        {
            if (min > max || count <= 0) return null;

            Random random = new Random(DateTime.Now.Millisecond);
            List<int> list = new List<int>();

            while (list.Count < count)
            {
                int r = random.Next(min, max);
                if (list.Contains(r)) continue;

                list.Add(r);
            }
            return list;
        }
    }
}
