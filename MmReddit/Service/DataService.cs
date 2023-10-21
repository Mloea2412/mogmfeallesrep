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


        // HENTER ALLE KOMMENTARENE UD SOM EN LISTE MED TILHØRENDE OPLYSNINGER OG KOMMENTARER
        public List<Post> GetPosts()
        {
            return db.Posts.Include(p => p.Comments).ThenInclude(p => p.User).ToList();
        }

        // HENTER ALLE BRUGERNE
        public List<User> GetUsers()
        {
            return db.Users.ToList();
        }

        // DER BLIVER IKKE HENTET USERID MEN USER = NULL NÅR DER BLIVER HENTET POSTS. HVERKEN VED ALLE PÅ DETS ID
        // OG PÅ COMMENT ER POSIID = NULL

        // HENTER POST UD FRA DETS ID
        public Post GetPost(int id)
        {
            return db.Posts.Include(p => p.Comments).ThenInclude(p => p.User).FirstOrDefault(p => p.PostId == id)!;
        }
       

        // LAVES FÆRDIG SÅ DEN VIRKER
        public string CreatePost(string title, User user, string content, int upvote, int downvote, int numberOfVotes, DateTime postTime)
        {

            User tempuser = db.Users.FirstOrDefault(a => a.UserId == user.UserId)!;
            if (tempuser == null)
            {
                //db.Users.Add()
                db.Posts.Add(new Post(title, user, content, upvote, downvote, numberOfVotes, DateTime.Now));
            }
            else
            {
                db.Posts.Add(new Post(title, tempuser, content, upvote, downvote, numberOfVotes, DateTime.Now));
            }
            db.SaveChanges();
            return "Post created";

        }

        // SKAL LAVES FÆRDIG SÅ DEN VIRKER
        /*// CREATE POST
        public string CreatePost(string title, User user, string content, int upvotes, int downvotes, int numberOfVotes, DateTime postTime)
        {
            // Kontrollér, om brugeren findes i databasen
            User user1 = db.Users.FirstOrDefault(u => u.UserId == user.UserId)!;
            if (user1 == null)
            {
                return "Bruger ikke fundet";
            }
            // Opret et nyt Post-objekt
            Post nyPost = new Post(title, user1, content, upvotes, downvotes, numberOfVotes, postTime);

            // Tilføj den nye post til databasen
            db.Posts.Add(nyPost);
            db.SaveChanges();

            return "Post oprettet";
        }
        */

        // SKAL LAVES FÆRDIG
        // CreateComment
        public string CreateComment(string content, int upvotes, int downvotes, int numberOfVotes, int postid, User user, DateTime CommentTime)
        {
            var post = db.Posts.Where(p => p.PostId == postid).FirstOrDefault();
            if (post == null)
            {
                return "Post not found";
            }

            if (db == null)
            {
                return "Database not initialized";
            }

            post.Comments.Add(new Comment(content, downvotes, upvotes, numberOfVotes, user, DateTime.Now));
            db.SaveChanges();
            return "Comment created"; return "Comment created";
        }


        // SKAL LAVES FÆRDIG SÅ DET VIRKER MED KOMMENTARENE, AT DE HAR ET POSTID
        public void SeedData()
        {
            User user1 = new User { Username = "M-L" };
            if (db.Users.FirstOrDefault(u => u.Username == user1.Username) == null)
            {
                db.Add(user1);
                db.SaveChanges();
            }

            Post post = new Post
            {
                Title = "Sikke dejligt vejr",
                User = user1,
                Content = "Hvad tænker I?",
                Downvotes = 0,
                Upvotes = 10,
                NumberOfVotes = 10
            };

            if (db.Posts.FirstOrDefault(p => p.Title == post.Title) == null)
            {
                db.Add(post);
                db.SaveChanges();
            }

            Comment comment = new Comment
            {
                Content = "Vi blæser væk",
                Downvotes = 4,
                Upvotes = 5,
                NumberOfVotes = 9,
                User = user1, // Brug den samme bruger som ejer indlægget
                CommentTime = DateTime.Now // Tilføj tidspunkt
            };

            // SKAL KOBLES PÅ, AT KOMMENTAREN TILHØRE OPSLAGET. DER SKAL ET POSTID PÅ
            if (db.Comments.FirstOrDefault(c => c.Content == comment.Content) == null)
            {
                db.Add(comment);
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


