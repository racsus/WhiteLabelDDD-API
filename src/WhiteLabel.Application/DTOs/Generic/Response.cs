using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WhiteLabel.Domain.Generic;

namespace WhiteLabel.Application.DTOs.Generic
{
    public sealed class Response<TReturn> 
    {
        public TReturn Object { get; set; }
        public IEnumerable<GenericError> Errors { get; } = Enumerable.Empty<GenericError>();
        public bool Succeeded => !this.Errors.Any();


        public Response() => this.Errors = new GenericError[0] { };
        public Response(TReturn obj) => this.Object = obj;
        public Response(string error) => this.Errors = new GenericError[1] { new GenericError(error) };
        public Response(ApplicationErrorEnum singleError) => this.Errors = new GenericError[1] { new GenericError(singleError) };
        public Response(ApplicationErrorEnum singleError, Exception exception) => this.Errors = new GenericError[1] { new GenericError(singleError, exception) };
        public Response(IEnumerable<GenericError> errors) => this.Errors = errors;
        public Response(IEnumerable<string> errors)
        {
            var res = new List<GenericError>();
            foreach (var error in errors)
            {
                res.Add(new GenericError(error));
            };
            this.Errors = res;
        }
    }
}