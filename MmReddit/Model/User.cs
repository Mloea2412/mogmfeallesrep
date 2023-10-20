using System;
namespace MmReddit.Model
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public User(string username = "")
        {
            Username = username;
        }
        public User()
        {
            UserId = 0;
            Username = "";
        }
    }
}


