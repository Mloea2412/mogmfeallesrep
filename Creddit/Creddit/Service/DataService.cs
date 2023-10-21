using System;
using Creddit.Model;
using System.Collections.Generic; // Importerer namespace for at arbejde med lister.
using Microsoft.EntityFrameworkCore;


namespace Creddit.Service
{
    public class DataService
    {
        private CredditContext db { get; } // Privat felt til at gemme en reference til RedditContext, som er databasekonteksten.

        public DataService(CredditContext db)
        {
            this.db = db; // Konstruktøren modtager en RedditContext-injektion og tildeler den til det private felt.
        }

        public void SeedData() //Seeder Data ind i vores database, hvis der ikke er noget i den.
        {
            Post post = db.Posts.FirstOrDefault()!;
            if (post == null)
            {
                Post post2 = db.Posts.FirstOrDefault()!;
                User user1 = new User("Testuser");
                User user2 = new User("Magnus");
                User user3 = new User("M-L");
                User user4 = new User("GithubHater22");

                post = new Post("Github", user1, "Github er et fantastisk værktøj som gør mit liv lettere", 12, 2, 14, DateTime.Now);
                Comment comment1 = new Comment("Det har du ret i! Jeg elsker Github!!", user1, 5, 3, 8, DateTime.Now);
                Comment comment2 = new Comment("Github er spild af tid og giver ikke mening at bruge", user2, 0, 8, 8, DateTime.Now);
                Comment comment3 = new Comment("Github virker ikke altid, men er godt når det virker", user3, 5, 2, 7, DateTime.Now);

                post.Comments.Add(comment1);
                post.Comments.Add(comment2);
                post.Comments.Add(comment3);
                db.Posts.Add(post);
                post2 = new Post("#!$@ Github!!", user4, "Jeg hader github. Det er et dårligt værktøj!!!", 1, 3, 4, DateTime.Now);
                Comment comment4 = new Comment("Ej du har bare ret og er super smart", user2, 0, 2, 2, DateTime.Now);
                post2.Comments.Add(comment4);
                db.Posts.Add(post2);
                db.SaveChanges();
            }
        }

        // CREATE POST
        public string CreatePost(string title, User user, string content, int upvote, int downvote, int numberOfVotes, DateTime postTime)
        {

            Post localu = db.Posts.Include(a=> a.User == user).FirstOrDefault(a => a.User.UserId == user.UserId)!;
            if (localu  == null) {
                //db.Users.Add()
                return "User not found";//db.Posts.Add(new Post(title,user,text, upvote, downvote, numberOfVotes, DateTime.Now));
            }
            else {
                Post post = new Post(title, user, content, upvote, downvote, numberOfVotes, postTime);
                db.Posts.Add(post);
            }
            db.SaveChanges();
            return "Post created";
        }
        
        // CREATE COMMMENT
        public string CreateComment(string content, User user, int downvote, int upvote, int numberOfVotes, DateTime CommentTime, int postid)
        { 
            Post post = db.Posts.Include(c => c.Comments).ThenInclude(u => u.User == user).FirstOrDefault(p => p.PostId == postid)!;

                        if (db == null)
            {
                return "Database not initialized";
            }
            else
            {
                Comment comment = new Comment(content, user, downvote, upvote, numberOfVotes, CommentTime);
                post.Comments.Add(comment);
                db.SaveChanges();
                return "Comment commented";
            }
        }
            // Henter alle poster som en liste
            public List<Post> GetPosts()
            {
                return db.Posts.Include(c => c.Comments).ThenInclude(c => c.User).ToList();
            }

            // Henter specifikt post
            public Post GetPost(int Postid)
            {
                return db.Posts.Include(c => c.Comments).ThenInclude(c => c.User).FirstOrDefault(c => c.PostId == Postid);
            }
            // Henter alle kommentarer som en liste
            public List<Post> GetComments()
            {
                return db.Posts.Include(c => c.Comments).ToList(); // Returnerer en liste af alle kommentarer fra databasen ved at konvertere dem til en liste.
            }

            // Henter en specifik kommentar baseret på dens id
            public Post GetComment(int commentid)
            {
                return db.Posts.Include(c => c.Comments).FirstOrDefault(c => c.Comment.CommentId == commentid)!; // Henter en kommentar fra databasen baseret på dens id og returnerer den.
            }

            // Henter alle brugere som en liste
            public List<Post> GetUsers()
            {
                return db.Posts.Include(u => u.User).ToList(); // Returnerer en liste af alle brugere fra databasen ved at konvertere dem til en liste.
            }

            // Henter en specifik bruger baseret på dens id
            public Post GetUserId(int userid)
            {
                return db.Posts.Include(u => u.User).FirstOrDefault(u => u.User.UserId == userid)!; // Henter en bruger fra databasen baseret på dens id og returnerer den.
            }
     // Håndterer op- og nedstemninger på en post
     public bool PostVoting(int postId, User user, bool UpvoteOrDownvote)
        {
            Post post = db.Posts.Include(u => u.User == user).FirstOrDefault(p => p.PostId == postId);
            // Finder posten baseret på dens id.
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

     // Håndterer op- og nedstemninger på en comment
     public bool CommentVoting(int commentId, User user, bool UpvoteOrDownvote)
        {
            Post comment = db.Posts.Include(c => c.Comments).ThenInclude(u => u.User == user).FirstOrDefault(c => c.Comment.CommentId == commentId);
            if (comment == null)
            {
                return false;
            }
            if (UpvoteOrDownvote == true)

            {
                comment.Upvote++;
                comment.NumberOfVotes++;
                db.SaveChanges();
                return true;
            }

            else if (UpvoteOrDownvote == false)
            {
                comment.Downvote--;
                comment.NumberOfVotes++;
                db.SaveChanges();
                return false;
            }
            return true;
        }
    }
}


