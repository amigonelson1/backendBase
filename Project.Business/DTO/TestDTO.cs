using System.ComponentModel.DataAnnotations;
using Business.Validators;

namespace Business.DTOs
{
    public class TestDTO
    {
        public string Id { get; set; } = null!;
        [Required]
        [StringLength(20)]
        public string StringValue { get; set; } = null!;
        [Required]
        public int IntValue { get; set; }
        [Required]
        [Range(0.0, 1000.00, ErrorMessage = "El campo {0} debe ser mayor que {1}.")]
        public double DoubleValue { get; set; }
        [Required]
        [RangeDate(startDate: "2022/01/01", endDate: "2023/01/01")]
        public DateTime DateValue { get; set; }

        #region Navigations Properties
            public List<double>? Childs { get; set; } = null!;
        #endregion
    }
}