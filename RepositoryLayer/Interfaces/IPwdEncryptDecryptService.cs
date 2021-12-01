using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Interfaces
{
    public interface IPwdEncryptDecryptService
    {
        string EncryptPassword(string password);

        string DecryptPassword(string encryptpwd);
    }
}
