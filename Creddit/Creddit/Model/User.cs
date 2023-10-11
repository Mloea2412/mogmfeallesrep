using System;
namespace Creddit.Model
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }

        public User(string name)
        {
            this.Name = name;
        }
    }
}

