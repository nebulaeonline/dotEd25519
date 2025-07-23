using nebulae.dotEd25519;

namespace dotEd25519Tests;

public class Ed25519Tests
{
    [Fact]
    public void GenerateKeypair_ShouldReturn_ValidLengths()
    {
        var (pub, priv) = Ed25519.GenerateKeypair();
        Assert.Equal(32, pub.Length);
        Assert.Equal(64, priv.Length);
    }

    [Fact]
    public void GenerateKeypairFromSeed_ShouldBeDeterministic()
    {
        var seed = new byte[32];
        new Random(1234).NextBytes(seed);

        var (pub1, priv1) = Ed25519.GenerateKeypairFromSeed(seed);
        var (pub2, priv2) = Ed25519.GenerateKeypairFromSeed(seed);

        Assert.Equal(pub1, pub2);
        Assert.Equal(priv1, priv2);
    }

    [Fact]
    public void SignAndVerify_ShouldWork()
    {
        var (pub, priv) = Ed25519.GenerateKeypair();
        var message = System.Text.Encoding.UTF8.GetBytes("Hello, Ed25519!");

        var sig = Ed25519.Sign(message, priv);
        Assert.Equal(64, sig.Length);

        var valid = Ed25519.Verify(message, sig, pub);
        Assert.True(valid);
    }

    [Fact]
    public void Verify_ShouldFail_OnTamperedMessage()
    {
        var (pub, priv) = Ed25519.GenerateKeypair();
        var message = System.Text.Encoding.UTF8.GetBytes("Original");
        var sig = Ed25519.Sign(message, priv);

        var tampered = System.Text.Encoding.UTF8.GetBytes("Corrupted");
        var valid = Ed25519.Verify(tampered, sig, pub);
        Assert.False(valid);
    }

    [Fact]
    public void Verify_ShouldFail_OnTamperedSignature()
    {
        var (pub, priv) = Ed25519.GenerateKeypair();
        var message = System.Text.Encoding.UTF8.GetBytes("Valid Message");
        var sig = Ed25519.Sign(message, priv);

        sig[0] ^= 0xFF; // corrupt signature
        var valid = Ed25519.Verify(message, sig, pub);
        Assert.False(valid);
    }

    [Fact]
    public void SignWithWrongKeyLength_ShouldThrow()
    {
        var msg = new byte[10];
        var badKey = new byte[10];

        var ex = Assert.Throws<ArgumentException>(() => Ed25519.Sign(msg, badKey));
        Assert.Contains("Private key must be", ex.Message);
    }
}