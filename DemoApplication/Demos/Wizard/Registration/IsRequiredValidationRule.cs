using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Globalization;
using BrokenHouse.Utils;


namespace DemoApplication.Demos.Wizard.Registration
{
     public class IsRequiredValidationRule : ValidationRule
    {
        public string ErrorMessage { get; set; }
        
        public override ValidationResult Validate( object value, CultureInfo cultureInfo )
        {
            bool    isValid = (value != null);

            return new ValidationResult(isValid, isValid? null : ErrorMessage);
        }
    }
}
