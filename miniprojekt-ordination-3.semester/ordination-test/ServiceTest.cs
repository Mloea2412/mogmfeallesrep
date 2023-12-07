namespace ordination_test;

using Microsoft.EntityFrameworkCore;

using Service;
using Data;
using shared.Model;
using shared;
using static shared.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class ServiceTest
{
    private DataService service;

    [TestInitialize]
    // Denne attribut markerer metoden som en initialiseringsmetode, der skal køres før hver testmetode.
    public void SetupBeforeEachTest()
    {
        // Opretter en instans af DbContextOptionsBuilder for din OrdinationContext.
        var optionsBuilder = new DbContextOptionsBuilder<OrdinationContext>();

        // Konfigurerer optionsBuilder til at bruge en in-memory database med et specifikt navn for test-databasen.
        optionsBuilder.UseInMemoryDatabase(databaseName: "test-database");

        // Opretter en ny instans af OrdinationContext ved at bruge de konfigurerede options og optionsBuilder.
        var context = new OrdinationContext(optionsBuilder.Options);

        // Opretter en ny instans af DataService og tildeler den til testens service-variabel.
        service = new DataService(context);

        // Kalder SeedData-metoden på DataService-instansen for at udfylde in-memory-databasen med testdata.
        service.SeedData();
    }


    [TestMethod]
    // Denne attribut markerer metoden som en testmetode.
    public void PatientsExist()
    {
        // Tjekker, om der er nogen patienter i testtjenesten. Forventningen er, at der er mindst én, da SeedData-metoden kaldes under initialiseringen.
        Assert.IsNotNull(service.GetPatienter());
    }


    // DER SKAL LAVES TESTCASE NUMMER, SÅ DET KAN DIREKTE HENFØRES TIL VORES DOKUMENTATION

    [TestMethod]
    // Denne attribut markerer metoden som en testmetode.
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
        service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId, 2, 2, 1, 0, DateTime.Now, DateTime.Now.AddDays(2));

        // Tjekker, om der nu er to daglige faste objekter i tjenesten. Forventningen er, at antallet er steget fra 1 til 2.
        Assert.AreEqual(2, service.GetDagligFaste().Count());
    }



    [TestMethod]
    // Denne attribut markerer metoden som en testmetode.
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



    /* Her har du lidt noter fra hvad jeg testet som ikke virker ://
       [TestMethod]
    public void PN_TEST_VIRKERIKKE()
    {

        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();
        service.OpretPN(patient.PatientId, lm.LaegemiddelId, 3, startDato: new DateTime(2023, 12, 7), slutDato: new DateTime(2023, 12, 23));
        DateTime testdag = new DateTime(2023, 12, 30);
        service.AnvendOrdination(id: 1, new Dato { dato = testdag });        
    }
    */

    // TEST AF PN //

    [TestMethod]// TC2: PATIENTID -2
    [ExpectedException(typeof(ArgumentNullException))]
    public void TC3OpretPNPatientIdUgyldigInt()
    {
        // Henter den første laegemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Forsøger at oprette en PN med et ikke-eksisterende laegemiddel.
        service.OpretPN(-2, lm.LaegemiddelId, 2, DateTime.Now, DateTime.Now.AddDays(2));

        // Udskriver en besked, hvis oprettelse af PN fejler, og patientens id skal være et positivt heltal.
        Console.WriteLine("Oprettelse af PN er korrekt fejlet, patienten id skal være positivt heltal.");
    }



    [TestMethod] // TC3: PATIENTID 11
    [ExpectedException(typeof(ArgumentNullException))]
    public void TC3OpretPNPatientIdeEksistererikke()
    {
        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Forsøger at oprette en PN med en ikke-eksisterende patient (PatientId 11).
        service.OpretPN(11, lm.LaegemiddelId, 8, DateTime.Now, DateTime.Now.AddDays(2));

        // Udskriver en besked, hvis oprettelse af PN fejler, og patientens id eksisterer ikke.
        Console.WriteLine("Oprettelse PN er fejlet korrekt, patientenId eksisterer ikke.");
    }



    [TestMethod]// TC4: LAEGEMIDDELID -4
    [ExpectedException(typeof(ArgumentNullException))]
    public void TC4OpretPNLaegemiddelIdUgyldigInt()
    {
        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Forsøger at oprette en PN med en ugyldig lægemiddel-ID (-4).
        service.OpretPN(patient.PatientId, -4, 8, DateTime.Now, DateTime.Now.AddDays(2));

        // Udskriver en besked, hvis oprettelse af PN fejler, og lægemiddel-ID skal være et positivt heltal.
        Console.WriteLine("Oprettelse af PN er korrekt fejlet, lægemiddelId skal være positivt heltal.");
    }


    [TestMethod]// TC5: LAEGEMIDDELID 11
    [ExpectedException(typeof(ArgumentNullException))]
    public void TC5OpretPNLaegemiddelIdUgyldigInt()
    {
        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Forsøger at oprette en PN med et ikke-eksisterende lægemiddel-ID (11).
        service.OpretPN(patient.PatientId, 11, 8, DateTime.Now, DateTime.Now.AddDays(2));

        // Udskriver en besked, hvis oprettelse af PN fejler, og lægemiddel-ID eksisterer ikke i databasen.
        Console.WriteLine("Oprettelse af PN er korrekt fejlet, lægemiddelId eksisterer ikke i databasen.");
    }


    [TestMethod]// TC6: SLUTDATO FØR STARTDATO
    [ExpectedException(typeof(ArgumentNullException))]
    public void TC6OpretPNSlutDatoSkalVæreStørreEllerLigStartDato()
    {
        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Forsøger at oprette en PN med en startDato, der er efter slutDato.
        service.OpretPN(patient.PatientId, lm.LaegemiddelId, 8, DateTime.Now.AddDays(4), DateTime.Now);

        // Udskriver en besked, hvis oprettelse af PN fejler, da slutDato skal være større eller lig startDato.
        Console.WriteLine("Oprettelse af PN er korrekt fejlet, da slutDato skal være større eller lig startDato.");
    }


    [TestMethod]// TC7: STARTDATO SKAL VÆRE VALID
    [ExpectedException(typeof(ArgumentNullException))]
    public void TC7OpretPNStartDatoSkalVæreValid()
    {
        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Forsøger at oprette en PN med en startDato, der er før dagens dato.
        service.OpretPN(patient.PatientId, lm.LaegemiddelId, 8, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(4));

        // Udskriver en besked, hvis oprettelse af PN fejler, da startDato skal være dagens dato eller senere.
        Console.WriteLine("Oprettelse af PN er korrekt fejlet, da startDato skal være dagens dato eller senere.");
    }



    // TEST AF DAGLIG FAST //

    [TestMethod] // TC2: PATIENTID -5
    [ExpectedException(typeof(ArgumentNullException))]
    public void TC2OpretDagligFastPatientIdNegativInt()
    {
        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Forsøger at oprette en daglig fast dosis med et negativt patient-ID (-5).
        // Forventer, at en ArgumentNullException bliver kastet som en fejl.
        service.OpretDagligFast(-5, lm.LaegemiddelId, 4, 1, 2, 6, DateTime.Now, DateTime.Now.AddDays(4));

        // Udskriver en besked til konsollen for at bekræfte, at oprettelsen af daglig fast dosis fejlede korrekt.
        Console.WriteLine("Oprettelse af daglig fast er korrekt fejlet, da patientId skal være positiv int.");
    }



    [TestMethod] // TEST CASE 3: PATIENTID 11
    [ExpectedException(typeof(ArgumentNullException))]
    public void TC3OpretDagligFastPatientEksistererIkke()
    {
        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Forsøger at oprette en daglig fast dosis med en ikke-eksisterende patient (PatientId 11).
        // Forventer, at en ArgumentNullException bliver kastet som en fejl.
        service.OpretDagligFast(11, lm.LaegemiddelId, 4, 1, 2, 6, DateTime.Now, DateTime.Now.AddDays(4));

        // Udskriver en besked til konsollen for at bekræfte, at oprettelsen af daglig fast dosis fejlede korrekt.
        Console.WriteLine("Oprettelse af daglig fast er korrekt fejlet, da patientId ikke eksisterer.");
    }



    [TestMethod] // TEST CASE 4: LAEGEMIDDELID 11
    [ExpectedException(typeof(ArgumentNullException))]
    public void TC4OpretDagligFastLaegemiddelEksistererIkke()
    {
        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Forsøger at oprette en daglig fast dosis med et ikke-eksisterende lægemiddel (LaegemiddelId 11).
        // Forventer, at en ArgumentNullException bliver kastet som en fejl.
        service.OpretDagligFast(patient.PatientId, 11, 4, 1, 2, 6, DateTime.Now, DateTime.Now.AddDays(4));

        // Udskriver en besked til konsollen for at bekræfte, at oprettelsen af daglig fast dosis fejlede korrekt.
        Console.WriteLine("Oprettelse af daglig fast er korrekt fejlet, da laegemiddelId ikke eksisterer.");
    }



    [TestMethod] // TEST CASE 5: LAEGEMIDDELID -2
    [ExpectedException(typeof(ArgumentNullException))]
    public void TC5OpretDagligFastLaegemiddelIdNegativInt()
    {
        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Forsøger at oprette en daglig fast dosis med et negativt lægemiddel-ID (-2).
        // Forventer, at en ArgumentNullException bliver kastet som en fejl.
        service.OpretDagligFast(patient.PatientId, -2, 4, 1, 2, 6, DateTime.Now, DateTime.Now.AddDays(4));

        // Udskriver en besked til konsollen for at bekræfte, at oprettelsen af daglig fast dosis fejlede korrekt.
        Console.WriteLine("Oprettelse af daglig fast er korrekt fejlet, da laegemiddelId skal være positiv int.");
    }



    [TestMethod] // TEST CASE 6: ANTALMORGEN -4
    [ExpectedException(typeof(ArgumentNullException))]
    public void TC6OpretDagligFastAntalMorgenNegative()
    {
        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Forsøger at oprette en daglig fast dosis med et negativt antalMorgen (-4).
        // Forventer, at en ArgumentNullException bliver kastet som en fejl.
        service.OpretDagligFast(3, 2, -4, 1, 2, 6, DateTime.Now, DateTime.Now.AddDays(4));

        // Udskriver en besked til konsollen for at bekræfte, at oprettelsen af daglig fast dosis fejlede korrekt.
        Console.WriteLine("Oprettelse af daglig fast er korrekt fejlet, da antalMorgen skal være en positiv double.");
    }



    [TestMethod] // TEST CASE 7: ANTALMIDDAG -1
    [ExpectedException(typeof(ArgumentNullException))]
    public void TC7OpretDagligFastAntalMiddagNegative()
    {
        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Forsøger at oprette en daglig fast dosis med et negativt antalMiddag (-1).
        // Forventer, at en ArgumentNullException bliver kastet som en fejl.
        service.OpretDagligFast(3, 2, 4, -1, 2, 6, DateTime.Now, DateTime.Now.AddDays(4));

        // Udskriver en besked til konsollen for at bekræfte, at oprettelsen af daglig fast dosis fejlede korrekt.
        Console.WriteLine("Oprettelse af daglig fast er korrekt fejlet, da antalMiddag skal være en positiv double.");
    }



    [TestMethod] // TEST CASE 8: ANTALAFTEN -2
    [ExpectedException(typeof(ArgumentNullException))]
    public void TC8OpretDagligFastAntalAftenNegative()
    {
        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Forsøger at oprette en daglig fast dosis med et negativt antalAften (-2).
        // Forventer, at en ArgumentNullException bliver kastet som en fejl.
        service.OpretDagligFast(3, 2, 4, 1, -2, 6, DateTime.Now, DateTime.Now.AddDays(4));

        // Udskriver en besked til konsollen for at bekræfte, at oprettelsen af daglig fast dosis fejlede korrekt.
        Console.WriteLine("Oprettelse af daglig fast er korrekt fejlet, da antalAften skal være en positiv double.");
    }



    [TestMethod]// TEST CASE 9: ANTALAFTEN -6
    [ExpectedException(typeof(ArgumentNullException))]
    public void TC9OpretDagligFastAntalNatgNegative()
    {
        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Forsøger at oprette en daglig fast dosis med et negativt antalNat (-6).
        // Forventer, at en ArgumentNullException bliver kastet som en fejl.
        service.OpretDagligFast(3, 2, 4, 1, 2, -6, DateTime.Now, DateTime.Now.AddDays(4));

        // Udskriver en besked til konsollen for at bekræfte, at oprettelsen af daglig fast dosis fejlede korrekt.
        Console.WriteLine("Oprettelse af daglig fast er korrekt fejlet, da antalNat skal være en positiv double.");
    }



    [TestMethod] // TEST CASE 10: IKKE VALID STARTDATO
    [ExpectedException(typeof(ArgumentNullException))]
    public void TC10OpretDagligFastValidStartDato()
    {
        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Forsøger at oprette en daglig fast dosis med en ugyldig startDato (1 dag før dagens dato).
        // Forventer, at en ArgumentNullException bliver kastet som en fejl.
        service.OpretDagligFast(3, 2, 4, 1, 2, 6, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(4));

        // Udskriver en besked til konsollen for at bekræfte, at oprettelsen af daglig fast dosis fejlede korrekt.
        Console.WriteLine("Oprettelse af daglig fast er korrekt fejlet, da startDato ikke er gyldig.");
    }




    [TestMethod] // TEST CASE 11: IKKE VALID SLUTDATO
    [ExpectedException(typeof(ArgumentNullException))]
    public void TC11OpretDagligFastSlutDatoStørreEllerLigStartDato()
    {
        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Forsøger at oprette en daglig fast dosis med en ikke-valid slutDato (før startDato).
        // Forventer, at en ArgumentNullException bliver kastet som en fejl.
        service.OpretDagligFast(3, 2, 4, 1, 2, 6, DateTime.Now.AddDays(4), DateTime.Now);

        // Udskriver en besked til konsollen for at bekræfte, at oprettelsen af daglig fast dosis fejlede korrekt.
        Console.WriteLine("Oprettelse af daglig fast er korrekt fejlet, startDato skal være større eller lig slutdate.");
    }




    // TEST AF DAGLIG SKÆV

    [TestMethod] // TC2: PATIENTID -1
    [ExpectedException(typeof(ArgumentNullException))]
    public void OpretDagligSkævPatientIdNegativInt()
    {
        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Opretter en skæv daglig dosis med et negativt patient-ID (-1).
        // Forventer, at en ArgumentNullException bliver kastet som en fejl.
        Dosis[] doser = new Dosis[] { new Dosis(CreateTimeOnly(12, 0, 0), 3) };
        service.OpretDagligSkaev(-1, lm.LaegemiddelId, doser, DateTime.Now, DateTime.Now.AddDays(4));

        // Udskriver en besked til konsollen for at bekræfte, at oprettelsen af daglig skæv dosis fejlede korrekt.
        Console.WriteLine("Oprettelse af daglig skæv er korrekt fejlet, da patientId skal være positiv int.");
    }



    [TestMethod] // TC3: PATIENTID 11
    [ExpectedException(typeof(ArgumentNullException))]
    public void OpretDagligSkævUgyldigPatientId()
    {
        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Opretter en skæv daglig dosis med et ikke-eksisterende patient-ID (11).
        // Forventer, at en ArgumentNullException bliver kastet som en fejl.
        Dosis[] doser = new Dosis[] { new Dosis(CreateTimeOnly(12, 0, 0), 3) };
        service.OpretDagligSkaev(11, lm.LaegemiddelId, doser, DateTime.Now, DateTime.Now.AddDays(4));

        // Udskriver en besked til konsollen for at bekræfte, at oprettelsen af daglig skæv dosis fejlede korrekt.
        Console.WriteLine("Oprettelse af daglig skæv er korrekt fejlet, da patientId ikke eksisterer.");
    }



    [TestMethod]// TEST CASE 4: LAGEMIDDELID 11
    [ExpectedException(typeof(ArgumentNullException))]
    public void OpretDagligSkævLaegemiddelIdEksistererIkke()
    {
        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Opretter en skæv daglig dosis med et ikke-eksisterende lægemiddel-ID (11).
        // Forventer, at en ArgumentNullException bliver kastet som en fejl.
        Dosis[] doser = new Dosis[] { new Dosis(CreateTimeOnly(12, 0, 0), 3) };
        service.OpretDagligSkaev(patient.PatientId, 11, doser, DateTime.Now, DateTime.Now.AddDays(4));

        // Udskriver en besked til konsollen for at bekræfte, at oprettelsen af daglig skæv dosis fejlede korrekt.
        Console.WriteLine("Oprettelse af daglig skæv er fejlet korrekt, da det pågældende lægemiddelId ikke eksisterer i databasen.");
    }




    [TestMethod] // TEST CASE 5: LAEGEMIDDEL -1
    [ExpectedException(typeof(ArgumentNullException))]
    public void OpretDagligSkævLaegemiddelIdIkkeGyldigInt()
    {
        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Opretter en skæv daglig dosis med et ikke-gyldigt lægemiddel-ID (-1).
        // Forventer, at en ArgumentNullException bliver kastet som en fejl.
        Dosis[] doser = new Dosis[] { new Dosis(CreateTimeOnly(12, 0, 0), 3) };
        service.OpretDagligSkaev(patient.PatientId, -1, doser, DateTime.Now, DateTime.Now.AddDays(4));

        // Udskriver en besked til konsollen for at bekræfte, at oprettelsen af daglig skæv dosis fejlede korrekt.
        Console.WriteLine("Oprettelse af daglig skæv er fejlet korrekt, da det pågældende lægemiddelId skal være en positiv int.");
    }



    [TestMethod] // TEST CASE 6: DOSER MÅ IKKE VÆRE TOM
    [ExpectedException(typeof(ArgumentNullException))]
    public void OpretDagligSkævDoserNotNull()
    {
        // Opretter en skæv daglig dosis med tomme doser.
        // Forventer, at en ArgumentNullException bliver kastet som en fejl.
        Dosis[] doser = new Dosis[] { };
        service.OpretDagligSkaev(3, 1, doser, DateTime.Now, DateTime.Now.AddDays(4));

        // Udskriver en besked til konsollen for at bekræfte, at oprettelsen af daglig skæv dosis fejlede korrekt.
        Console.WriteLine("oprettelse af daglig skæv fejlet korrekt, da doser er tom");
    }



    [TestMethod]// TEST CASE 7: VALID STARTDATO
    [ExpectedException(typeof(ArgumentNullException))]
    public void OpretDagligSkævValidStartDato()
    {
        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Opretter en skæv daglig dosis med en ikke-valid startDato (1 dag før dagens dato).
        // Forventer, at en ArgumentNullException bliver kastet som en fejl.
        Dosis[] doser = new Dosis[] { new Dosis(CreateTimeOnly(12, 0, 0), 0) };
        service.OpretDagligSkaev(3, 1, doser, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(4));

        // Udskriver en besked til konsollen for at bekræfte, at oprettelsen af daglig skæv dosis fejlede korrekt.
        Console.WriteLine("Oprettelse af daglig skæv er fejlet korrekt, da startDato skal være en valid dato");
    }


    [TestMethod] // TEST CASE 8 
    [ExpectedException(typeof(ArgumentNullException))]
    public void OpretDagligSkævSlutDatoStørreEllerLigStartDato()
    {
        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel lm = service.GetLaegemidler().First();

        // Opretter en skæv daglig dosis med en slutDato, der ikke er større eller lig med startDato (4 dage efter dagens dato og derefter dagens dato).
        // Forventer, at en ArgumentNullException bliver kastet som en fejl.
        Dosis[] doser = new Dosis[] { new Dosis(CreateTimeOnly(12, 0, 0), 0) };
        service.OpretDagligSkaev(3, 1, doser, DateTime.Now.AddDays(4), DateTime.Now);

        // Udskriver en besked til konsollen for at bekræfte, at oprettelsen af daglig skæv dosis fejlede korrekt.
        Console.WriteLine("Oprettelse af daglig skæv er fejlet korrekt, da slutDato skal være større eller lig startDato");
    }



    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestAtKodenSmiderEnException()
    {
        // Koden herunder giver en exception, fordi startdatoen starter i februar, og slutdatoen er en måned før.

        // Testen fejler, hvis man indsætter korrekte start- og slutdatoer.

        // Henter den første patient fra tjenesten.
        Patient patient = service.GetPatienter().First();

        // Henter den første lægemiddel fra tjenesten.
        Laegemiddel laegemiddel = service.GetLaegemidler().First();

        // Opretter en skæv daglig dosis med en startdato (1. februar 2023) og en slutdato (1. januar 2023),
        // der resulterer i en exception.
        // Forventer, at en ArgumentNullException bliver kastet som en fejl.
        service.OpretDagligSkaev(patient.PatientId, laegemiddel.LaegemiddelId,
           new Dosis[] {
            new Dosis(Util.CreateTimeOnly(12, 0, 0), 0.5),
            new Dosis(Util.CreateTimeOnly(12, 40, 0), 1),
            new Dosis(Util.CreateTimeOnly(16, 0, 0), 2.5),
            new Dosis(Util.CreateTimeOnly(18, 45, 0), 3)
           }, new DateTime(2023, 2, 01), new DateTime(2023, 01, 01));

        // Udskriver en besked til konsollen for at bekræfte, at koden smider en exception som forventet.
        Console.WriteLine("Testen forventer, at koden smider en exception, da startdatoen er i februar og slutdatoen er en måned før.");
    }

}

