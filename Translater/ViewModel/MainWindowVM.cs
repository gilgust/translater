using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using Google.Cloud.Translation.V2;
using Translater.View;

namespace Translater.ViewModel
{
    class MainWindowVM : BaseNotifyPropertyChanged
    {
        private string _originFile;
        private string _originText;
        private string _translatedFile;
        private string _translatedText;

        private Language _sourceLanguage;
        private Language _targetLanguage;

        private TranslationClient _translater;

        //private Window _settingsWindow;

        public MainWindowVM()
        {
            var SingletonTranslater = GoogleTranslater.GetInstance();
            _translater = SingletonTranslater.Translater;

            SupportedLanguages = _translater.ListLanguages("ru");
            //_settingsWindow = new SettingsWindow();
        }


        #region property

        public string OriginFile {
            get => _originFile;
            set
            {
                if (_originFile == value) return;
                _originFile = value;
                OnPropertyChanged(nameof(OriginFile));
            }
        }
        public string TranslatedFile
        {
            get => _translatedFile;
            set
            {
                if (_translatedFile == value) return;
                _translatedFile = value;
                OnPropertyChanged(nameof(TranslatedFile));
            }
        }
        public string OriginText
        {
            get => _originText;
            set
            {
                if (_originText == value) return;
                _originText = value;
                OnPropertyChanged(nameof(OriginText));
            }
        }
        public string TranslatedText
        {
            get => _translatedText;
            set
            {
                if (_translatedText == value) return;
                _translatedText = value;
                OnPropertyChanged(nameof(TranslatedText));
            }
        }
        public Language TargetLanguage
        {
            get => _targetLanguage;
            set
            {
                if (_targetLanguage == value) return;
                _targetLanguage = value;
                OnPropertyChanged(nameof(TargetLanguage));
            }
        }
        public Language SourceLanguage
        {
            get => _sourceLanguage;
            set
            {
                if (_sourceLanguage == value) return;
                _sourceLanguage = value;
                OnPropertyChanged(nameof(SourceLanguage));
            }
        }
        public IList<Language> SupportedLanguages { get; }

        #endregion


        #region command

        private RelayCommand _chooseFile;
        public RelayCommand ChooseFile
        {
            get
            {
                return _chooseFile ??
                    (_chooseFile = new RelayCommand(obj =>
                    {
                        var ofd = new OpenFileDialog
                        {
                            InitialDirectory = @"C:\cSharp\test_folder",
                            Filter = "txt files (*.txt)|*.txt|All files(*.*)|*.*"
                        };
                        var result = ofd.ShowDialog();

                        if (result != true) return;

                        OriginFile = ofd.FileName;
                        var originFilesDir = Path.GetDirectoryName(_originFile);
                        var translatedFilesDir = Path.Combine(originFilesDir, "translated");
                        TranslatedFile = Path.Combine(translatedFilesDir, ofd.SafeFileName);

                        if (Directory.Exists(translatedFilesDir)) return;

                        //Directory.CreateDirectory(translatedFilesDir);
                    }));
            }
        }

        private RelayCommand _translateFile;
        public RelayCommand TranslateFile
        {
            get
            {
                return _translateFile ??
                       (_translateFile = new RelayCommand(obj =>
                       {
                           CheckDirectoryTarget(); 

                           OriginText = "";
                           TranslatedText = "";

                           var reader = new StreamReader(_originFile, Encoding.Default);
                           var writer = new StreamWriter(_translatedFile, false, Encoding.Default);

                           var content = reader.ReadToEnd();

                           if (content.Length > 2000)
                           {
                               var strings = Split(content, 2000);
                               if (strings.Count() > 1)
                               {
                                   foreach (var item in strings)
                                   {
                                       var response = _translater.TranslateText(item, _targetLanguage.Code, _sourceLanguage.Code);

                                       writer.WriteLine(response.TranslatedText);
                                       OriginText += item;
                                       TranslatedText += response.TranslatedText;
                                   }
                               }
                           }
                           else
                           {
                               var response = _translater.TranslateText(content, _targetLanguage.Code, _sourceLanguage.Code);
                               writer.WriteLine(response.TranslatedText);
                           }

                           reader.Close();
                           writer.Close();
                       }, obj => { return File.Exists(_originFile) && _targetLanguage != null && _sourceLanguage != null ; }
                       ));
            }
        }

        private RelayCommand _openSettings;
        public RelayCommand OpenSettings
        {
            get
            {
                return _openSettings ??
                    (_openSettings = new RelayCommand(obj =>
                    {
                        var sw = new SettingsWindow();
                        sw.Show();
                    }));
            }
        }

        #endregion

        private void CheckDirectoryTarget()
        {
            var originFilesDir = Path.GetDirectoryName(_originFile);
            var translatedFilesDir = Path.Combine(originFilesDir, "translated");
            if (!Directory.Exists(translatedFilesDir))
            {
                Directory.CreateDirectory(translatedFilesDir);
            }
        }

        IEnumerable<string> Split(string str, int chunkSize)
        {
            List<string> strings = new List<string>();
            var count = str.Length / chunkSize;

            if (str.Length % chunkSize > 0) count++;

            for(var i = 0; i < count; i++)
            {
                if(i * chunkSize + chunkSize > str.Length)
                    strings.Add(str.Substring(i * chunkSize));
                else
                    strings.Add(str.Substring(i * chunkSize, chunkSize));
                
            }
            return strings;
        }

    }
}