using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
