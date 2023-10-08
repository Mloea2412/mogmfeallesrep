using System; // Importerer System-namespace, som indeholder almindelige systemklasser og -metoder.
using Reddit2._0.Model; // Importerer Model-namespace for at bruge klasserne Post, Comment og User.
using System.Collections.Generic; // Importerer namespace for at arbejde med lister.

namespace Reddit2._0.Service
{
    public class DataService
    {
        private RedditContext db { get; } // Privat felt til at gemme en reference til RedditContext, som er databasekonteksten.

        public DataService(RedditContext db)
        {
            this.db = db; // Konstruktøren modtager en RedditContext-injektion og tildeler den til det private felt.
        }

        // Henter alle poster som en liste
        public List<Post> GetAllPosts()
        {
            return db.Posts.ToList(); // Returnerer en liste af alle poster fra databasen ved at konvertere dem til en liste.
        }

        // Henter en specifik post baseret på dens id
        public Post GetPostById()
        {
            return db.Posts.Where(p => p.PostId == postid).FirstOrDefault()!;
        }
        

        // Henter alle kommentarer som en liste
        public List<Comment> GetAllComments()
        {
            return db.Comments.ToList(); // Returnerer en liste af alle kommentarer fra databasen ved at konvertere dem til en liste.
        }

        // Henter en specifik kommentar baseret på dens id
        public Comment GetCommentById(int commentid)
        {
            return db.Comments.Where(c => c.CommentId == commentid).FirstOrDefault()!; // Henter en kommentar fra databasen baseret på dens id og returnerer den.
        }

        // Henter alle brugere som en liste
        public List<User> GetAllUsers()
        {
            return db.Users.ToList(); // Returnerer en liste af alle brugere fra databasen ved at konvertere dem til en liste.
        }

        // Henter en specifik bruger baseret på dens id
        public User GetUserById(int userid)
        {
            return db.Users.Where(u => u.UserId == userid).FirstOrDefault()!; // Henter en bruger fra databasen baseret på dens id og returnerer den.
        }

        // Håndterer op- og nedstemninger på en post
        public bool PostVoting(int postId, User user, bool UpvoteOrDownvote)
        {
            var post = db.Posts.FirstOrDefault(p => p.PostId == postId); // Finder posten baseret på dens id.
            if (post == null)
            {
                return false; // Hvis posten ikke findes, returneres falsk.
            }

            if (UpvoteOrDownvote == true) // Hvis UpvoteOrDownvote er sand (opstemning).
            {
                post.Upvote++; // Øger antallet af opstemninger på posten.
                post.NumberOfVotes++; // Øger det samlede antal stemmer på posten.
                db.SaveChanges(); // Gemmer ændringerne i databasen.
            }
            else if (UpvoteOrDownvote == false) // Hvis UpvoteOrDownvote er falsk (nedstemning).
            {
                post.Downvote++; // Øger antallet af nedstemninger på posten.
                post.NumberOfVotes++; // Øger det samlede antal stemmer på posten.
                db.SaveChanges(); // Gemmer ændringerne i databasen.
            }
            else
            {
                return false; // Hvis UpvoteOrDownvote er hverken sand eller falsk, returneres falsk.
            }

            return true; // Returnerer sand, hvis op- eller nedstemningen blev behandlet korrekt.
        }
    }

    ///..............

    // Skal have en commentvoting i samme stil - jeg har en idé
    // Skal have noget seeddata, jeg har også en idé her
}
