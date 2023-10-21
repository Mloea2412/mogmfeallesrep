using System;
namespace MmReddit.Model
{
    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public User User { get; set; }
        public string Content { get; set; }
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }
        public int NumberOfVotes { get; set; }
        public DateTime PostTime { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();



        public Post(string title, User user, string content, int upvotes, int downvotes, int numberOfVotes, DateTime postTime)
        {
            Title = title;
            User = user;
            Content = content;
            Upvotes = upvotes;
            Downvotes = downvotes;
            NumberOfVotes = numberOfVotes;
            PostTime = postTime;
        }

        public Post()
        {
            PostId = 0;
            Title = "";
            User = null;
            Content = "";
            Upvotes = 0;
            Downvotes = 0;
            PostTime = DateTime.Now;
        }

    }
}

