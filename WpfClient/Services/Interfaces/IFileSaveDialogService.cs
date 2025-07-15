namespace RequestManagement.WpfClient.Services.Interfaces;

public interface IFileSaveDialogService
{
    string? ShowSaveFileDialog(string defaultFileName, string filter);
}