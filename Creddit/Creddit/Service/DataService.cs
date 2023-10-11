using System;
using Creddit.Model;
using System.Collections.Generic; // Importerer namespace for at arbejde med lister.


namespace Creddit.Service
{
    public class DataService
    {
        private CredditContext db { get; } // Privat felt til at gemme en reference til RedditContext, som er databasekonteksten.

        public DataService(CredditContext db)
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
                post = new Post { PostId = 1, Title = "Post om alverden", User = user1, Content = "En vigtig post om alverden", Downvote = 2, Upvote = 12, NumberOfVotes = 14 };
                db.Add(post);
                db.SaveChanges();
            }

            Comment comment = db.Comments.FirstOrDefault()!;
            if (comment == null)
            {
                comment = new Comment { CommentId = 1, Content = "Det lyder bare spændende", Downvote = 3, Upvote = 5, NumberOfVotes = 8 };
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

        /*// Henter alle poster som en liste
        public List<Post> GetPosts()
        {
            return db.Posts.ToList(); // Returnerer en liste af alle poster fra databasen ved at konvertere dem til en liste.
        }

        */

        //ET FORSØG PÅ INCLUDE HENTER ALLE POSTS
         
        public List<Post> GetPosts() {
        return db.Posts.Include(c => c.Comments).ThenInclude(c => c.User).ToList();
        }


        public Post GetPost(int id)
        {
            return db.Posts.Include(c => c.Comments).ThenInclude(c => c.User).FirstOrDefault(p = p.PostId == id);
        }
         


       // CREATE POST
        public string CreatePost(string title, User user, string content, int upvote, int downvote, int numberOfVotes, DateTime postTime) {

        User user = db.Users.FirstOrDefault(a => a.UserId == user.UserId)!;
        if(user==null){
            //db.Users.Add()
            return "User not found";//db.Posts.Add(new Post(title,user,text, upvote, downvote, numberOfVotes, DateTime.Now));
        }
        else{
            Post post = new Post(title, user, content, upvote, downvote, numberOfVotes, DateTime.Now); 
            db.Posts.Add(post);
        }
        db.SaveChanges();
        return "Post created";

        }

        // CREATE COMMMENT
         
        public string CreateComment(string content, User user, int downvote, int upvote, int numberOfVotes, DateTime CommentTime,int postid) { //bruger id til at finde en specifik author- kan ikke lave nye authors.
        Post post = db.Posts.FirstOrDefault(p => p.PostId == postid)!;
        if (post = null) {
            return "Post not found"
        }

        if (db == null) {
            return "Database not initialized";
        }
        else 
        {
            Comment comment = new Comment(content, user,downvote,upvote,numberOfVotes,commentTime);
            post.Comments.Add(comment);
            db.Posts.Add(comment);
            db.SaveChanges();
        }
        

         public bool PostVoting(int postId, User user, bool UpvoteOrDownvote) 
         {
            var post = db.Posts.FirstOrDefault(p => p.postId == postId);
            if (post == null) 
            {
                return false;
            }

            if (UpvoteOrDownvote == true) 
            {
                post.Upvote++;
                post.NumberOfVotes++;
                db.SaveChanges();

                return true;
            }

            else if (UpvoteOrDownvote == false) 
            {
                post.Downvote--;
                post.NumberOfVotes++;
                db.SaveChanges();
                
                return false;
            }
         }


        public bool CommentVoting(int commentId, User user, bool UpvoteOrDownvote)
        {
            {
                var comment = db.Comments.FirstOrDefault(c = char.CommentId == commentId);
                if (comment == null)
                {
                    return false;
                }
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
                commentId.Downvote--:
                comment.NumberOfVotes++;
                db.SaveChanges();

                return false;
            }
        }

        /*public Post GetPost()
        {
            return db.Posts.Where(p => p.PostId == postid).FirstOrDefault()!;
        }
*/

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
}

