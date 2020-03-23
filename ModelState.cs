using System;
using Microsoft.AspNetCore.Mvc;

namespace AzureFunctions.ModelBinding
{
    public class ModelState<T>
    {
        public bool IsValid { get; internal set; }
        public T Model { get; internal set; }
        public IActionResult BadRequest { get; internal set; }
    }
}
