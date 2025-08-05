using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace RequestManagement.WpfClient.ViewModels
{
    public partial class AddMaterialsInUseToOffViewModel : ObservableObject
    {
        [ObservableProperty] private string _documentNumber = string.Empty;
        [ObservableProperty] private string _reason = string.Empty;
        [ObservableProperty] private DateTime _documentDate = DateTime.Now;
        public event EventHandler CloseWindowRequested;

        public bool DialogResult { get; set; }

        public void Init(string documentNumber = "", string reason = "", DateTime documentDate = default)
        {
            DocumentNumber = documentNumber;
            Reason = reason;
            DocumentDate = documentDate;
        }

        [RelayCommand]
        private void SaveResult()
        {
            DialogResult = true;
            CloseWindowRequested.Invoke(this, EventArgs.Empty);
        }
    }
}
