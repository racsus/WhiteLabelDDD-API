﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhiteLabel.Application.DTOs.Generic;
using WhiteLabel.Domain.Generic;

namespace WhiteLabel.WebAPI.Filters
{
    public class ValidationFilter: IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var listOfErrors = new List<GenericError>();
                var modelErrors = context.ModelState.Values;
                foreach (var modelError in modelErrors)
                {
                    foreach (var error in modelError.Errors)
                    {
                        listOfErrors.Add(new GenericError(ApplicationErrorEnum.DATA_VALIDATION, error.Exception, error.ErrorMessage));
                    }
                }
                var response = new Response<string>(listOfErrors);
                context.Result = new BadRequestObjectResult(response);              
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }
}
