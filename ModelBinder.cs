using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace AzureFunctions.ModelBinding
{
    public static class ModelBinder
    {
        static Regex intRegex = new Regex("^[09]+$") ;
        static Dictionary<Type, JSchema> schemas = new Dictionary<Type, JSchema>();
        static BadRequestObjectResult defaultBadRequest = new BadRequestObjectResult(new string[] { "Invalid payload. Body should be a valid JSON Object" });

        public static JsonSerializerSettings SerializationSettings = new JsonSerializerSettings();
        

        public static void Register<T>(string name = null)
        {
            name = name ?? typeof(T).Name;

            if (schemas.ContainsKey(typeof(T)))
                return;

            var binDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var rootDirectory = Path.GetFullPath(Path.Combine(binDirectory, ".."));

            string json = File.ReadAllText(rootDirectory + "/Schemas/" + name + ".json");
            
            schemas.Add(typeof(T), JSchema.Parse(json));
        }

        public static ModelState<T> Bind<T>(HttpRequest request)
        {

            if (request.Body == null)
            {
                return new ModelState<T>() {
                    IsValid = false,
                    BadRequest = defaultBadRequest
                };
            }

            string requestBody = new StreamReader(request.Body).ReadToEnd();

            return Bind<T>(requestBody);
            
        }

        public static ModelState<T> Bind<T>(string json)
        {
            IList<string> errorMessages = new List<string>();

            ModelState<T> binding = new ModelState<T>()
            {
                IsValid = false
            };

            if (!schemas.ContainsKey(typeof(T)))
            {
                throw new ValidationSchemaNotRegisteredException<T>();
            }

            try
            {
                bool validRequest = JObject.Parse(json).IsValid(schemas[typeof(T)], out errorMessages);

                if (!validRequest)
                {
                    binding.BadRequest = new BadRequestObjectResult(errorMessages);
                    return binding;
                }


                binding.Model = BsonSerializer.Deserialize<T>(json);
                binding.IsValid = true;
            }
            catch
            {
                binding.BadRequest = new BadRequestObjectResult(errorMessages);
            }

            return binding;

        }


        public static QueryModelState<T> BindQuery<T>(HttpRequest req)
        {
            QueryModelState<T> ret = new QueryModelState<T>();

            if (req.Query.ContainsKey("limit") && intRegex.IsMatch(req.Query["limit"]))
                ret.Limit = int.Parse(req.Query["limit"]);

            if (req.Query.ContainsKey("offset") && intRegex.IsMatch(req.Query["offset"]))
                ret.Offset = int.Parse(req.Query["offset"]);

            if (ret.Limit > 100)
                ret.Limit = 100;


            
            ModelState<T> state = Bind<T>(req.QueryString.Value);
            ret.IsValid = state.IsValid;
            ret.BadRequest = state.BadRequest;
            ret.Model = state.Model;

            return ret;
        }
    }
}
