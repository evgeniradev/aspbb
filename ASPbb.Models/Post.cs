using System;
using System.ComponentModel.DataAnnotations;

namespace ASPbb.Models
{
    public class Post
    {
        public Post()
        {
            this.CreatedDate = DateTime.UtcNow;
            this.UpdatedDate = DateTime.UtcNow;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(5000)]
        public string Content { get; set; }

        public int TopicId { get; set; }
        public Topic Topic { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
