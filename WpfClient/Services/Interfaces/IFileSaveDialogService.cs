using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfClient.Services.Interfaces
{
    public interface IFileSaveDialogService
    {
        string? ShowSaveFileDialog(string defaultFileName, string filter);
    }
}
