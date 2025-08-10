using RequestManagement.Common.Models;

namespace OneCOverlayClient.Messages.Models
{
    public class SelectDriverTaskModel
    {
        public bool Result { get; set; }
        public Driver? Driver { get; set; }
        public Type? Caller { get; set; }
        public bool EditMode { get; set; }
    }
}
