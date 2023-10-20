using System;
using MmReddit.Model;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Forms;

namespace MmReddit.Service
{
    public class DataService
    {

        private MmRedditContext db { get; }

        public DataService(MmRedditContext db)
        {
            this.db = db;
        }

        public List<Post> Posts()
        {
            return db.Posts.ToList();
        }

        // Henter alle kommentarer som en liste
        public List<Post> GetPosts()
        {
            return db.Posts.Include(p => p.Comments).ThenInclude(p => p.User).ToList();
        }

        public List<User> GetUsers()
        {
            return db.Users.ToList();
        }


        // Henter den specifike post ud fra dets Id
        public Post GetPost(int id)
        {
            return db.Posts.Include(p => p.Comments).ThenInclude(p => p.User).FirstOrDefault(p => p.PostId == id);
        }



        // CREATE POST
        public string CreatePost(string title, User user, string content, int upvotes, int downvotes, int numberOfVotes, DateTime postTime)
        {
            // Kontrollér, om brugeren findes i databasen
            User user1 = db.Users.FirstOrDefault(u => u.UserId == user.UserId);
            if (user1 == null)
            {
                return "Bruger ikke fundet";
            }

            // Opret et nyt Post-objekt
            Post nyPost = new Post(title, user1, content, upvotes, downvotes, numberOfVotes, postTime);

            // Tilføj den nye post til databasen
            db.Posts.Add(nyPost);
            db.SaveChanges();

            return "Opstilling oprettet";
        }


        // CreateComment
        public string CreateComment(string content, int upvotes, int downvotes, int postid, User user, DateTime commentTime)
        {
            // Find den ønskede post i databasen
            Post post = db.Posts.Include(p => p.Comments).FirstOrDefault(p => p.PostId == postid);

            if (post == null)
            {
                return "Post not found";
            }

            // Opret en ny kommentar
            Comment comment = new Comment(content, upvotes, downvotes, user);

            // Tilføj kommentaren til posten
            post.Comments.Add(comment);

            // Gem ændringerne i databasen
            db.SaveChanges();

            return "Comment created";
        }

        public void SeedData()
        {
            User user = db.Users.FirstOrDefault()!;
            if (user == null)
            {
                User user2 = new User("M");
                User user3 = new User("N");
                User user4 = new User("Magnus");
                db.Add(user2);
                db.Add(user3);
                db.Add(user4);
                db.SaveChanges();
            }
        }


    }


}



/////////// TIDLIGERE BENYTTET KODE /////////
///  /*//henter post og returner dem som en liste
/*public List<Comment> Comments()
{
    return db.Comments.ToList();
}

// Henter post på dets id
public Post Post(int postid)
{
    return db.Posts.Where(p => p.PostId == postid).FirstOrDefault()!;

}

// Henter kommentaren på dets id
public Comment Comment(int commentid)
{
    return db.Comments.Where(p => p.CommentId == commentid).FirstOrDefault()!;

}

// Henter bruger på dets id
public User User(int userid)
{
    return db.Users.Where(p => p.UserId == userid).FirstOrDefault()!;
}

// Henter alle brugere
public List<User> Users()
{
    return db.Users.ToList();
}
*/



/*public void SeedData()
        {
            if (!db.Posts.Any())
            {
                User user1 = new User("Testuser");
                User user2 = new User("Magnus");
                User user3 = new User("M-L");
                User user4 = new User("GithubHater22");

                Post post1 = new Post("Github", user1, "Github er et fantastisk værktøj som gør mit liv lettere", 12, 2, 14, DateTime.Now);
                Comment comment1 = new Comment("Det har du ret i! Jeg elsker Github!!", user1, 5, 3);
                Comment comment2 = new Comment("Github er spild af tid og giver ikke mening at bruge", user2, 0, 8);
                Comment comment3 = new Comment("Github virker ikke altid, men er godt når det virker", user3, 5, 2);

                post1.Comments.Add(comment1);
                post1.Comments.Add(comment2);
                post1.Comments.Add(comment3);

                Post post2 = new Post("#!$@ Github!!", user4, "Jeg hader Github. Det er et dårligt værktøj!!!", 1, 3, 4, DateTime.Now);
                Comment comment4 = new Comment("Ej, du har bare ret og er super smart", user2, 2, 0);
                post2.Comments.Add(comment4);

                db.Posts.Add(post1);
                db.Posts.Add(post2);
                db.SaveChanges();
            }
        }
        */


/*public void SeedData()
{
    if (!db.Users.Any())
    {
        User user = new User("Testuser");
        db.Users.Add(user);

        Post post = new Post("Min første post", user, "Dette er min første post på Reddit.", 10, 2, 12, DateTime.Now);
        db.Posts.Add(post);

        Comment comment = new Comment("Hej verden!", user, 3, 1);
        post.Comments.Add(comment);

        db.SaveChanges();
    }
}

*/