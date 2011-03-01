using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Globalization;
using BrokenHouse.Utils;

namespace DemoApplication.Demos.Wizard.Registration
{
     public class StringLengthValidationRule : ValidationRule
    {
        public int? Minimum { get; set; }
        
        public int? Maximum { get; set; }
        
        public string ErrorMessage { get; set; }
        
        public override ValidationResult Validate( object value, CultureInfo cultureInfo )
        {
            int     minimum = Minimum ?? -1;
            int     maximum = Maximum ?? int.MaxValue;
            string  input   = (value ?? string.Empty).ToString();
            bool    isValid = ((input.Length >= minimum) && (input.Length <= maximum));

            return new ValidationResult(isValid, isValid? null : ErrorMessage);
        }
    }
}
