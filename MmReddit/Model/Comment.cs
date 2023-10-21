using System;
namespace MmReddit.Model
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string Content { get; set; }
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }
        public int NumberOfVotes { get; set; }
        public User User { get; set; }
        public DateTime CommentTime { get; set; }

        public Comment(string content, int downvotes, int upvotes, int numberOfVotes, User user, DateTime commentTime)
        {
            Content = content;
            Upvotes = upvotes;
            Downvotes = downvotes;
            NumberOfVotes = numberOfVotes;
            User = user;
            CommentTime = commentTime;
        }
        public Comment()
        {
            CommentId = 0;
            Content = "";
            Upvotes = 0;
            Downvotes = 0;
            NumberOfVotes = 0;
        }
    }
}

