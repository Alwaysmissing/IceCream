using IceCream.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace IceCream.ViewModel
{
    public class MainWindowViewModel : ModelBase
    {
        private string _Window1Process;
        public string Window1Process
        {
            get { return _Window1Process; }
            set
            {
                _Window1Process = value;
                OnPropertyChanged(nameof(Window1Process));
            }
        }

        private string _Window2Process;
        public string Window2Process
        {
            get { return _Window2Process; }
            set
            {
                _Window2Process = value;
                OnPropertyChanged(nameof(Window2Process));
            }
        }

        private bool _CanMakeIceCream;
        public bool CanMakeIceCream
        {
            get => _CanMakeIceCream;
            set
            {
                _CanMakeIceCream = value;
                OnPropertyChanged(nameof(CanMakeIceCream));
            }
        }

        private bool _CanReloadMaterials;
        public bool CanReloadMaterials
        {
            get => _CanReloadMaterials;
            set
            {
                _CanReloadMaterials = value;
                OnPropertyChanged(nameof(CanReloadMaterials));
            }
        }

        private ICommand _StartMakeCommand;
        public ICommand StartMakeCommand
        {
            get => _StartMakeCommand;
            set
            {
                _StartMakeCommand = value;
                OnPropertyChanged(nameof(StartMakeCommand));
            }
        }

        private ICommand _ReloadMaterialCommand;
        public ICommand ReloadMaterialCommand
        {
            get => _ReloadMaterialCommand;
            set
            {
                _ReloadMaterialCommand = value;
                OnPropertyChanged(nameof(ReloadMaterialCommand));
            }
        }

        private MachineState _machineState;
        public MachineState machineState
        {
            get
            {
                return _machineState;
            }
            set
            {
                _machineState = value;
                OnPropertyChanged(nameof(machineState));
            }
        }

        private bool IsShowed = false;

        public MainWindowViewModel()
        {
            machineState = new MachineState();
            machineState.PropertyChanged += machineState_PropertyChanged;
            StartMakeCommand = new RelayCommand(async _ => await StartMakeIceCreamAsync());
            ReloadMaterialCommand = new RelayCommand(_ => machineState.Reload());
            machineState.LoadState();
            if (machineState.IsMakingIceCream)
            {
                var result = MessageBox.Show("检测到上次未完成的制作过程，是否继续？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result == MessageBoxResult.No)
                {
                    machineState.IsMakingIceCream = false;
                    machineState.CurrentActivity = "Idle";
                }
                else
                {
                    Task.Run(()=> StartMakeIceCreamAsync());
                }
            }
            UpdateCanMakeIceCream();
            UpdateCanReloadMaterial();
        }

        private void machineState_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateCanMakeIceCream();
            UpdateCanReloadMaterial();
        }

        private void UpdateCanReloadMaterial()
        {
            if (machineState.Remain > 10)
            {
                CanReloadMaterials = !machineState.IsMakingIceCream;
            }
            else
            {
                if (machineState.Remain == 10 && !IsShowed)
                {
                    IsShowed = true;
                    MessageBox.Show("请及时更换材料。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                CanReloadMaterials = true;
            }
        }

        private void UpdateCanMakeIceCream()
        {
            CanMakeIceCream = !machineState.IsMakingIceCream && machineState.Remain > 0 && !machineState.IsReloading;
        }

        private async Task StartMakeIceCreamAsync()
        {
            machineState.InitializeCancellationTokenSource();
            machineState.IsMakingIceCream = true;
            var cancellationToken = machineState.GetCancellationToken();
            Task window1Task = machineState.MakeIceCreamAsync(step => Window1Process = step, 1, cancellationToken);
            Task window2Task = machineState.MakeIceCreamAsync(step => Window2Process = step, 2, cancellationToken);

            await Task.WhenAll(window1Task, window2Task);
            machineState.IsMakingIceCream = false;
        }
    }
}
