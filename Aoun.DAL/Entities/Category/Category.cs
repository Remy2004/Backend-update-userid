using System.ComponentModel.DataAnnotations;

namespace Aoun.DAL.Entities.Category
{
    public class Category
    {

        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        // public DateTime CreatedAt { get; set; }
        public List<Case> Cases { get; set; }

    }
}
