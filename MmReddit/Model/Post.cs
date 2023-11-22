using System;
namespace MmReddit.Model
{
    public class Post
    {
        // Egenskaber (Properties) for opslaget
        public int PostId { get; set; } // Unikt ID for opslaget
        public string Title { get; set; } // Titlen på opslaget
        public User User { get; set; } // Brugeren, der har oprettet opslaget
        public string Content { get; set; } // Indholdet af opslaget
        public int Upvotes { get; set; } // Antal upvotes for opslaget
        public int Downvotes { get; set; } // Antal downvotes for opslaget
        public int NumberOfVotes { get; set; } // Samlet antal afstemninger (upvotes - downvotes)
        public DateTime PostTime { get; set; } // Tidspunktet, hvor opslaget blev oprettet
        public List<Comment> Comments { get; set; } = new List<Comment>(); // En liste af tilhørende kommentarer

        // Konstruktør til at oprette et nyt opslag
        public Post(string title, User user, string content, int upvotes, int downvotes, int numberOfVotes, DateTime postTime)
        {
            // Initialiserer egenskaberne med de angivne værdier
            Title = title;
            User = user;
            Content = content;
            Upvotes = upvotes;
            Downvotes = downvotes;
            NumberOfVotes = numberOfVotes;
            PostTime = postTime;
        }

        // Standardkonstruktør (uden parametre)
        public Post()
        {
            PostId = 0; // Initialiserer PostId til 0 (typisk brugt for nye, ikke-gemte opslag)
            Title = ""; // Initialiserer Title til en tom streng
            User = null; // Initialiserer User til null (typisk brugt for nye, ikke-tilknyttede opslag)
            Content = ""; // Initialiserer Content til en tom streng
            Upvotes = 0; // Initialiserer Upvotes til 0
            Downvotes = 0; // Initialiserer Downvotes til 0
            PostTime = DateTime.Now; // Initialiserer PostTime til det aktuelle tidspunkt
        }
    }
}
