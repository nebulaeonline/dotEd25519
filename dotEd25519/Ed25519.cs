using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotEd25519;

public static class Ed25519
{
    public const int PublicKeyLength = 32;
    public const int PrivateKeyLength = 64;
    public const int SignatureLength = 64;
    public const int SeedLength = 32;

    static Ed25519()
    {
        // Ensure the native library is loaded
        Ed25519Library.Init();
    }

    /// <summary>
    /// Generates a new Ed25519 public and private key pair.
    /// </summary>
    /// <remarks>The keys are generated using the Ed25519 algorithm, which is known for its high performance
    /// and security.</remarks>
    /// <returns>A tuple containing the generated public key and private key as byte arrays.</returns>
    public static (byte[] PublicKey, byte[] PrivateKey) GenerateKeypair()
    {
        var pub = new byte[PublicKeyLength];
        var priv = new byte[PrivateKeyLength];
        Ed25519Interop.ed25519_generate_keypair(pub, priv);
        return (pub, priv);
    }

    /// <summary>
    /// Generates a public and private key pair from the specified seed.
    /// </summary>
    /// <param name="seed">The seed used to generate the key pair. Must be exactly <see cref="SeedLength"/> bytes long.</param>
    /// <returns>A tuple containing the generated public key and private key.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="seed"/> is not <see cref="SeedLength"/> bytes long.</exception>
    public static (byte[] PublicKey, byte[] PrivateKey) GenerateKeypairFromSeed(byte[] seed)
    {
        if (seed.Length != SeedLength)
            throw new ArgumentException($"Seed must be {SeedLength} bytes.", nameof(seed));

        var pub = new byte[PublicKeyLength];
        var priv = new byte[PrivateKeyLength];
        Ed25519Interop.ed25519_generate_keypair_from_seed(pub, priv, seed);
        return (pub, priv);
    }

    /// <summary>
    /// Generates a digital signature for the specified message using the provided private key.
    /// </summary>
    /// <param name="message">The message to be signed, represented as a read-only span of bytes.</param>
    /// <param name="privateKey">The private key used to sign the message. Must be exactly <see cref="PrivateKeyLength"/> bytes long.</param>
    /// <returns>A byte array containing the digital signature of the message.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="privateKey"/> is not <see cref="PrivateKeyLength"/> bytes long.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the signing operation fails.</exception>
    public static byte[] Sign(ReadOnlySpan<byte> message, byte[] privateKey)
    {
        if (privateKey.Length != PrivateKeyLength)
            throw new ArgumentException($"Private key must be {PrivateKeyLength} bytes.", nameof(privateKey));

        var sig = new byte[SignatureLength];
        var msg = message.ToArray();

        if (Ed25519Interop.ed25519_sign(sig, msg, (UIntPtr)msg.Length, privateKey) != 1)
            throw new InvalidOperationException("Signing failed.");

        return sig;
    }

    /// <summary>
    /// Verifies the authenticity of a message using the provided signature and public key.
    /// </summary>
    /// <param name="message">The message to verify, represented as a read-only span of bytes.</param>
    /// <param name="signature">The signature to verify against, which must be exactly <see cref="SignatureLength"/> bytes long.</param>
    /// <param name="publicKey">The public key used for verification, which must be exactly <see cref="PublicKeyLength"/> bytes long.</param>
    /// <returns><see langword="true"/> if the signature is valid for the given message and public key; otherwise, <see
    /// langword="false"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="signature"/> is not <see cref="SignatureLength"/> bytes long or if <paramref
    /// name="publicKey"/> is not <see cref="PublicKeyLength"/> bytes long.</exception>
    public static bool Verify(ReadOnlySpan<byte> message, byte[] signature, byte[] publicKey)
    {
        if (signature.Length != SignatureLength)
            throw new ArgumentException($"Signature must be {SignatureLength} bytes.", nameof(signature));

        if (publicKey.Length != PublicKeyLength)
            throw new ArgumentException($"Public key must be {PublicKeyLength} bytes.", nameof(publicKey));

        var msg = message.ToArray();
        return Ed25519Interop.ed25519_verify(msg, (UIntPtr)msg.Length, signature, publicKey) == 1;
    }
}
