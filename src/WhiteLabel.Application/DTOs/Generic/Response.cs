using System;
using System.Collections.Generic;
using System.Linq;
using WhiteLabel.Domain.Generic;

namespace WhiteLabel.Application.DTOs.Generic
{
    public sealed class Response<TReturn>
    {
        public TReturn Object { get; set; }
        public IEnumerable<GenericError> Errors { get; } = Enumerable.Empty<GenericError>();
        public bool Succeeded => !Errors.Any();

        public Response()
        {
            Errors = Array.Empty<GenericError>();
        }

        public Response(TReturn obj)
        {
            Object = obj;
        }

        public Response(string error)
        {
            Errors = new GenericError[] { new(error) };
        }

        public Response(ApplicationErrorEnum singleError)
        {
            Errors = new GenericError[] { new(singleError) };
        }

        public Response(ApplicationErrorEnum singleError, Exception exception)
        {
            Errors = new GenericError[] { new(singleError, exception) };
        }

        public Response(IEnumerable<GenericError> errors)
        {
            Errors = errors;
        }

        public Response(IEnumerable<string> errors)
        {
            var res = errors.Select(error => new GenericError(error)).ToList();
            Errors = res;
        }
    }
}
