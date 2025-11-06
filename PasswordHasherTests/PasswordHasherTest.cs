using PriceFlowSecurity;

namespace PasswordHasherTests;

[TestClass]
public sealed class PasswordHasherTest
{

    private static string EncryptAndFormatPasswordForSQLScript(string password)
    {
        return string.Concat("0x", BitConverter.ToString(PasswordHelper.CalculateHashAndSalt(password)).Replace("-", ""));
    }
    [TestMethod]
    public void GenerateAndSaveAdminPasswordHash()
    {
        string passwordHash = EncryptAndFormatPasswordForSQLScript("Admin123.");

        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "admin_password.txt");

        File.WriteAllText(filePath, passwordHash);

        Assert.IsTrue(File.Exists(filePath), "The hash file was not created");
        Console.WriteLine($"Hashed password saved to: {filePath}");
    }
}
