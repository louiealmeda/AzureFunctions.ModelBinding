using System;
using Microsoft.AspNetCore.Mvc;

namespace CourseAPI.AzureFunctions.ModelBinding
{
    public class ServiceResponse<T>
    {

        public ActionResult Response { get; set; }
        public T Data { get; set; }

        public ServiceResponse()
        {
            
        }

    }
}
