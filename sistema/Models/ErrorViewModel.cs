using System;

namespace farmamest.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public string Message { get; set; }

        public bool IsDatabaseError { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public bool ShowMessage => !string.IsNullOrEmpty(Message);
    }
}
