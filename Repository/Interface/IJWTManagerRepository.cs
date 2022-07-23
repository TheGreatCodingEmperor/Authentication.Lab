using  Authentication.Lab.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Lab.Repository.Interface
{
   public interface IJWTManagerRepository
    {
        Tokens Authenticate(Users users); 
    }
   
}