using Microsoft.EntityFrameworkCore;
using System.Text.Json;

using shared.Model;
using static shared.Util;
using Data;

namespace Service;

public class DataService
{
    private OrdinationContext db { get; }

    public DataService(OrdinationContext db) {
        this.db = db;
    }

    /// <summary>
    /// Seeder noget nyt data i databasen, hvis det er nødvendigt.
    /// </summary>
    public void SeedData() {

        // Patients
        Patient[] patients = new Patient[5];
        patients[0] = db.Patienter.FirstOrDefault()!;

        if (patients[0] == null)
        {
            patients[0] = new Patient("121256-0512", "Jane Jensen", 63.4);
            patients[1] = new Patient("070985-1153", "Finn Madsen", 83.2);
            patients[2] = new Patient("050972-1233", "Hans Jørgensen", 89.4);
            patients[3] = new Patient("011064-1522", "Ulla Nielsen", 59.9);
            patients[4] = new Patient("123456-1234", "Ib Hansen", 87.7);

            db.Patienter.Add(patients[0]);
            db.Patienter.Add(patients[1]);
            db.Patienter.Add(patients[2]);
            db.Patienter.Add(patients[3]);
            db.Patienter.Add(patients[4]);
            db.SaveChanges();
        }

        Laegemiddel[] laegemiddler = new Laegemiddel[5];
        laegemiddler[0] = db.Laegemiddler.FirstOrDefault()!;
        if (laegemiddler[0] == null)
        {
            laegemiddler[0] = new Laegemiddel("Acetylsalicylsyre", 0.1, 0.15, 0.16, "Styk");
            laegemiddler[1] = new Laegemiddel("Paracetamol", 1, 1.5, 2, "Ml");
            laegemiddler[2] = new Laegemiddel("Fucidin", 0.025, 0.025, 0.025, "Styk");
            laegemiddler[3] = new Laegemiddel("Methotrexat", 0.01, 0.015, 0.02, "Styk");
            laegemiddler[4] = new Laegemiddel("Prednisolon", 0.1, 0.15, 0.2, "Styk");

            db.Laegemiddler.Add(laegemiddler[0]);
            db.Laegemiddler.Add(laegemiddler[1]);
            db.Laegemiddler.Add(laegemiddler[2]);
            db.Laegemiddler.Add(laegemiddler[3]);
            db.Laegemiddler.Add(laegemiddler[4]);

            db.SaveChanges();
        }

        Ordination[] ordinationer = new Ordination[6];
        ordinationer[0] = db.Ordinationer.FirstOrDefault()!;
        if (ordinationer[0] == null) {
            Laegemiddel[] lm = db.Laegemiddler.ToArray();
            Patient[] p = db.Patienter.ToArray();

            ordinationer[0] = new PN(new DateTime(2023, 1, 1), new DateTime(2023, 1, 12), 123, lm[1]);    
            ordinationer[1] = new PN(new DateTime(2023, 2, 12), new DateTime(2023, 2, 14), 3, lm[0]);    
            ordinationer[2] = new PN(new DateTime(2023, 1, 20), new DateTime(2023, 1, 25), 5, lm[2]);    
            ordinationer[3] = new PN(new DateTime(2023, 1, 1), new DateTime(2023, 1, 12), 123, lm[1]);
            ordinationer[4] = new DagligFast(new DateTime(2023, 1, 10), new DateTime(2023, 1, 12), lm[1], 2, 0, 1, 0);
            ordinationer[5] = new DagligSkæv(new DateTime(2023, 1, 23), new DateTime(2023, 1, 24), lm[2]);
            
            ((DagligSkæv) ordinationer[5]).doser = new Dosis[] { 
                new Dosis(CreateTimeOnly(12, 0, 0), 0.5),
                new Dosis(CreateTimeOnly(12, 40, 0), 1),
                new Dosis(CreateTimeOnly(16, 0, 0), 2.5),
                new Dosis(CreateTimeOnly(18, 45, 0), 3)        
            }.ToList();
            

            db.Ordinationer.Add(ordinationer[0]);
            db.Ordinationer.Add(ordinationer[1]);
            db.Ordinationer.Add(ordinationer[2]);
            db.Ordinationer.Add(ordinationer[3]);
            db.Ordinationer.Add(ordinationer[4]);
            db.Ordinationer.Add(ordinationer[5]);

            db.SaveChanges();

            p[0].ordinationer.Add(ordinationer[0]);
            p[0].ordinationer.Add(ordinationer[1]);
            p[2].ordinationer.Add(ordinationer[2]);
            p[3].ordinationer.Add(ordinationer[3]);
            p[1].ordinationer.Add(ordinationer[4]);
            p[1].ordinationer.Add(ordinationer[5]);

            db.SaveChanges();
        }
    }

    
    public List<PN> GetPNs() {
        return db.PNs.Include(o => o.laegemiddel).Include(o => o.dates).ToList();
    }

