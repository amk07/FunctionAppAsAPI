using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionAppAsAPI
{
    public class ContactModel
    {        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
    }

    public class Contact : ContactModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("n");
    }
}
