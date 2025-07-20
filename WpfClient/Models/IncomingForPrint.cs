using System;
using System.Windows.Media;
using RequestManagement.Common.Models;

namespace RequestManagement.WpfClient.Models;

public class IncomingForPrint
{
    public Incoming Incoming { get; set; }

    public ImageSource? QrCodeImage { get; set; }

    public static IncomingForPrint FromIncoming(Incoming incoming, Func<string, ImageSource> qrGenerator)
    {
        return new IncomingForPrint
        {
            Incoming = incoming,
            QrCodeImage = qrGenerator(incoming.Stock.Nomenclature.Id.ToString())
        };
    }
}