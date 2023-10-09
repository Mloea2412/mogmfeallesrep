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


        // SEED DATA HER - ET BUD PÅ SEEDDATA

        public void SeedData()
        {
            Post post = db.Posts.FirstOrDefault()!;
            if (post == null)
            {
                User user1 = new User("M");
                post = new Post { PostId = 1, Title = "Post om alverden", User = user1, Text = "En vigtig post om alverden", Downvote = 2, Upvote = 12, NumberOfVotes = 14 };
                db.Add(post);
                db.SaveChanges();
            }

            Comment comment = db.Comments.FirstOrDefault()!;
            if (comment == null)
            {
                comment = new Comment { CommentId = 1, Text = "Det lyder bare spændende", Downvote = 3, Upvote = 5, NumberOfVotes = 8 };
                db.Add(comment);
                db.SaveChanges();

            }

            User user = db.Users.FirstOrDefault();
            if (user == null)
            {
                User user1 = new User("Testuser");
                User user2 = new User("Magnus");
                User user3 = new User("M-L");
                User user4 = new User("TestUser4");
                db.Add(user1);
                db.Add(user2);
                db.Add(user3);
                db.Add(user4);
                db.SaveChanges();

            }
        }

        // Henter alle poster som en liste
        public List<Post> GetPosts()
        {
            return db.Posts.ToList(); // Returnerer en liste af alle poster fra databasen ved at konvertere dem til en liste.
        }

        // Henter en specifik post baseret på dens id
        public Post GetPost()
        {
           return db.Posts.Where(p => p.PostId == postid).FirstOrDefault()!;
        }
        

        // Henter alle kommentarer som en liste
        public List<Comment> GetComments()
        {
            return db.Comments.ToList(); // Returnerer en liste af alle kommentarer fra databasen ved at konvertere dem til en liste.
        }

        // Henter en specifik kommentar baseret på dens id
        public Comment GetComment(int commentid)
        {
            return db.Comments.Where(c => c.CommentId == commentid).FirstOrDefault()!; // Henter en kommentar fra databasen baseret på dens id og returnerer den.
        }

        // Henter alle brugere som en liste
        public List<User> GetUsers()
        {
            return db.Users.ToList(); // Returnerer en liste af alle brugere fra databasen ved at konvertere dem til en liste.
        }

        // Henter en specifik bruger baseret på dens id
        public User GetUserId(int userid)
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
