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
 
    // TEST AF OPRETPN
    [TestMethod] 
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


    // TEST AF PN

    [TestMethod]// TC2
    [ExpectedException(typeof(ArgumentNullException))]
    public void TC3OpretPNPatientIdUgyldigInt()
    {
        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Forsøger at oprette en PN med en ikke-eksisterende patient (PatientId 11).

        service.OpretPN(-2, lm.LaegemiddelId, 2, DateTime.Now, DateTime.Now.AddDays(2));

        Console.WriteLine("Oprettelse af PN er korrekt fejlet, patienten id skal være potisiv int.");
    }


    [TestMethod] // TC3
    [ExpectedException(typeof(ArgumentNullException))]
    public void TC3OpretPNPatientIdeEksistererikke()
    {
        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Forsøger at oprette en PN med en ikke-eksisterende patient (PatientId 11).
        
        service.OpretPN(11, lm.LaegemiddelId, 8, DateTime.Now, DateTime.Now.AddDays(2));
        
        Console.WriteLine("Oprettelse PN er fejlet korrekt, patientenId eksisterer ikke.");
    }


    [TestMethod]// TC4
    [ExpectedException(typeof(ArgumentNullException))]
    public void TC4OpretPNLaegemiddelIdUgyldigInt()
    {
        Patient patient = service.GetPatienter().First();
        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Forsøger at oprette en PN med en ikke-eksisterende patient (PatientId 11).
        service.OpretPN(patient.PatientId, -4, 8, DateTime.Now, DateTime.Now.AddDays(2));

        Console.WriteLine("Oprettelse af PN er korrekt fejlet, patientId skal være potisiv int.");
    }

    [TestMethod]// TC5
    [ExpectedException(typeof(ArgumentNullException))]
    public void TC5OpretPNLaegemiddelIdUgyldigInt()
    {
        Patient patient = service.GetPatienter().First();
        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Forsøger at oprette en PN med en ikke-eksisterende patient (PatientId 11).
        service.OpretPN(patient.PatientId, 11, 8, DateTime.Now, DateTime.Now.AddDays(2));

        Console.WriteLine("Oprettelse af PN er korrekt fejlet, patientId eksisterer ikke i databasen.");
    }

    [TestMethod]// TC6
    [ExpectedException(typeof(ArgumentNullException))]
    public void TC6OpretPNSlutDatoSkalVæreStørreEllerLigStartDato()
    {
        Patient patient = service.GetPatienter().First();
        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Forsøger at oprette en PN med en ikke-eksisterende patient (PatientId 11).
        service.OpretPN(3, 2, 8, DateTime.Now.AddDays(4), DateTime.Now);

        Console.WriteLine("Oprettelse af PN er korrekt fejlet, da slutDato skal være større eller lig startDato.");
    }

    /*// Start dato skal sættes til 2023,12,6 (som vil virke i morgen, idag med 2023,12,5)
    [TestMethod]// TC7
    [ExpectedException(typeof(ArgumentNullException))]
    public void TC6OpretPNStartDatoSkalVæreValid()
    {
        Patient patient = service.GetPatienter().First();
        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Forsøger at oprette en PN med en ikke-eksisterende patient (PatientId 11).
        service.OpretPN(3, 2, 8, DateTime.Now, DateTime.Now);

        Console.WriteLine("Oprettelse af PN er korrekt fejlet, da startDato skal være en valid dato");
    }*/

    // TEST AF DAGLIG FAST

    [TestMethod] //TC2
    [ExpectedException(typeof(ArgumentNullException))]
    public void TC2OpretDagligFastPatientIdNegativInt()
    {
        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Forsøger at oprette en daglig fast dosis med en ikke-eksisterende patient (PatientId 33).
        // Forventningen er, at en ArgumentNullException kastes.
        service.OpretDagligFast(-5, lm.LaegemiddelId, 4, 1, 2, 6, DateTime.Now, DateTime.Now.AddDays(2));

        // Udskriver en besked til konsollen for at bekræfte, at oprettelsen af daglig fast dosis fejlede korrekt.
        Console.WriteLine("Oprettelse af daglig fast er korrekt fejlet, da patientId skal være positiv int.");
    }

    // TEST CASE 3: PATIENTID ISN'T FOUND IN THE DATABASE (11)
    [TestMethod] // Testmetode for at validere, at en ArgumentNullException kastes ved forsøg på at oprette en daglig fast dosis med en ikke-eksisterende patient (PatientId 33).
    [ExpectedException(typeof(ArgumentNullException))]
    public void TC3OpretDagligFastPatientEksistererIkke()
    {
        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Forsøger at oprette en daglig fast dosis med en ikke-eksisterende patient (PatientId 33).
        // Forventningen er, at en ArgumentNullException kastes.
        service.OpretDagligFast(11, lm.LaegemiddelId, 4, 1, 2, 6, DateTime.Now, DateTime.Now.AddDays(4));

        // Udskriver en besked til konsollen for at bekræfte, at oprettelsen af daglig fast dosis fejlede korrekt.
        Console.WriteLine("Oprettelse af daglig fast er korrekt fejlet, da patientId ikke eksisterer.");
    }

    // TEST CASE 4: LAEGEMIDDELID IS NOT FOUND UN THE DATABASE (11)
    [TestMethod] // Testmetode for at validere, at en ArgumentNullException kastes ved forsøg på at oprette en daglig fast dosis med en ikke-eksisterende patient (PatientId 33).
    [ExpectedException(typeof(ArgumentNullException))]
    public void TC4OpretDagligFastLaegemiddelEksistererIkke()
    {
        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Forsøger at oprette en daglig fast dosis med en ikke-eksisterende patient (PatientId 33).
        // Forventningen er, at en ArgumentNullException kastes.
        service.OpretDagligFast(patient.PatientId, 11, 4, 1, 2, 6, DateTime.Now, DateTime.Now.AddDays(4));

        // Udskriver en besked til konsollen for at bekræfte, at oprettelsen af daglig fast dosis fejlede korrekt.
        Console.WriteLine("Oprettelse af daglig fast er korrekt fejlet, da laegemiddelId ikke eksisterer.");
    }

    // TEST CASE 5: LAEGEMIDDEL MUST BE A POSITIVE INT (-2)
    [TestMethod] // Testmetode for at validere, at en ArgumentNullException kastes ved forsøg på at oprette en daglig fast dosis med en ikke-eksisterende patient (PatientId 33).
    [ExpectedException(typeof(ArgumentNullException))]
    public void TC5OpretDagligFastLaegemiddelIdNegativInt()
    {
        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Forsøger at oprette en daglig fast dosis med en ikke-eksisterende patient (PatientId 33).
        // Forventningen er, at en ArgumentNullException kastes.
        service.OpretDagligFast(patient.PatientId, -2, 4, 1, 2, 6, DateTime.Now, DateTime.Now.AddDays(4));

        // Udskriver en besked til konsollen for at bekræfte, at oprettelsen af daglig fast dosis fejlede korrekt.
        Console.WriteLine("Oprettelse af daglig fast er korrekt fejlet, da laegemiddelId skal være posistiv int.");
    }

    // TEST CASE 6: ANTALMORGEN MUST BE A POSITIVE DOUBLE (-4)
    [TestMethod] // Testmetode for at validere, at en ArgumentNullException kastes ved forsøg på at oprette en daglig fast dosis med en ikke-eksisterende patient (PatientId 33).
    [ExpectedException(typeof(ArgumentNullException))]
    public void TC6OpretDagligFastAntalMorgenNegative()
    {
        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Forsøger at oprette en daglig fast dosis med en ikke-eksisterende patient (PatientId 33).
        // Forventningen er, at en ArgumentNullException kastes.
        service.OpretDagligFast(3, 2, -4, 1, 2, 6, DateTime.Now, DateTime.Now.AddDays(4));

        // Udskriver en besked til konsollen for at bekræfte, at oprettelsen af daglig fast dosis fejlede korrekt.
        Console.WriteLine("Oprettelse af daglig fast er korrekt fejlet, da antalMorgen skal være en posistiv double.");
    }


    // TEST CASE 7: ANTALMORGEN MUST BE A POSITIVE DOUBLE (-4)
    [TestMethod] // Testmetode for at validere, at en ArgumentNullException kastes ved forsøg på at oprette en daglig fast dosis med en ikke-eksisterende patient (PatientId 33).
    [ExpectedException(typeof(ArgumentNullException))]
    public void TC7OpretDagligFastAntalMiddagNegative()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        service.OpretDagligFast(3, 2, 4, -1, 2, 6, DateTime.Now, DateTime.Now.AddDays(4));
        Console.WriteLine("Oprettelse af daglig fast er korrekt fejlet, da antalMiddag skal være en positiv double.");
    }

    // TEST CASE 8: ANTALMORGEN MUST BE A POSITIVE DOUBLE (-4)
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TC8OpretDagligFastAntalMiddagNegative()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        service.OpretDagligFast(3, 2, 4, 1, -2, 6, DateTime.Now, DateTime.Now.AddDays(4));
        Console.WriteLine("Oprettelse af daglig fast er korrekt fejlet, da antalAften skal være en positiv double.");
    }

    // TEST CASE 9: ANTALMORGEN MUST BE A POSITIVE DOUBLE (-4)
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TC9OpretDagligFastAntalMiddagNegative()
    {
        Patient patient = service.GetPatienter().First();

        Laegemiddel lm = service.GetLaegemidler().First();

        service.OpretDagligFast(3, 2, 4, 1, 2, -6, DateTime.Now, DateTime.Now.AddDays(4));

        Console.WriteLine("Oprettelse af daglig fast er korrekt fejlet, da antalNat skal være positiv double.");
    }
    // Start dato skal sættes til 2023,12,6 (som vil virke i morgen, idag med 2023,12,5)
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TC10OpretDagligFastValidStartDato()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        service.OpretDagligFast(3, 2, 4, 1, 2, 6, DateTime.Now.AddDays(2), DateTime.Now);
        Console.WriteLine("Oprettelse af daglig fast er korrekt fejlet, da patientId ikke eksisterer.");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TC11OpretDagligFastSlutDatoStørreEllerLigStartDato()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        service.OpretDagligFast(3, 2, 4, 1, 2, 6, DateTime.Now.AddDays(4), DateTime.Now);
        Console.WriteLine("Oprettelse af daglig fast er korrekt fejlet, startDato skal være større eller lig slutdate");
    }

    // TEST AF DAGLIG SKÆV

    [TestMethod] // TC2
    [ExpectedException(typeof(ArgumentNullException))]
    public void OpretDagligSkævPatientIdNegativInt()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        Dosis[] doser = new Dosis[] { new Dosis(CreateTimeOnly(12, 0, 0), 3) };
        service.OpretDagligSkaev(-1, lm.LaegemiddelId, doser, DateTime.Now, DateTime.Now.AddDays(4));

        Console.WriteLine("Oprettelse af daglig skæv er korrekt fejlet, da patientId skal være positiv int.");
    }
    [TestMethod] // TC3
    [ExpectedException(typeof(ArgumentNullException))]
    public void OpretDagligSkævUgyldigPatientId()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        Dosis[] doser = new Dosis[] { new Dosis(CreateTimeOnly(12, 0, 0), 3) };
        service.OpretDagligSkaev(11, lm.LaegemiddelId, doser, DateTime.Now, DateTime.Now.AddDays(4));

        Console.WriteLine("Oprettelse af daglig skæv er korrekt fejlet, da patientId ikke eksisterer.");
    }

    
    // TEST CASE 3: PATIENTID NOT FOUND IN THE DATABSE
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OpretDagligSkævPatientIdEksistererIkke()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        Dosis[] doser = new Dosis[] { new Dosis(CreateTimeOnly(12, 0, 0), 3) };
        service.OpretDagligSkaev(15, lm.LaegemiddelId, doser, DateTime.Now, DateTime.Now.AddDays(4));

        Console.WriteLine("Oprettelse af daglig skæv er fejlet korrekt, da det pågældende patient id ikke eksisterer i databasen.");
    }


    [TestMethod]// TEST CASE 4: LAGEMIDDEL NOT FOUND IN THE DATABASE
    [ExpectedException(typeof(ArgumentNullException))]
    public void OpretDagligSkævLaegemiddelIdEksistererIkke()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        Dosis[] doser = new Dosis[] { new Dosis(CreateTimeOnly(12, 0, 0), 3) };
        service.OpretDagligSkaev(patient.PatientId, 11, doser, DateTime.Now, DateTime.Now.AddDays(4));

        Console.WriteLine("Oprettelse af daglig skæv er fejlet korrekt, da det pågældende lægemiddelId ikke eksisterer i databasen.");
    }


   
    // TEST CASE 5: LAEGEMIDDELID MUST BE A POSITIVE INT
    [TestMethod] 
    [ExpectedException(typeof(ArgumentNullException))]
    public void OpretDagligSkævLaegemiddelIdIkkeGyldigInt()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        Dosis[] doser = new Dosis[] { new Dosis(CreateTimeOnly(12, 0, 0), 3) };
        service.OpretDagligSkaev(patient.PatientId, -1, doser, DateTime.Now, DateTime.Now.AddDays(4));

        Console.WriteLine("Oprettelse af daglig skæv er fejlet korrekt, da det pågældende lægemiddelId skal være en positiv int.");
    }



    // TEST CASE 6
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OpretDagligSkævDoserNotNull()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        Dosis[] doser = new Dosis[] { new Dosis(CreateTimeOnly(12, 0, 0), 0) };
        service.OpretDagligSkaev(patient.PatientId, -1, doser, DateTime.Now, DateTime.Now.AddDays(4));

        Console.WriteLine("Oprettelse af daglig skæv er fejlet korrekt, da doser ikke må være 0");
    }

    // TEST CASE 7// Start dato skal sættes til 2023,12,6 (som vil virke i morgen, idag med 2023,12,5)
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OpretDagligSkævValidStartDato()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        Dosis[] doser = new Dosis[] { new Dosis(CreateTimeOnly(12, 0, 0), 0) };
        service.OpretDagligSkaev(patient.PatientId, -1, doser, DateTime.Now, DateTime.Now.AddDays(4));

        Console.WriteLine("Oprettelse af daglig skæv er fejlet korrekt, da startDato skal være en valid dato");
    }

    // TEST CASE 7// 
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OpretDagligSkævSlutDatoStørreEllerLigStartDato()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        Dosis[] doser = new Dosis[] { new Dosis(CreateTimeOnly(12, 0, 0), 0) };
        service.OpretDagligSkaev(patient.PatientId, -1, doser, DateTime.Now, DateTime.Now.AddDays(4));

        Console.WriteLine("Oprettelse af daglig skæv er fejlet korrekt, da slutDato skal være større eller lig startDato");
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

