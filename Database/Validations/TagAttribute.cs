using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Database.Validations
{
    /// <summary>
    /// Make a [Tag] attribute that would validate if the given string is valid tag. A valid tag is a string starting with pound sign (#), do not contain any spaces in it and is no more than 20 symbols long. 
    /// </summary>
    public class TagAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var tag = value as string;

            if (tag == null)
            {
                return true;
            }

            return tag.StartsWith('#') 
                && tag.All(s => char.IsWhiteSpace(s)) 
                && tag.Length <= 20;
        }
    }
}
