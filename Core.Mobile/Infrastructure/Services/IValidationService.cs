using System.Text.RegularExpressions;

namespace Core
{
    public interface IValidationService
    {
        bool EmailValid(string email);
    }

    public class ValidationService : IValidationService
    {             
        public bool EmailValid(string email)
            => Regex.IsMatch(email,
                @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
               @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$");        
    }
}
