using LeaveManagement.Infrastructure.Security;

namespace LeaveManagement.Tests;

public class PasswordHasherTests
{
    [Fact]
    public void Hash_And_Verify_Should_Succeed()
    {
        var hasher = new Pbkdf2PasswordHasher();
        const string password = "Strong@123";

        var hash = hasher.Hash(password);

        Assert.True(hasher.Verify(password, hash));
        Assert.False(hasher.Verify("Wrong@123", hash));
    }
}
