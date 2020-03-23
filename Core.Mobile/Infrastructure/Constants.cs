using System;
using System.IO;
using System.Security;

namespace Core
{
    public static class Constants
    {
        public static string Database => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "database.db");


        public const string FingerprintKey = "fpk";
        public const string DateTokenKey = "dtk";
        public const string AuthSessionKey = "ask";
        public const string LoggedUserKey = "luk";
        public const string UserModeKey = "umk";
        public const string LoginCompleteKey = "lck";

        public const string PinKey = "pk";
    }
}
