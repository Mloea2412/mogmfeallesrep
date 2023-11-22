using System;

namespace MmReddit.Model
{
    public class Comment
    {
        // Egenskaber (Properties) for kommentaren
        public int CommentId { get; set; } // Unikt ID for kommentaren
        public string Content { get; set; } // Teksten i kommentaren
        public int Upvotes { get; set; } // Antal upvotes for kommentaren
        public int Downvotes { get; set; } // Antal downvotes for kommentaren
        public int NumberOfVotes { get; set; } // Samlet antal afstemninger (upvotes - downvotes)
        public User User { get; set; } // Brugeren, der har oprettet kommentaren
        public DateTime CommentTime { get; set; } // Tidspunktet, hvor kommentaren blev oprettet

        // Konstruktør til at oprette en ny kommentar
        public Comment(string content, int downvotes, int upvotes, int numberOfVotes, User user, DateTime commentTime)
        {
            // Initialiserer egenskaberne med de angivne værdier
            Content = content;
            Upvotes = upvotes;
            Downvotes = downvotes;
            NumberOfVotes = numberOfVotes;
            User = user;
            CommentTime = commentTime;
        }

        // Standardkonstruktør (uden parametre)
        public Comment()
        {
            CommentId = 0; // Initialiserer CommentId til 0 (typisk brugt for nye, ikke-gemte kommentarer)
            Content = ""; // Initialiserer Content til en tom streng
            Upvotes = 0; // Initialiserer Upvotes til 0
            Downvotes = 0; // Initialiserer Downvotes til 0
            NumberOfVotes = 0; // Initialiserer NumberOfVotes til 0 (samlet antal stemmer)
        }
    }
}
