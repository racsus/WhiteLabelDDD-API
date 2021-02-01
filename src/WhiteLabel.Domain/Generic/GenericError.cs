using System;
using System.ComponentModel.DataAnnotations;
using WhiteLabel.Domain.Extensions;

namespace WhiteLabel.Domain.Generic
{
    public enum ApplicationErrorEnum : uint
    {
        [Display(Name = "Generic Error")]
        GENERIC = 100000,
        [Display(Name = "Data Validation")]
        DATA_VALIDATION = 100001
    }

    public class GenericError
    {
        public string Code { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        public Exception Exception { get; set; }

        public GenericError(ApplicationErrorEnum singleError)
        {
            this.Code = singleError.GetHashCode().ToString();
            this.Description = singleError.GetDisplayName();
            this.Name = EnumExtensions.GetDescription(singleError);
            this.Exception = null;
        }

        public GenericError(ApplicationErrorEnum singleError, Exception exception)
        {
            this.Code = singleError.GetHashCode().ToString();
            this.Description = singleError.GetDisplayName();
            this.Name = singleError.GetType().ToString();
            this.Exception = exception;
        }

        public GenericError(string errorMessage)
        {
            this.Code = ApplicationErrorEnum.GENERIC.GetHashCode().ToString();
            this.Description = errorMessage;
            this.Name = EnumExtensions.GetDisplayName(ApplicationErrorEnum.GENERIC);
            this.Exception = null;
        }

        public GenericError(ApplicationErrorEnum singleError, Exception exception, string errorMessage)
        {
            this.Code = singleError.GetHashCode().ToString();
            this.Description = errorMessage;
            this.Name = EnumExtensions.GetDisplayName(singleError);
            this.Exception = exception;
        }

        public GenericError()
        {
            this.Code = ApplicationErrorEnum.GENERIC.GetHashCode().ToString();
            this.Description = "";
            this.Name = EnumExtensions.GetDisplayName(ApplicationErrorEnum.GENERIC);
            this.Exception = null;
        }
    }
}
