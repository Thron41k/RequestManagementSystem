using IronOcr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestManagement.WarehouseScan.Services;

public static class TextRecognitionService
{
    public static async Task<string> RecognizeAsync(ImageSource imageSource)
    {
        try
        {
            var handler = imageSource as FileImageSource;
            if (handler == null)
                return "Не удалось распознать источник";

            var path = handler.File;
            var ocr = new IronTesseract();
            using var input = new OcrInput(path);
            var result = await Task.Run(() => ocr.Read(input));
            return result.Text;
        }
        catch (Exception ex)
        {
            return $"Ошибка: {ex.Message}";
        }
    }
}