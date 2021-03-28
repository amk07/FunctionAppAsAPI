using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Host;
using System.Linq;

namespace FunctionAppAsAPI
{
    public static class ContactAPI
    {

        static List<Contact> contacts = new List<Contact>();

        [FunctionName("CreateContact")]
        public static async Task<IActionResult> CreateContact(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "contact")] HttpRequest req, ILogger log)
        {
            log.LogInformation("Creating a new contact");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<ContactModel>(requestBody);

            var contact = new Contact() { FirstName = input.FirstName, LastName = input.LastName, Phone = input.Phone };
            contacts.Add(contact);
            return new OkObjectResult(contact);
        }

        [FunctionName("GetContacts")]
        public static IActionResult GetContacts(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "contact")] HttpRequest req, ILogger log)
        {
            log.LogInformation("Getting contacts");
            return new OkObjectResult(contacts);
        }

        [FunctionName("GetContactById")]
        public static IActionResult GetContactById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "contact/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            var contact = contacts.FirstOrDefault(t => t.Id == id);
            if (contact == null)
            {
                log.LogWarning("contact not found");

                return new NotFoundResult();
            }

            log.LogInformation("Getting contact by Id");

            return new OkObjectResult(contact);
        }

        [FunctionName("UpdateContact")]
        public static async Task<IActionResult> UpdateContact(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "contact/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            var contact = contacts.FirstOrDefault(t => t.Id == id);
            if (contact == null)
            {
                log.LogWarning("contact not found");

                return new NotFoundResult();
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updatedContact = JsonConvert.DeserializeObject<ContactModel>(requestBody);

            if (!string.IsNullOrEmpty(updatedContact.FirstName))
            {
                contact.FirstName = updatedContact.FirstName;
            }
            if (!string.IsNullOrEmpty(updatedContact.LastName))
            {
                contact.LastName = updatedContact.LastName;
            }
            if (!string.IsNullOrEmpty(updatedContact.Phone))
            {
                contact.Phone = updatedContact.Phone;
            }

            return new OkObjectResult(contact);
        }

        [FunctionName("DeleteContact")]
        public static IActionResult DeleteContact(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "contact/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            var contact = contacts.FirstOrDefault(t => t.Id == id);
            if (contact == null)
            {
                return new NotFoundResult();
            }
            contacts.Remove(contact);
            return new OkResult();
        }

    }
}
