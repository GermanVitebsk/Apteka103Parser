using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Apteka103Parser.Annotations;
using Apteka103Parser.Helpers;

namespace Apteka103Parser.ViewModels
{
    public class MedecineViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly MedicineContext db;

        public MedecineViewModel()
        {
            db = new MedicineContext();
            DeleteCommand = new DelegateCommand(DeleteCommandFun);
            FindCommand = new DelegateCommand(FindCommandFun);

            IsNoBusy = true;

            ReloadData();
        }

        public ObservableCollection<Medicine> MedicineList { get; set; }
        public Medicine SelectedItem { get; set; }

        public ICommand DeleteCommand { get; set; }
        public ICommand FindCommand { get; set; }

        public bool IsNoBusy { get; set; }

        private string statusText;
        public string StatusText
        {
            get { return statusText; }
            set
            {
                if (statusText != value)
                {
                    statusText = value;
                    OnPropertyChanged(nameof(StatusText));
                }
            }
        }

        private int recordsCount;
        public int RecordsCount
        {
            get { return recordsCount; }
            set
            {
                if (recordsCount != value)
                {
                    recordsCount = value;
                    OnPropertyChanged(nameof(RecordsCount));
                }
            }
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void FindCommandFun(object obj)
        {
            IsNoBusy = false;
            StatusText = "Загрузка данных";
            OnPropertyChanged(nameof(IsNoBusy));

            MedicineParserResults result = new MedicineParserResults();
            Task.Factory.StartNew(() =>
            {
                result = MedicineParser.Parse();
                ReloadData();
            }).ContinueWith(task =>
            {
                IsNoBusy = true;
                StatusText = "Добавлено записей: " + result.NewRowsCount;
                OnPropertyChanged(nameof(IsNoBusy));
            });
        }

        private void DeleteCommandFun(object obj)
        {
            if (SelectedItem == null)
                return;

            if (MessageBox.Show(
                    "Вы точно хотите удалить запись?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                ) != MessageBoxResult.Yes)
                return;

            db.Medicines.Remove(SelectedItem);
            MedicineList.Remove(SelectedItem);
            db.SaveChanges();
        }

        private void ReloadData()
        {
            MedicineList = new ObservableCollection<Medicine>(db.Medicines);
            OnPropertyChanged(nameof(MedicineList));
            RecordsCount = MedicineList.Count;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}