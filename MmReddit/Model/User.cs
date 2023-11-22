using System;

namespace MmReddit.Model
{
    public class User
    {
        // Egenskaber (Properties) for brugeren
        public int UserId { get; set; } // Unikt ID for brugeren
        public string Username { get; set; } // Brugernavnet

        // Konstruktør til at oprette en ny bruger med et brugernavn
        public User(string username = "")
        {
            // Initialiserer brugernavnet med det angivne eller tomt brugernavn
            Username = username;
        }

        // Standardkonstruktør (uden parametre)
        public User()
        {
            UserId = 0; // Initialiserer UserId til 0 (typisk brugt for nye, ikke-gemte brugere)
            Username = ""; // Initialiserer Username til en tom streng
        }
    }
}
