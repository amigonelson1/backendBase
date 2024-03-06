using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess
{
    public class TestChildTable
    {
        [Key]
        public int Id { get; set; }
        public string IdParent { get; set; } = null!;
        public double Doublevalue { get; set; }

        #region Navigations properties
            [ForeignKey("IdParent")]
            public TestParentTable Parent { get; set; } = null!;
        #endregion
    }
}