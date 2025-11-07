using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Events
    {
            public Guid Id { get; internal set; }

            public required string Title { get; set; }

            public required string Description { get; set; }

            public required DateTime StartTime { get; set; }

            public required DateTime EndTime { get; set; }

        [ForeignKey("Owner")]
        public string OwnerId { get; set; }= string.Empty;

        public required Users Owner { get; set; }


        public List<string>? ReadersIds { get; set; }
        public List<string>? EditorsIds { get; set; }
        public List<string>? AdminsIds { get; set; }

        public Users[]? Collaborators { get; set; }



    }
}
