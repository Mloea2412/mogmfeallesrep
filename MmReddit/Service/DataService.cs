using System;
using MmReddit.Model;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Hosting;

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
            return db.Posts.Include(p => p.Comments).ThenInclude(u => u.User).Include(p => p.User).ToList();
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
        // CREATECOMMENT - OPRETTER EN NY KOMMENTAR
        public string CreateComment(string content, int upvotes, int downvotes, int numberOfVotes, int postid, User user, DateTime CommentTime)
        {
            Post post = db.Posts.FirstOrDefault(p => p.PostId == postid);
            if (post == null)
            {
                return "Post not found";
            }

            if (db == null)
            {
                return "Database not initialized";
            }

            post.Comments.Add(new Comment(content, downvotes, upvotes, numberOfVotes, user, CommentTime));
            db.SaveChanges();
            return "Comment created";
        }

        public bool PostVoting (int postid, bool UpvoteOrDownvote)
        {
            var post = db.Posts.FirstOrDefault(p => p.PostId == postid);
            if (post == null)
            {
                return false;
            }

            if (UpvoteOrDownvote == true)
            {
                post.Upvotes++;
                post.NumberOfVotes++;
                db.SaveChanges();

                return true;
            }
            else if (UpvoteOrDownvote == false)
            {
                post.Downvotes++;
                post.NumberOfVotes++;
                db.SaveChanges();
                return false;
            } else
            {
                return false;
            }
        }


        public bool CommentVoting(int commentid, bool UpvoteOrDownvote)
        {
            var comment = db.Comments.FirstOrDefault(c => c.CommentId == commentid);
            {
                if (comment == null)
                {
                    return false;
                }

                if (UpvoteOrDownvote == true)
                {
                    comment.Upvotes++;
                    comment.NumberOfVotes++;
                    db.SaveChanges();
                    return true;
                }

                else if (UpvoteOrDownvote == false)
                {
                    comment.Downvotes++;
                    comment.NumberOfVotes++;
                    db.SaveChanges();
                    return false;
                }
                else
                {
                    return false;
                }
            }
        }


        // SEED DATA TIL AT OPRETTE USER, POST OG COMMENT
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

