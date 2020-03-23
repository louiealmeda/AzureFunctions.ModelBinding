using System;
namespace AzureFunctions.ModelBinding
{
    public class ValidationSchemaNotRegisteredException<T> : Exception
    {
        public ValidationSchemaNotRegisteredException()
            :base("Schema for " + typeof(T).Name + " is not registered")
        {

        }   
    }
}
