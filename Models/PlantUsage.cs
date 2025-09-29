using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plantpedia.Models
{
    [Table("plant_usage")]
    public class PlantUsage
    {
        [Key, Column("plant_id", TypeName = "char(10)")]
        public string PlantId { get; set; }
        public PlantInfo Plant { get; set; }

        [Key, Column("usage_id", TypeName = "char(10)")]
        public string UsageId { get; set; }
        public Usage Usage { get; set; }
    }
}
