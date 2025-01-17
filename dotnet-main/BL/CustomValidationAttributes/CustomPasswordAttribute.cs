using System.ComponentModel.DataAnnotations;

namespace PIP.BL.CustomValidationAttributes;

public class CustomPasswordAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        var password = value as string;

        if (string.IsNullOrEmpty(password))
            return false;


        if (password.Length < 8)
            return false;


        if (!password.Any(char.IsUpper) || 
            !password.Any(char.IsLower) || 
            !password.Any(char.IsDigit) || 
            !password.Any(IsSpecialCharacter)) 
        {
            return false;
        }

        return true;
    }

    private bool IsSpecialCharacter(char c)
    {

        var specialCharacters = new char[] { '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '+', '=' };

        return specialCharacters.Contains(c);
    }
}