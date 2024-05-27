using System;
using System.ComponentModel.DataAnnotations;
using WhiteLabel.Domain.Extensions;

namespace WhiteLabel.Domain.Generic
{
    public enum ApplicationErrorEnum : uint
    {
        [Display(Name = "Generic Error")]
        Generic = 100000,

        [Display(Name = "Data Validation")]
        DataValidation = 100001
    }

    public class GenericError
    {
        public string Code { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        public Exception Exception { get; set; }

        public GenericError(ApplicationErrorEnum singleError)
        {
            Code = singleError.GetHashCode().ToString();
            Description = singleError.GetDisplayName();
            Name = EnumExtensions.GetDescription(singleError);
            Exception = null;
        }

        public GenericError(ApplicationErrorEnum singleError, Exception exception)
        {
            Code = singleError.GetHashCode().ToString();
            Description = singleError.GetDisplayName();
            Name = singleError.GetType().ToString();
            Exception = exception;
        }

        public GenericError(string errorMessage)
        {
            Code = ApplicationErrorEnum.Generic.GetHashCode().ToString();
            Description = errorMessage;
            Name = EnumExtensions.GetDisplayName(ApplicationErrorEnum.Generic);
            Exception = null;
        }

        public GenericError(
            ApplicationErrorEnum singleError,
            Exception exception,
            string errorMessage
        )
        {
            Code = singleError.GetHashCode().ToString();
            Description = errorMessage;
            Name = EnumExtensions.GetDisplayName(singleError);
            Exception = exception;
        }

        public GenericError()
        {
            Code = ApplicationErrorEnum.Generic.GetHashCode().ToString();
            Description = "";
            Name = EnumExtensions.GetDisplayName(ApplicationErrorEnum.Generic);
            Exception = null;
        }
    }
}
