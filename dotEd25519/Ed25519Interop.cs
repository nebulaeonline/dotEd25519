using System.Runtime.InteropServices;

namespace nebulae.dotEd25519;

internal class Ed25519Interop
{
    private const string LIB = "ed25519";

    [DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void ed25519_generate_keypair(byte[] pub, byte[] priv);

    [DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void ed25519_generate_keypair_from_seed(byte[] pub, byte[] priv, byte[] seed);

    [DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int ed25519_sign(byte[] sig, byte[] msg, UIntPtr msgLen, byte[] priv);

    [DllImport(LIB, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int ed25519_verify(byte[] msg, UIntPtr msgLen, byte[] sig, byte[] pub);
}
