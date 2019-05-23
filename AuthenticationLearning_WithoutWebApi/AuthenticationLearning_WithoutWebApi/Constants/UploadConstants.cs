using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuthenticationLearning_WithoutWebApi.Constants
{
    public static class UploadConstants
    {
        public const string UploadError = "ErrorMessage";
        public const string UploadSuccess = "SuccessMessage";
        public const string FileNotFound = "Please select the Pricing Excel to upload!!!";
        public const string InvalidFileFormat = "\n. Please select Pricing Excel file of format .xls and .xlsx!!!";
        public const string UploadSuccessMessage = "The Pricing Data is Successfully Updated!!!";
    }
}