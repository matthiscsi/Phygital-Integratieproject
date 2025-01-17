// using System.ComponentModel.DataAnnotations;
// using Microsoft.Extensions.DependencyInjection;
// using UI.MVC.Controllers;
//
// namespace PIP.BL.Validation;
//
// public class NoProfanityAttribute : ValidationAttribute
// {
//     private readonly IFilter _filter;
//
//     public NoProfanityAttribute(IFilter filter)
//     {
//         _filter = filter;
//     }
//
//     protected override ValidationResult IsValid(object value, ValidationContext validationContext)
//     {
//         var stringValue = value as string;
//
//         if (!string.IsNullOrEmpty(stringValue) && _filter.ContainsProfanity(stringValue))
//         {
//             return new ValidationResult("The field contains prohibited words. Please remove them.");
//         }
//
//         return ValidationResult.Success;
//     }
//     
//   
// }