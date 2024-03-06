using System.ComponentModel.DataAnnotations;

namespace DataAccess
{
    public class TestParentTable
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [StringLength(20)]
        public string StringValue { get; set; } = null!;
        public int IntValue { get; set; }
        public double DoubleValue { get; set; }
        public DateTime DateValue { get; set; }

        #region Navigations properties
            public List<TestChildTable> Childs { get; set; } = null!;
        #endregion
    }
}