using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using ContosoAPI.Models;
using System.Threading.Tasks;

namespace ContosoAPI.Controllers
{
    public class ValuesController : ApiController
    {

        // GET api/values
        [SwaggerOperation("GetAll")]
        public IEnumerable<string> Get()
        {
            List<string> returnList = new List<string>();

            using (var db = new ContosoDBEntities()) { 
                var keyValuePairs = db.DatabaseKeyValues;

                foreach (var keyValue in keyValuePairs)
                {
                    returnList.Add(keyValue.Key + " || " + keyValue.Value);
                }
            }

            return returnList;
        }

        // GET api/values/5
        [SwaggerOperation("GetById")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public string GetItem(string key)
        {
            string returnValue = null;

            using (var db = new ContosoDBEntities())
            {
                var keyValuePairs = db.DatabaseKeyValues;

                foreach (var keyValue in keyValuePairs)
                {
                    if (keyValue.Key.Equals(key))
                    {
                        returnValue = keyValue.Value;
                    }
                }

                if (returnValue == null)
                {
                    return null;
                }
            }

            return returnValue;
        }

        // POST api/values
        [SwaggerOperation("Create")]
        [SwaggerResponse(HttpStatusCode.Created)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public HttpResponseMessage Post([FromBody]string value)
        {
            if(value == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The key value pair is malformed. (no key and/or value supplied)");
            }

            if(!value.Contains(@"||"))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The key value pair is not in the expected format; 'key || value'");
            }

            var keyValuePair = value.Split(new string[]{ @"||" }, StringSplitOptions.RemoveEmptyEntries);

            if(keyValuePair.Length <= 1)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The key value pair is malformed. (no key and/or value supplied)");
            }

            using (var db = new ContosoDBEntities())
            {
                db.DatabaseKeyValues.Add(new DatabaseKeyValue { Key = keyValuePair[0].Trim(), Value = keyValuePair[1].Trim() });
                db.SaveChanges();
            }

            return Request.CreateResponse(HttpStatusCode.Created);
        }

        // PUT api/values/5
        [SwaggerOperation("Update")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public HttpResponseMessage Put(string key, [FromBody]string newValue)
        {
            bool found = false;

            using (var db = new ContosoDBEntities())
            {
                var keyValuePairs = db.DatabaseKeyValues;

                foreach (var keyValue in keyValuePairs)
                {
                    if (keyValue.Key.Equals(key))
                    {
                        keyValue.Value = newValue;
                        found = true;
                        db.SaveChanges();
                        break;
                    }
                }
            }

            if (!found)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "The requested key does not exist.");
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // DELETE api/values/5
        [SwaggerOperation("Delete")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public HttpResponseMessage Delete(string key)
        {
            bool found = false;

            using (var db = new ContosoDBEntities())
            {
                var keyValuePairs = db.DatabaseKeyValues;

                foreach (var keyValue in keyValuePairs)
                {
                    if (keyValue.Key.Equals(key))
                    {
                        db.DatabaseKeyValues.Remove(keyValue);
                        found = true;
                        break;
                    }
                }

                if(found)
                {
                    db.SaveChanges();
                }
            }

            if (!found)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "The key you are trying to delete does not exist.");
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
