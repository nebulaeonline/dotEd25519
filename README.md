# dotEd25519

A high-performance, minimal, cross-platform wrapper around BoringSSL's Ed25519 implementation, exposing only the raw primitives you need for secure key generation, signing, and verification - with no ASN.1, no certificates, no baggage.

The native backend is compiled directly from BoringSSL's curve25519.cc, sha512.c, and related modules - ensuring constant-time cryptographic operations and production-grade optimizations across Windows, Linux, and macOS.

Tests are included and available in the Github repo.

[![NuGet](https://img.shields.io/nuget/v/nebulae.dotEd25519.svg)](https://www.nuget.org/packages/nebulae.dotEd25519)

---

## Features

- **Cross-platform**: Works on Windows, Linux, and macOS (x64 & Apple Silicon).
- **High performance**: Optimized for speed, leveraging native SIMD-enabled code.
- **Easy to use**: Simple API for generating keys and signing/verification.
- **Secure**: Uses Google's BoringSSL implementation, which is widely trusted in the industry.
- **Minimal dependencies**: No external dependencies required (all are included), making it lightweight and easy to integrate.

---

## Requirements

- .NET 8.0 or later
- Windows x64, Linux x64, or macOS (x64 & Apple Silicon)

---

## Usage

```csharp

using System;
using nebulae.dotEd25519;

public static class Example
{
    public static void Main()
    {
        Console.WriteLine("Ed25519 Example Using nebulae.dotEd25519");

        // 1. Generate a random keypair
        var (publicKey1, privateKey1) = Ed25519.GenerateKeypair();
        Console.WriteLine("Generated random keypair");

        // 2. Generate keypair from a fixed seed
        var seed = new byte[32];
        for (int i = 0; i < seed.Length; i++)
            seed[i] = (byte)i;

        var (publicKey2, privateKey2) = Ed25519.GenerateKeypairFromSeed(seed);
        Console.WriteLine("Generated keypair from seed");

        // 3. Sign a message with both keypairs
        var message = System.Text.Encoding.UTF8.GetBytes("Hello from Ed25519!");

        var signature1 = Ed25519.Sign(message, privateKey1);
        var signature2 = Ed25519.Sign(message, privateKey2);

        Console.WriteLine("Signed message with both keys");

        // 4. Verify both signatures
        bool v1 = Ed25519.Verify(message, signature1, publicKey1);
        bool v2 = Ed25519.Verify(message, signature2, publicKey2);

        Console.WriteLine($"Verified Signature1: {(v1 ? "VALID" : "INVALID")}");
        Console.WriteLine($"Verified Signature2: {(v2 ? "VALID" : "INVALID")}");

        // 5. Tamper check
        signature1[0] ^= 0xFF;
        bool tampered = Ed25519.Verify(message, signature1, publicKey1);
        Console.WriteLine($"Tampered Signature1 valid? {(tampered ? "YES (BUG)" : "NO (GOOD)")}");
    }
}

```

---

## Installation

You can install the package via NuGet:

```bash

$ dotnet add package nebulae.dotEd25519

```

Or via git:

```bash

$ git clone https://github.com/nebulaeonline/dotEd25519.git
$ cd dotEd25519
$ dotnet build

```

---

## License

MIT

## Roadmap

Unless there are vulnerabilities found, there are no plans to add any new features. The focus of this library is low-level Ed25519 operations and raw keys. Other packages handle pem/der already.