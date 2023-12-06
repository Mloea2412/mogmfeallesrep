namespace ordination_test;

using Microsoft.EntityFrameworkCore;

using Service;
using Data;
using shared.Model;
using shared;
using static shared.Util;


[TestClass]
public class ServiceTest
{
    private DataService service;

    [TestInitialize]
    // Dette er en attribut, der markerer metoden som en initialiseringsmetode, der skal køres før hver testmetode.
    public void SetupBeforeEachTest()
    {
        // Opretter en instans af DbContextOptionsBuilder for din OrdinationContext.
        var optionsBuilder = new DbContextOptionsBuilder<OrdinationContext>();

        // Konfigurer optionsBuilder til at bruge en in-memory database med et specifikt navn for test-databasen.
        optionsBuilder.UseInMemoryDatabase(databaseName: "test-database");

        // Opretter en ny instans af OrdinationContext ved at bruge de konfigurerede options og optionsBuilder.
        var context = new OrdinationContext(optionsBuilder.Options);

        // Opretter en ny instans af DataService og tildel den til testens service-variabel.
        service = new DataService(context);

        // Kalder SeedData-metoden på DataService-instansen for at udfylde in-memory-databasen med testdata.
        service.SeedData();
    }

    [TestMethod]
    // Dette er en attribut, der markerer metoden som en testmetode.
    public void PatientsExist()
    {
        // Tjekker, om der er nogen patienter i testtjenesten. Forventningen er, at der er mindst én, da SeedData-metoden kaldes under initialiseringen.
        Assert.IsNotNull(service.GetPatienter());
    }

    // DER SKAL LAVES TESTCASE NUMMER, SÅ DET KAN DIREKTE HENFØRES TIL VORES DOKUMENTATION


    [TestMethod]
    // Dette er en attribut, der markerer metoden som en testmetode.
    public void OpretDagligFast()
    {
        // Hent den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Tjekker, om der er ét daglig fast objekt i tjenesten. Forventningen er, at der er ét, da testen starter.
        Assert.AreEqual(1, service.GetDagligFaste().Count());

        // Kald metoden for at oprette et nyt dagligt fast objekt for patienten og lægemidlet med de specificerede parametre:
        // - Patientens ID
        // - Lægemidlets ID
        // - Antal doser pr. dag (2)
        // - Antal piller pr. dosis (2)
        // - Intervallet mellem doser (1 time)
        // - Antal dage doseringen skal vare (0, da det er en fast dosering)
        // - Startdato (dagens dato)
        // - Slutdato (dagens dato + 3 dage)
        service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId, 2, 2, 1, 0, DateTime.Now, DateTime.Now.AddDays(3));

