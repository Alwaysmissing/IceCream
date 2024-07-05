using IceCream.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace IceCream.Model
{
    public class MachineState : ModelBase
    {
        private int _ChocolateCount;
        public int ChocolateCount
        {
            get { return _ChocolateCount; }
            set
            {
                _ChocolateCount = value;
                UpdateRemain();
                OnPropertyChanged(nameof(ChocolateCount));
                OnPropertyChanged(nameof(CanMakeIceCream));
            }
        }

        private int _StrawberryCount;
        public int StrawberryCount
        {
            get { return _StrawberryCount; }
            set
            {
                _StrawberryCount = value;
                UpdateRemain();
                OnPropertyChanged(nameof(StrawberryCount));
                OnPropertyChanged(nameof(CanMakeIceCream));
            }
        }

        private int _IceCreamCount;
        public int IceCreamCount
        {
            get { return _IceCreamCount; }
            set
            {
                _IceCreamCount = value;
                UpdateRemain();
                OnPropertyChanged(nameof(IceCreamCount));
                OnPropertyChanged(nameof(CanMakeIceCream));
            }
        }

        private int _Remain;
        public int Remain
        {
            get { return _Remain; }
            set
            {
                _Remain = value;
                OnPropertyChanged(nameof(Remain));
            }
        }

        private bool _IsMakingIceCream;
        public bool IsMakingIceCream
        {
            get { return _IsMakingIceCream; }
            set
            {
                _IsMakingIceCream = value;
                OnPropertyChanged(nameof(IsMakingIceCream));
            }
        }

        private bool _IsReloading;
        public bool IsReloading
        {
            get { return _IsReloading; }
            set
            {
                _IsReloading = value;
                OnPropertyChanged(nameof(IsReloading));
            }
        }

        private string _CurrentActivity;
        public string CurrentActivity
        {
            get { return _CurrentActivity; }
            set
            {
                _CurrentActivity = value;
                OnPropertyChanged(nameof(CurrentActivity));
            }
        }
        public bool CanMakeIceCream => Remain > 0;
        private CancellationTokenSource cancellationTokenSource;

        public MachineState()
        {
            ChocolateCount = 100;
            StrawberryCount = 100;
            IceCreamCount = 100;
        }

        public async Task MakeIceCreamAsync(Action<string> updateState, int window, CancellationToken cancellationToken)
        {
            if (!CanMakeIceCream) return;
            CurrentActivity = "Ready";
            updateState(CurrentActivity);
            await Task.Delay(100);
            
            CurrentActivity = "Start Making";
            updateState(CurrentActivity);
            await Task.Delay(100);

            IsMakingIceCream = true;

            while (!cancellationToken.IsCancellationRequested && CanMakeIceCream)
            {
                if (window == 1)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    CurrentActivity = "Add Strawberry";
                    updateState(CurrentActivity);
                    await Task.Delay(1000);
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    if (StrawberryCount > 0)
                        StrawberryCount--;
                    else
                        break;
                    CurrentActivity = "Add Chocolate";
                    updateState(CurrentActivity);
                    await Task.Delay(1000);
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    if (ChocolateCount > 0)
                        ChocolateCount--;
                    else
                        break;
                    CurrentActivity = "Add IceCream";
                    updateState(CurrentActivity);
                    await Task.Delay(1000);
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    if (IceCreamCount > 0)
                        IceCreamCount--;
                    else
                        break;
                }
                else if (window == 2)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    CurrentActivity = "Add IceCream";
                    updateState(CurrentActivity);
                    await Task.Delay(1200);
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    if (IceCreamCount > 0)
                        IceCreamCount--;
                    else
                        break;
                    CurrentActivity = "Add Strawberry";
                    updateState(CurrentActivity);
                    await Task.Delay(1200);
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    if (StrawberryCount > 0)
                        StrawberryCount--;
                    else
                        break;
                    CurrentActivity = "Add Chocolate";
                    updateState(CurrentActivity);
                    await Task.Delay(1200);
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    if (ChocolateCount > 0)
                        ChocolateCount--;
                    else
                        break;
                }

            }
            CurrentActivity = "Stop";
            updateState(CurrentActivity);
            IsMakingIceCream = false;
        }

        public void Reload()
        {
            if (IsMakingIceCream && Remain <= 10)
            {
                IsMakingIceCream = false;
                cancellationTokenSource?.Cancel();
            }

            IsReloading = true;
            Thread.Sleep(3000);
            StrawberryCount = ChocolateCount = IceCreamCount = 100;
            IsReloading = false;
        }

        public void UpdateRemain()
        {
            Remain = Math.Min(StrawberryCount, Math.Min(ChocolateCount, IceCreamCount));
        }

        public void InitializeCancellationTokenSource()
        {
            cancellationTokenSource = new CancellationTokenSource();
        }

        public CancellationToken GetCancellationToken()
        {
            return cancellationTokenSource.Token;
        }

        public void SaveState()
        {
            var dbHelper = new DataBaseHelper();
            dbHelper.SaveState(this);
        }

        public void LoadState()
        {
            var dbHelper = new DataBaseHelper();
            dbHelper.LoadState(this);
        }
    }
}
