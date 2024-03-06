using System.ComponentModel.DataAnnotations;

namespace Business.Validators
{
    public class RangeDate : ValidationAttribute
    {
        private readonly DateTime _startDate;
        private readonly DateTime _endDate;

        public RangeDate(string startDate, string endDate)
        {
            _startDate = new DateTime(int.Parse(startDate.Split("/")[0]), int.Parse(startDate.Split("/")[1]), int.Parse(startDate.Split("/")[2]));
            _endDate = new DateTime(int.Parse(endDate.Split("/")[0]), int.Parse(endDate.Split("/")[1]), int.Parse(endDate.Split("/")[2]));
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if(value == null)
            {
                return new ValidationResult("Required Date");
            }
            var date = (DateTime)value;
            if(date.Date < _startDate.Date || date.Date > _endDate.Date){
                return new ValidationResult("date out of range");
            }
            return ValidationResult.Success;
        }
    }
}