        // Tjekker, om der nu er to daglige faste objekter i tjenesten. Forventningen er, at antallet er steget fra 1 til 2.
        Assert.AreEqual(2, service.GetDagligFaste().Count());
    }


    [TestMethod]
    // Dette er en attribut, der markerer metoden som en testmetode.
    public void OpretDagligSkaev()
    {
        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Tjekker, om der er ét daglig skævt objekt i tjenesten. Forventningen er, at der er ét, da testen starter.
        Assert.AreEqual(1, service.GetDagligSkæve().Count());

        // Opretter en liste af doser med en enkelt dosis, der skal gives kl. 12:00 med en mængde på 2.
        Dosis[] doser = new Dosis[] { new Dosis(CreateTimeOnly(4, 0, 0), 6) };

        // Kalder metoden for at oprette et nyt dagligt skævt objekt for patienten og lægemidlet med de specificerede doser.
        // Angiver start- og slutdato for doseringen som dagens dato og to dage frem.
        service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId, doser, DateTime.Now, DateTime.Now.AddDays(2));

        // Tjekker, om der nu er to daglige skæve objekter i tjenesten. Forventningen er, at antallet er steget fra 1 til 2.
        Assert.AreEqual(2, service.GetDagligSkæve().Count());
    }


  

    // TEST AF PN

    [TestMethod] // Testmetode for at oprette en PN.
    public void OpretPN()
    {
        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Tjekker, om der er to PN-objekter i tjenesten. Forventningen er, at der er to, da testen starter.
        Assert.AreEqual(4, service.GetPNs().Count());

        // Opretter en PN med patientens ID, lægemidlets ID, en mængde på 3 og en doseringsperiode fra i dag til to dage frem.
        service.OpretPN(patient.PatientId, lm.LaegemiddelId, 3, DateTime.Now, DateTime.Now.AddDays(2));

        // Tjekker, om der stadig er to PN-objekter i tjenesten. Forventningen er, at antallet forbliver uændret, da testen ikke forventer at tilføje et nyt objekt.
        Assert.AreEqual(5, service.GetPNs().Count());
    }



    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OpretPNPatientIdeEksistererikke()
    {
        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Forsøger at oprette en PN med en ikke-eksisterende patient (PatientId 20).
        
            service.OpretPN(20, lm.LaegemiddelId, 3, DateTime.Now, DateTime.Now.AddDays(2));
        
        Console.WriteLine("Patienten med det angivne ID eksisterer ikke.");
    }



    // TEST AF DAGLIG FAST

    [TestMethod] // Testmetode for at validere, at en ArgumentNullException kastes ved forsøg på at oprette en daglig fast dosis med en ikke-eksisterende patient (PatientId 33).
    [ExpectedException(typeof(ArgumentNullException))]
    public void OpretDagligFastPatientEksistererIkke()
    {
        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Forsøger at oprette en daglig fast dosis med en ikke-eksisterende patient (PatientId 33).
        // Forventningen er, at en ArgumentNullException kastes.
        service.OpretDagligFast(33, lm.LaegemiddelId, 8, 2, 2, 6, DateTime.Now, DateTime.Now.AddDays(2));

        // Udskriver en besked til konsollen for at bekræfte, at oprettelsen af daglig fast dosis fejlede korrekt.
        Console.WriteLine("Oprettelse af daglig fast er korrekt fejlet, da patientId ikke eksisterer.");
    }




    // TEST AF DAGLIG SKÆV


    [TestMethod] // Testmetode for at validere, at en ArgumentNullException kastes ved forsøg på at oprette en daglig skæv dosis med ugyldig PatientId (-2).
    [ExpectedException(typeof(ArgumentNullException))]
    public void OpretDagligSkævUgyldigPatientId()
    {
        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Opretter en dosis med et klokkeslæt på 04:00 og en mængde på 6.
        Dosis[] doser = new Dosis[] { new Dosis(CreateTimeOnly(4, 0, 0), 6) };

        // Forsøger at oprette en daglig skæv dosis med en ugyldig PatientId (-2). Forventningen er, at en InvalidCastException kastes.
        service.OpretDagligSkaev(-2, lm.LaegemiddelId, doser, DateTime.Now, DateTime.Now.AddDays(2));

        // Udskriver en besked til konsollen for at bekræfte, at oprettelsen af daglig skæv dosis fejlede korrekt.
        Console.WriteLine("Oprettelse af daglig skæv er fejlet korrekt, da patient id skal være et positivt integer.");
    }



    [TestMethod] // Testmetode for at validere, at en ArgumentNullException kastes ved forsøg på at oprette en daglig skæv dosis med en PatientId, der ikke eksisterer (15).
    [ExpectedException(typeof(ArgumentNullException))]
    public void OpretDagligSkævPatientIdEksistererIkke()
    {
        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Opretter en dosis med et klokkeslæt på 04:00 og en mængde på 6.
        Dosis[] doser = new Dosis[] { new Dosis(CreateTimeOnly(12, 0, 0), 4) };

        // Forsøger at oprette en daglig skæv dosis med en PatientId (15), der ikke eksisterer i databasen.
        // Forventningen er, at en InvalidCastException kastes.
        service.OpretDagligSkaev(15, lm.LaegemiddelId, doser, DateTime.Now, DateTime.Now.AddDays(2));

        // Udskriver en besked til konsollen for at bekræfte, at oprettelsen af daglig skæv dosis fejlede korrekt.
        Console.WriteLine("Oprettelse af daglig skæv er fejlet korrekt, da det pågældende patient id ikke eksisterer i databasen.");
    }



    [TestMethod] // Testmetode for at validere, at en ArgumentNullException kastes ved forsøg på at oprette en daglig skæv dosis med en LaegemiddelId, der ikke eksisterer (23).
    [ExpectedException(typeof(ArgumentNullException))]
    public void OpretDagligSkævLaegemiddelIdEksistererIkke()
    {
        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Opretter en dosis med et klokkeslæt på 04:00 og en mængde på 6.
        Dosis[] doser = new Dosis[] { new Dosis(CreateTimeOnly(12, 0, 0), 4) };

        // Forsøger at oprette en daglig skæv dosis med en LaegemiddelId (23), der ikke eksisterer i databasen.
        // Forventningen er, at en InvalidCastException kastes.
        service.OpretDagligSkaev(patient.PatientId, 23, doser, DateTime.Now, DateTime.Now.AddDays(2));

        // Udskriver en besked til konsollen for at bekræfte, at oprettelsen af daglig skæv dosis fejlede korrekt.
        Console.WriteLine("Oprettelse af daglig skæv er fejlet korrekt, da det pågældende lægemiddel id ikke eksisterer i databasen.");
    }










    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    
    public void TestAtKodenSmiderEnException()
    {
        // Koden herunder giver en exception fordi start datoen starter i feb og slut datoen er en måned før

        // Testen fejler hvis man indsætter en korrekte start- og slutdatoer.

        Patient patient = service.GetPatienter().First();
        Laegemiddel laegemiddel = service.GetLaegemidler().First();
        service.OpretDagligSkaev(patient.PatientId, laegemiddel.LaegemiddelId,
           new Dosis[] {
                new Dosis(Util.CreateTimeOnly(12, 0, 0), 0.5),
                new Dosis(Util.CreateTimeOnly(12, 40, 0), 1),
                new Dosis(Util.CreateTimeOnly(16, 0, 0), 2.5),
                new Dosis(Util.CreateTimeOnly(18, 45, 0), 3)

           }, new DateTime(2023, 2, 01), new DateTime(2023, 01, 01));
    }
}

