using System;
using System.ComponentModel.DataAnnotations;
using Teretana.Models;

namespace Teretana
{
    public class TimeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Clanovi clanovi = (Clanovi)validationContext.ObjectInstance;
            if (value == null)
            {
                ErrorMessage = "Neispravan datum";
                return new ValidationResult(ErrorMessage);
            }
            var date = DateTime.Parse(value.ToString());

            if (date < clanovi.VremeOd.AddMonths(1))
            {
                ErrorMessage = "Clanarina je minimum 1 mesec";
                return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}