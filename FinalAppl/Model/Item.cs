using System.ComponentModel.DataAnnotations;

namespace CakeApplication.Model
{
    public class Item
    {

        [Key]
        public int Id { get; set; }
        public string itemType { get; set; }
        public string itemName { get; set; }
        public string description { get; set; }
        public int cost { get; set; }
        public string imglink { get; set; }
    }
}
