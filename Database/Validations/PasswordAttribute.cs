using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Database.Validations
{
    /// <summary>
    /// 3.	Password – Required field. Text with length between 6 and 50 symbols. Should contain at least:
    /// a.	1 lowercase letter
    /// b.	1 uppercase letter
    /// c.  1 digit
    /// d.  1 special symbol(!, @, #, $, %, ^, &, *, (, ), _, +, <, >, ?)
    /// </summary>
    public class PasswordAttribute : ValidationAttribute
    {
        private readonly char[] symbols = new[] { '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '_', '+', '<', '>', '?' };


        public PasswordAttribute()
        {
            this.ErrorMessage = "Password is not valid.";
        }
        public override bool IsValid(object value)
        {
            var password = value as string;
            if (password == null)
            {
                return true;
            }

            return password.All(s => char.IsLower(s)
                            || char.IsUpper(s)
                            || char.IsDigit(s)
                            || this.symbols.Contains(s));

        }

    }
}
