using System.ComponentModel.DataAnnotations;

namespace ExampleEmpty.UI.AppUtilities
{
    public class EmailDomainValidator : ValidationAttribute
    {
        private readonly string emailDomain;
        public EmailDomainValidator(string emailDomain)
        {
            this.emailDomain = emailDomain;
        }
        public override bool IsValid(object value)
        {
            var result = value.ToString().Split('@');
            return result[1].ToUpper() == emailDomain.ToUpper();
        }

    }
}