    public List<DagligFast> GetDagligFaste() {
        return db.DagligFaste
            .Include(o => o.laegemiddel)
            .Include(o => o.MorgenDosis)
            .Include(o => o.MiddagDosis)
            .Include(o => o.AftenDosis)            
            .Include(o => o.NatDosis)            
            .ToList();
    }

    public List<DagligSkæv> GetDagligSkæve() {
        return db.DagligSkæve
            .Include(o => o.laegemiddel)
            .Include(o => o.doser)
            .ToList();
    }

    public List<Patient> GetPatienter() {
        return db.Patienter.Include(p => p.ordinationer).ToList();
    }

    public List<Laegemiddel> GetLaegemidler() {
        return db.Laegemiddler.ToList();
    }


    // Metode til at oprette en PN (patientordination)
    public PN OpretPN(int patientId, int laegemiddelId, double antal, DateTime startDato, DateTime slutDato)
    {
        // Tjekker om slutDato er større end eller lig med startDato og om startDato er større end eller lig med dagens dato
        if (slutDato >= startDato && startDato >= DateTime.Today)
        {
            // Tjekker om antal er større eller lig med 0
            if (antal >= 0)
            {
                // Finder patienten i databasen baseret på patientId
                Patient patient = db.Patienter.FirstOrDefault(p => p.PatientId == patientId);

                // Hvis patienten ikke findes, kastes en fejl med en passende meddelelse og navnet på parameteren (patientId)
                if (patient == null)
                {
                    throw new ArgumentNullException("Patienten med det angivne ID eksisterer ikke.", nameof(patientId));
                }

                // Finder lægemidlet i databasen baseret på lægemiddelId
                Laegemiddel laegemiddel = db.Laegemiddler.FirstOrDefault(l => l.LaegemiddelId == laegemiddelId);

                // Hvis lægemidlet ikke findes, kastes en fejl med en passende meddelelse og navnet på parameteren (laegemiddelId)
                if (laegemiddel == null)
                {
                    throw new ArgumentNullException("Lægemidlet med det angivne ID eksisterer ikke.", nameof(laegemiddelId));
                }

                // Opretter en ny PN (patientordination) med de angivne parametre
                PN pn = new PN(startDato, slutDato, antal, laegemiddel);

                // Tilføjer den nye PN til databasen
                db.PNs.Add(pn);

                // Tilføjer den nye PN til patientens liste af ordinationer
                patient.ordinationer.Add(pn);

                // Gemmer ændringerne i databasen
                db.SaveChanges();

                // Returnerer den oprettede PN
                return pn;
            }
            else
            {
                // Kaster en fejl, hvis antallet ikke er positivt
                throw new ArgumentOutOfRangeException("Antal skal være positivt.", nameof(antal));
            }
        }
        else
        {
            // Kaster en fejl, hvis slutDato er før startDato eller startDato er i fortiden
            throw new ArgumentNullException("SlutDato må ikke være før StartDato & StartDato må ikke være i fortiden", nameof(slutDato));
        }
    }


