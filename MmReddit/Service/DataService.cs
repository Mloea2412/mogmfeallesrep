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

        public void SeedData()
        {
            Post post = db.Posts.FirstOrDefault()!;
            if (post == null)
            {
                User user1 = new User("M-L");
                post = new Post { PostId = 1, Title = "Dette projekt er op ad pakke?", User = user1, Content = "Det er vildt på ad bakke?", Downvotes = 0, Upvotes = 10};
                db.Add(post);
                db.SaveChanges();
                //db.Posts.Add(new Post(1, "Basement åbningstider?",user1, "Hvornår har basement åben?", 0, 10, 10, DateTime.Now));
            }
        }

            /*//henter post og returner dem som en liste
            public List<Comment> Comments()
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
            }*/
        } }


        /*// Udkast til post post
        public string CreatePost(string title, User user, string text, int upvote, int downvote, int numberOfVotes, DateTime postTime)
        {

            User tempuser = db.Users.FirstOrDefault(a => a.UserId == user.UserId)!;
            if (tempuser == null)
            {
                //db.Users.Add()
                db.Posts.Add(new Post(user, title, content, upvote, downvote, numberOfVotes, DateTime.Now));
            }
            else
            {
                db.Posts.Add(new Post(title, tempuser, text, upvote, downvote, numberOfVotes, DateTime.Now));
            }
            db.SaveChanges();
            return "Post created";

        }*/

        /*

        // Gamle version
        public string CreateComment(string text, int upvote, int downvote, int numberOfVotes, int postid, User user, DateTime CommentTime)
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

            post.Comments.Add(new Comment(text, downvote, upvote, numberOfVotes, user, DateTime.Now));
            db.SaveChanges();
            return "Comment created";
        }

        */

        // Version 2.0
        /* public string CreateComment(string text, int upvote, int downvote, int numberOfVotes, int postid) {

         User tempuser = db.Users.FirstOrDefault(a => a.UserId == user.UserId)!;
         if(tempuser==null){
             //db.Users.Add()
             db.Comments.Add(new Comment(text, upvote, downvote, numberOfVotes));
         }
         else{
             db.Comments.Add(new Comment(text, upvote, downvote, numberOfVotes));
         }
         db.SaveChanges();
         return "Comment created";

         }*/


        /*public bool PostVoting(int postId, User user, bool UpvoteOrDownvote)
        {
            {
                // Find indlægget med det givne postId
                var post = db.Posts.FirstOrDefault(p => p.PostId == postId);
                if (post == null)
                {

                    return false;
                }

                // Hvis UpvoteOrDownvote er sat som true upvote
                // mulig implementering af at add en user til en liste, så personen ikke kan vote flere gange 

                if (UpvoteOrDownvote == true)
                {
                    post.Upvote++;
                    post.NumberOfVotes++;
                    // post.UserVotes.Add(tempUser);
                    db.SaveChanges();

                    return true;

                    // Hvis UpvoteOrDownvote er sat som false downvote
                    // mulig implementering af at fjerne en user fra en liste, 
                }
                else if (UpvoteOrDownvote == false)
                {
                    post.Downvote--;
                    post.NumberOfVotes++;

                    //post.UserVotes.Remove(tempUser);
                    db.SaveChanges();
                    return false;
                }
                else
                {
                    return false;
                }
            }
        }

*//*

        public bool CommentVoting(int commentId, User user, bool UpvoteOrDownvote)
        {
            {
                // Find indlægget med det givne postId
                var comment = db.Comments.FirstOrDefault(p => p.CommentId == commentId);
                if (comment == null)
                {

                    return false;
                }

                // Hvis UpvoteOrDownvote er sat som true upvote
                // mulig implementering af at add en user til en liste, så personen ikke kan vote flere gange 

                if (UpvoteOrDownvote == true)
                {
                    comment.Upvote++;
                    comment.NumberOfVotes++;
                    // post.UserVotes.Add(tempUser);
                    db.SaveChanges();

                    return true;

                    // Hvis UpvoteOrDownvote er sat som false downvote
                    // mulig implementering af at fjerne en user fra en liste, 
                }
                else if (UpvoteOrDownvote == false)
                {
                    comment.Downvote--;
                    comment.NumberOfVotes++;

                    //post.UserVotes.Remove(tempUser);
                    db.SaveChanges();
                    return false;
                }
                else
                {
                    return false;
                }
            }
        }
        public void SeedData()
        {
            Post post = db.Posts.FirstOrDefault()!;
            if (post == null)
            {
                User user1 = new User("Boes");
                post = new Post { PostId = 1, Title = "Basement åbningstider?", User = user1, Text = "Hvornår har basement åben?", Downvote = 0, Upvote = 10, NumberOfVotes = 10 };
                db.Add(post);
                db.SaveChanges();
                //db.Posts.Add(new Post(1, "Basement åbningstider?",user1, "Hvornår har basement åben?", 0, 10, 10, DateTime.Now));
            }
            Comment comment = db.Comments.FirstOrDefault()!;
            if (comment == null)
            {
                comment = new Comment { CommentId = 1, Text = "Den har åben Torsdag og fredag fra kl 12", Downvote = 4, Upvote = 5, NumberOfVotes = 9 };
                db.Add(comment);
                db.SaveChanges();
            }
            User user = db.Users.FirstOrDefault()!;
            if (user == null)
            {
                User user2 = new User("Mads");
                User user3 = new User("ML");
                User user4 = new User("Rasmus");
                db.Add(user2);
                db.Add(user3);
                db.Add(user4);
                db.SaveChanges();
            }
        }
    }
}

*/