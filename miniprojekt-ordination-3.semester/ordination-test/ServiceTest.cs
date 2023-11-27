namespace ordination_test;

using Microsoft.EntityFrameworkCore;

using Service;
using Data;
using shared.Model;
using shared;

[TestClass]
public class ServiceTest
{
    private DataService service;

    [TestInitialize]
    public void SetupBeforeEachTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrdinationContext>();
        optionsBuilder.UseInMemoryDatabase(databaseName: "test-database");
        var context = new OrdinationContext(optionsBuilder.Options);
        service = new DataService(context);
        service.SeedData();
    }

    [TestMethod]
    public void PatientsExist()
    {
        Assert.IsNotNull(service.GetPatienter());
    }

    [TestMethod]
    public void OpretDagligFast()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        Assert.AreEqual(1, service.GetDagligFaste().Count());

        service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId,
            2, 2, 1, 0, DateTime.Now, DateTime.Now.AddDays(3));

        Assert.AreEqual(2, service.GetDagligFaste().Count());
    }

    [TestMethod]
    public void OpretDagligSkaev()
    {
        // Hent en patient og et lægemiddel
        Patient patient = service.GetPatienter().First();
        Laegemiddel laegemiddel = service.GetLaegemidler().First();

        // Opret en daglig skæv
        service.OpretDagligSkaev(patient.PatientId, laegemiddel.LaegemiddelId,
            new Dosis[] {
                new Dosis(DateTime.Now, 2),
                new Dosis(DateTime.Now.AddHours(6), 2)
            },DateTime.Now, DateTime.Now.AddDays(3));

        //vi forventer 2 og sammenligner det med vores egentlige antal
        Assert.AreEqual(2, service.GetDagligSkæve().Count());



        service.OpretDagligSkaev(patient.PatientId, laegemiddel.LaegemiddelId,
           new Dosis[] {
                new Dosis(Util.CreateTimeOnly(12, 0, 0), 0.5),
                new Dosis(Util.CreateTimeOnly(12, 40, 0), 1),
                new Dosis(Util.CreateTimeOnly(16, 0, 0), 2.5),
                new Dosis(Util.CreateTimeOnly(18, 45, 0), 3)

           }, new DateTime(2023, 01, 01), new DateTime(2023, 02, 01));

        // tjekker man om der er oprettet 3 til listen
        Assert.AreEqual(3, service.GetDagligSkæve().Count());


    }

    [TestMethod]
    //[ExpectedException(typeof(ArgumentNullException))]
    [ExpectedException(typeof(Exception))]

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