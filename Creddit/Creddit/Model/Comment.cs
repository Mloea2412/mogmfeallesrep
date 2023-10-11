using System;
namespace Creddit.Model
{
    public class Comment
    {
        public int CommentId { get; set; }
        public User User { get; set; }
        public string Text { get; set; }
        public int Downvote { get; set; }
        public int Upvote { get; set; }
        public int NumberOfVotes { get; set; }
        public DateTime CommentTime { get; set; }

        public Comment(string text, User user, int downvote, int upvote, int numberOfVotes, DateTime commentTime)
        {
            this.User = user;
            this.Text = text;
            this.Downvote = downvote;
            this.Upvote = upvote;
            this.NumberOfVotes = numberOfVotes;
            this.CommentTime = commentTime;
        }

        public Comment()
        {
            CommentId = 0;
            Text = "";
            Downvote = 0;
            Upvote = 0;
            NumberOfVotes = 0;
            CommentTime = DateTime.Now;
        }
    }
}