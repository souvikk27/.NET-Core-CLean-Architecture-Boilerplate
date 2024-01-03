using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Presentation.Infrastructure.Services.Abstraction
{
    public interface IClientCredentialService
    {
         Task<OAuthClient> InvokeCredentialsAsync();
    }
}
