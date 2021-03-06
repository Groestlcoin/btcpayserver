using System.Collections.Generic;
using BTCPayServer.Configuration;

namespace BTCPayServer.Models.ServerViewModels
{
    public class ServicesViewModel
    {

        public class OtherExternalService
        {
            public string Name { get; set; }
            public string Link { get; set; }
        }

        public List<ExternalService> ExternalServices { get; set; } = new List<ExternalService>();
        public List<OtherExternalService> OtherExternalServices { get; set; } = new List<OtherExternalService>();
        public List<OtherExternalService> TorHttpServices { get; set; } = new List<OtherExternalService>();
        public List<OtherExternalService> TorOtherServices { get; set; } = new List<OtherExternalService>();
        public List<OtherExternalService> ExternalStorageServices { get; set; } = new List<OtherExternalService>();
    }
}