    // Metode til at oprette en DagligFast ordination
    public DagligFast OpretDagligFast(int patientId, int laegemiddelId,
            double antalMorgen, double antalMiddag, double antalAften, double antalNat,
            DateTime startDato, DateTime slutDato)
    {
        // Tjekker om slutDato er større end eller lig med startDato og om startDato er større end eller lig med dagens dato
        if (slutDato >= startDato && startDato >= DateTime.Today)
        {
            // Tjekker om alle antal-værdier er større eller lig med 0
            if (antalMorgen >= 0 && antalMiddag >= 0 && antalAften >= 0 && antalNat >= 0)
            {
                // Finder patienten i databasen baseret på patientId
                Patient patient = db.Patienter.FirstOrDefault(p => p.PatientId == patientId);
                // Hvis patienten ikke findes, kastes en fejl med en passende meddelelse og navnet på parameteren (patientId)
                if (patient == null)
                {
                    throw new ArgumentNullException("Patienten med det angivne ID eksisterer ikke.", nameof(patientId));
                }

                // Finder lægemidlet i databasen baseret på lægemiddelId
                Laegemiddel laegemiddel = db.Laegemiddler.FirstOrDefault(l => l.LaegemiddelId == laegemiddelId);
                // Hvis lægemidlet ikke findes, kastes en fejl med en passende meddelelse og navnet på parameteren (laegemiddelId)
                if (laegemiddel == null)
                {
                    throw new ArgumentNullException("Lægemidlet med det angivne ID eksisterer ikke.", nameof(laegemiddelId));
                }

                // Opretter en ny DagligFast ordination med de angivne parametre
                DagligFast dagligFast = new DagligFast(startDato, slutDato, laegemiddel, antalMorgen, antalMiddag, antalAften, antalNat);

                // Tilføjer den nye DagligFast ordination til databasen
                db.Ordinationer.Add(dagligFast);

                // Gemmer ændringerne i databasen
                db.SaveChanges();

                // Finder den nyligt oprettede ordination i databasen
                int temp = dagligFast.OrdinationId;
                Ordination ordination = db.Ordinationer.Find(temp)!;

                // Tilføjer den nyligt oprettede ordination til patientens liste af ordinationer
                patient.ordinationer.Add(ordination);

                // Gemmer ændringerne i databasen
                db.SaveChanges();

                // Returnerer den oprettede DagligFast ordination
                return null!;
            }
            else
            {
                // Kaster en fejl, hvis nogen af antal-værdierne ikke er positive
                throw new ArgumentNullException("Alle skal være positive");
            }
        }
        else
        {
            // Kaster en fejl, hvis slutDato er før startDato eller startDato er i fortiden
            throw new ArgumentNullException("SlutDato må ikke være før StartDato & StartDato må ikke være i fortiden", nameof(slutDato));
        }
    }


    // Metode til at oprette en DagligSkæv ordination
    public DagligSkæv OpretDagligSkaev(int patientId, int laegemiddelId, Dosis[] doser, DateTime startDato, DateTime slutDato)
    {
        // Tjekker om slutDato er større end eller lig med startDato og om startDato er større end eller lig med dagens dato
        if (slutDato >= startDato && startDato >= DateTime.Today)
        {
            // Tjekker om doser-arrayet er tomt
            if (doser.Length == 0)
            {
                // Kaster en fejl, hvis doser er tomt
                throw new ArgumentNullException(nameof(doser), "Doser-arrayet må ikke være tomt");
            }

            // Itererer gennem hver Dosis i doser-arrayet
            foreach (Dosis dosis in doser)
            {
                // Tjekker om antallet i hver dosis er mindre end 0
                if (dosis.antal < 0)
                {
                    // Kaster en fejl, hvis antallet ikke er positivt
                    throw new ArgumentNullException("Antallet skal have en positiv værdi");
                }
            }

            // Finder patienten i databasen baseret på patientId
            Patient patient = db.Patienter.FirstOrDefault(p => p.PatientId == patientId);

            // Hvis patienten ikke findes, kastes en fejl med en passende meddelelse og navnet på parameteren (patientId)
            if (patient == null)
            {
                throw new ArgumentNullException("Patienten med det angivne ID eksisterer ikke.", nameof(patientId));
            }

            // Finder lægemidlet i databasen baseret på lægemiddelId
            Laegemiddel laegemiddel = db.Laegemiddler.FirstOrDefault(l => l.LaegemiddelId == laegemiddelId);

            // Hvis lægemidlet ikke findes, kastes en fejl med en passende meddelelse og navnet på parameteren (laegemiddelId)
            if (laegemiddel == null)
            {
                throw new ArgumentNullException("Lægemidlet med det angivne ID eksisterer ikke.", nameof(laegemiddelId));
            }

            // Opretter en ny DagligSkæv ordination med de angivne parametre
            DagligSkæv skæv = new DagligSkæv(startDato, slutDato, laegemiddel, doser);

            // Tilføjer den nye DagligSkæv ordination til databasen
            db.DagligSkæve.Add(skæv);

            // Tilføjer den nye DagligSkæv ordination til patientens liste af ordinationer
            patient.ordinationer.Add(skæv);

            // Gemmer ændringerne i databasen
            db.SaveChanges();

            // Returnerer den oprettede DagligSkæv ordination
            return skæv;
        }
        else
        {
            // Kaster en fejl, hvis slutDato er før startDato eller startDato er i fortiden
            throw new ArgumentNullException("SlutDato må ikke være før StartDato & StartDato må ikke være i fortiden", nameof(slutDato));
        }
    }



