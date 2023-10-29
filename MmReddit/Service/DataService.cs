using System;
using MmReddit.Model;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.Design;

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
            return db.Posts.Include(p => p.Comments).ThenInclude(u => u.User).Include(p => p.User).OrderByDescending(c => c.PostTime).ToList();
        }

        // HENTER ALLE BRUGERNE
        public List<User> GetUsers()
        {
            return db.Users.ToList();
        }

        // HENTER ALLE KOMMENTARENE SOM EN LISTE
        public List<Comment> GetComments()
        {
            return db.Comments.ToList();
        }
        // DER BLIVER IKKE HENTET USERID MEN USER = NULL NÅR DER BLIVER HENTET POSTS. HVERKEN VED ALLE PÅ DETS ID
        // OG PÅ COMMENT ER POSIID = NULL

        // HENTER POST UD FRA DETS ID
        public Post GetPost(int id)
        {
            return db.Posts.Include(p => p.Comments).ThenInclude(u => u.User).Include(p => p.User).FirstOrDefault(p => p.PostId == id)!;
        }
        public User GetUserId(int userid)
        {
            return db.Users.FirstOrDefault(u => u.UserId == userid)!; // Henter en bruger fra databasen baseret på dens id og returnerer den.
        }
        public Comment GetComment(int commentid)
        {
            return db.Comments.Include(u => u.User).FirstOrDefault(c => c.CommentId == commentid)!; // Henter en kommentar fra databasen baseret på dens id og returnerer den.
        }

        public string CreatePost(string title, User user, string content, int upvotes, int downvotes, int numberOfVotes, DateTime postTime)
        {
            // LAVER KONTROL PÅ OM BRUGEREN FINDES I DATABASEN.
            User user1 = db.Users.FirstOrDefault(u => u.UserId == user.UserId)!;
            if (user1 == null)
            {
                return "Bruger ikke fundet";
            }
            // NYT POST
            Post nyPost = new Post(title, user1, content, upvotes, downvotes, numberOfVotes, postTime);

            // TILFØJER DET NYE POST TIL DATABASEN
            db.Posts.Add(nyPost);
            db.SaveChanges();

            return "Post oprettet";
        }
        // CREATECOMMENT - OPRETTER EN NY KOMMENTAR
        public string CreateComment(string content, int upvotes, int downvotes, int numberOfVotes, int postid, User user, DateTime CommentTime)
        {
            // LAVER KONTROL PÅ OM POSTET FINDES I DATABASEN.
            Post post = db.Posts.FirstOrDefault(p => p.PostId == postid);
            if (post == null)
            {
                return "Post not found";
            }
            // TJEKKER OM DB ER NULL
            if (db == null)
            {
                return "Database not initialized";
            }
            // LAVER EN NY COMMENT PÅ DET POST VARIABEL VI HAR FUNDET & TILFØJER DEN VIA POST-ID..
            post.Comments.Add(new Comment(content, downvotes, upvotes, numberOfVotes, user, CommentTime));
            db.SaveChanges();
            return "Comment created";
        }
        // PUT VOTES
        // SÆTTER UPVOTES / DOWNVOTES OP OG DET TOTALE ANTAL VOTES OP.
        public string PostUpvote(int id)
        {
            var post = db.Posts.FirstOrDefault(p => p.PostId == id);
            post.Upvotes++;
            post.NumberOfVotes++;
            db.SaveChanges();
            return "Post Upvoted";
        }
        public string PostDownvote(int id)
        {
            var post = db.Posts.FirstOrDefault(p => p.PostId == id);
            post.Downvotes++;
            post.NumberOfVotes++;
            db.SaveChanges();
            return "Post Downvoted";
        }
        public string CommentUpvote(int id)
        {
            var comment = db.Comments.FirstOrDefault(c => c.CommentId == id);
            comment.Upvotes++;
            comment.NumberOfVotes++;
            db.SaveChanges();
            return "Comment Upvoted";
        }
        public string CommentDownvote(int id)
        {
            var comment = db.Comments.FirstOrDefault(c => c.CommentId == id);
            comment.Downvotes++;
            comment.NumberOfVotes++;
            db.SaveChanges();
            return "Comment Downvoted";
        }
        

        // SEED DATA TIL AT OPRETTE USER, POST OG COMMENT
        public void SeedData()
        {
            //TILFØJER 2 USERS
            User user1 = new User { Username = "M-L" };
            User user2 = new User("Magnus");
            //TILFØJER USEREN "M-L" HVIS USEREN IKKE FINDES I DATABASEN
            if (db.Users.FirstOrDefault(u => u.Username == user1.Username) == null)
            {
                db.Add(user1);
                db.SaveChanges();
            }
            //Jeg vil også være med!! - Magnus :)
            if (db.Users.FirstOrDefault(u => u.Username == user2.Username) == null)
            {
                    db.Add(user2);
                    db.SaveChanges();
            }
            //LAVET ET POST
            Post post = new Post
            {
                Title = "Sikke dejligt vejr",
                User = user1, // M-L
                Content = "Hvad tænker I?",
                Downvotes = 0,
                Upvotes = 10,
                NumberOfVotes = 10,
                PostTime = DateTime.Now
            };
            //TILFØJER POSTET MED TITLEN "Sikke dejligt vejr" HVIS DEN IKKE FINDES I DATABASEN
            if (db.Posts.FirstOrDefault(p => p.Title == post.Title) == null)
            {
                db.Add(post);
                db.SaveChanges();
            }
            //LAVER EN COMMENT
            Comment comment = new Comment
            {
                Content = "Vi blæser væk",
                Downvotes = 4,
                Upvotes = 5,
                NumberOfVotes = 9,
                User = user2,
                CommentTime = DateTime.Now 
            };
            //TILFØJER COMMENTEN MED CONTENT "Vi blæser væK" HVIS DEN IKKE FINDES I DATABASEN
            if (db.Comments.FirstOrDefault(c => c.Content == comment.Content) == null)
            {
                //TILFØJER COMMENT IGENNEM DB.POSTS
                post.Comments.Add(comment);
                db.SaveChanges();
            }
        }
    }
}

