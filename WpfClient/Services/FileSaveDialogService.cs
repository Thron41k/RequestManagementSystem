using System.IO;
using Microsoft.Win32;
using RequestManagement.WpfClient.Services.Interfaces;

namespace RequestManagement.WpfClient.Services;

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