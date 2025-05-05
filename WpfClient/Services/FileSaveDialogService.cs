using System.IO;
using Microsoft.Win32;
using WpfClient.Services.Interfaces;

namespace WpfClient.Services
{
    public class FileSaveDialogService : IFileSaveDialogService
    {
        public string? ShowSaveFileDialog(string defaultFileName, string filter)
        {
            var dialog = new SaveFileDialog
            {
                FileName = defaultFileName,
                Filter = filter,
                DefaultExt = Path.GetExtension(defaultFileName),
                AddExtension = true,
                OverwritePrompt = true
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }
    }
}