    // Metode til at anvende en PN (patientordination) på en given dato
    public string AnvendOrdination(int id, Dato dato)
    {
        // Finder den ønskede PN (patientordination) i databasen baseret på id
        PN pn = db.PNs.Find(id);

        // Kalder givDosis-metoden på PN-objektet for at forsøge at anvende dosis på den givne dato
        bool anvendtOrdination = pn.givDosis(dato);

        // Tjekker om dosis blev anvendt
        if (anvendtOrdination)
        {
            // Gemmer ændringerne i databasen, da dosis blev anvendt
            db.SaveChanges();

            // Returnerer en bekræftelsesmeddelelse
            return ("Dosis er anvendt.");
        }
        else
        {
            // Returnerer en fejlmeddelelse, hvis dosis ikke blev anvendt
            return ("Ordination ikke anvendt");
        }
    }

    /// <summary>
    /// Den anbefalede dosis for den pågældende patient, per døgn, hvor der skal tages hensyn til
    /// patientens vægt. Enheden afhænger af lægemidlet. Patient og lægemiddel må ikke være null.
    /// </summary>
    /// <param name="patient"></param>
    /// <param name="laegemiddel"></param>
    /// <returns></returns>
    // Metode til at hente den anbefalede dosis per døgn for en given patient og et lægemiddel
    public double GetAnbefaletDosisPerDøgn(int patientId, int laegemiddelId)
    {
        // Finder patienten i databasen baseret på patientId
        Patient patient = db.Patienter.FirstOrDefault(p => p.PatientId == patientId);

        // Kaster en fejl, hvis patienten ikke findes
        if (patient == null)
        {
            throw new ArgumentNullException("Patienten med det angivne ID eksisterer ikke.", nameof(patientId));
        }

        // Finder lægemidlet i databasen baseret på lægemiddelId
        Laegemiddel laegemiddel = db.Laegemiddler.FirstOrDefault(l => l.LaegemiddelId == laegemiddelId);

        // Kaster en fejl, hvis lægemidlet ikke findes
        if (laegemiddel == null)
        {
            throw new ArgumentNullException("Lægemidlet med det angivne ID eksisterer ikke.", nameof(laegemiddelId));
        }

        // Initialiserer Anbefalet Dosis variablen til -1 (standardværdi, hvis beregning fejler)
        double AnDosis = -1;

        // Tjekker om patienten, lægemidlet og patientens vægt er gyldige
        if (patient != null && laegemiddel != null && patient.vaegt > 0)
        {
            // Beregner den anbefalede dosis per døgn baseret på patientens vægt og lægemidlets enhedPrKgPrDoegnLet
            AnDosis = patient.vaegt * laegemiddel.enhedPrKgPrDoegnLet;
        }
        else if (patient.vaegt <= 0)
        {
            // Kaster en fejl, hvis patientens vægt er mindre end eller lig med 0
            throw new ArgumentOutOfRangeException("Vægt skal gives som en positiv værdi");
        }

        // Returnerer den beregnede anbefalede dosis per døgn
        return AnDosis;
    }

}