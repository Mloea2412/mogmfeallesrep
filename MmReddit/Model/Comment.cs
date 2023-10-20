﻿using System;
namespace MmReddit.Model
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string Content { get; set; }
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }
        public User User { get; set; }
        public Comment(string content = "", int upvotes = 0, int downvotes = 0, User user = null)
        {
            Content = content;
            Upvotes = upvotes;
            Downvotes = downvotes;
            User = user;
        }
        public Comment()
        {
            CommentId = 0;
            Content = "";
            Upvotes = 0;
            Downvotes = 0;
        }
    }
}

