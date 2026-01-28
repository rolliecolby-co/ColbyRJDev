namespace ColbyRJ.Models.CustomValidators
{
    public class EmailDomainValidator : ValidationAttribute
    {
        public string AllowedDomain { get; set; }

        protected override ValidationResult IsValid(object value,
            ValidationContext validationContext)
        {
            string[] strings = value.ToString().Split('@');
            if (strings[1].ToUpper() == AllowedDomain.ToUpper())
            {
                return null;
            }

            return new ValidationResult($"Domain must be {AllowedDomain}",
            new[] { validationContext.MemberName });
        }
    }

    public class RemarksValidator : ValidationAttribute
    {
        public string Remarks { get; set; }

        protected override ValidationResult IsValid(object value,
            ValidationContext validationContext)
        {
            var remarks = value.ToString();
            if (remarks.Length > 10)
            {
                return null;
            }

            return new ValidationResult($"Please elaborate on {Remarks} narrative",
            new[] { validationContext.MemberName });
        }
    }

    public class YearValidator : ValidationAttribute
    {
        public string YearStr { get; set; }

        protected override ValidationResult IsValid(object value,
            ValidationContext validationContext)
        {
            if (value == null)
            {
                return null;
            }

            var yearStr = value.ToString();
            if (yearStr.Length == 0)
            {
                return null;
            }

            try
            {
                int yearInt = Convert.ToInt32(value.ToString());
                if (yearInt > 1800 && yearInt < 2400)
                {
                    return null;
                }
            }
            catch { }

            return new ValidationResult($"Please enter a valid 4-digit Year",
            new[] { validationContext.MemberName });
        }
    }

    public class MonthValidator : ValidationAttribute
    {
        public string MonthStr { get; set; }

        protected override ValidationResult IsValid(object value,
            ValidationContext validationContext)
        {
            if (value == null)
            {
                return null;
            }

            var monthStr = value.ToString();
            if (monthStr.Length == 0)
            {
                return null;
            }

            try
            {
                int monthInt = Convert.ToInt32(value.ToString());
                if (monthInt > 0 && monthInt < 13)
                {
                    return null;
                }
            }
            catch { }

            return new ValidationResult($"Please enter a valid Month integer",
            new[] { validationContext.MemberName });
        }
    }

    public class IntNotZeroValidator : ValidationAttribute
    {
        public string ValueStr { get; set; }

        protected override ValidationResult IsValid(object value,
            ValidationContext validationContext)
        {
            try
            {
                int valueInt = Convert.ToInt32(value.ToString());
                if (valueInt > 0)
                {
                    return null;
                }
            }
            catch { }

            return new ValidationResult($"Select {ValueStr}",
            new[] { validationContext.MemberName });
        }
    }
}
