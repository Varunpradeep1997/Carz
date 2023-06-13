using AspNetCore.Identity.MongoDbCore.Models;
using System.Runtime.Serialization;
using System;
using System.Collections.ObjectModel;
using MongoDbGenericRepository.Attributes;

namespace Carz.Models
{
    [CollectionName("users")]
    public class ApplicationUser:MongoIdentityUser<Guid>
    {
        public string  FullName { get; set; }=string.Empty;

       


    }
}
