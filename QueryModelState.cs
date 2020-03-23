using System;
namespace AzureFunctions.ModelBinding
{
    public class QueryModelState<T> : ModelState<T>
    {
        public int Offset { get; set; }
        public int Limit { get; set; }
    }
}
