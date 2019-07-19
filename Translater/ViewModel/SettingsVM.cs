using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Translater.Model;

namespace Translater.ViewModel
{
    public class SettingsVM : BaseNotifyPropertyChanged
    {
        private string _keyPath; 

        public SettingsVM()
        {
            SettingsModel.GetInstance();
            this._keyPath = SettingsModel.KeyPath;

        }

        public string KeyPath {
            get => _keyPath;
            set
            {
                if (_keyPath == value) return;
                _keyPath = value;
                OnPropertyChanged(nameof(KeyPath));
            }
        }
        #region commands
        private RelayCommand _saveSettings;
        public RelayCommand SaveSettings {
            get
            {
                return _saveSettings ??
                    (_saveSettings = new RelayCommand(obj =>
                    {
                        SettingsModel.SaveKey(KeyPath);
                    }));
            }
        }
        #endregion
    }
}
