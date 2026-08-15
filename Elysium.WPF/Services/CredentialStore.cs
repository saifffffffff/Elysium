using System.Runtime.InteropServices;

namespace Elysium.WPF.Services;

/// <summary>
/// Stores sign-in credentials in the Windows Credential Manager vault
/// </summary>
public static class CredentialStore
{
    private const string Target = "Elysium:SignIn";
    private const int CredTypeGeneric = 1;
    private const int CredPersistLocalMachine = 2;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct Credential
    {
        public int Flags;
        public int Type;
        public IntPtr TargetName;
        public IntPtr Comment;
        public long LastWritten;
        public int CredentialBlobSize;
        public IntPtr CredentialBlob;
        public int Persist;
        public int AttributeCount;
        public IntPtr Attributes;
        public IntPtr TargetAlias;
        public IntPtr UserName;
    }

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool CredWrite(ref Credential credential, uint flags);

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool CredRead(string target, int type, int reservedFlag, out IntPtr credentialPtr);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool CredDelete(string target, int type, int flags);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern void CredFree(IntPtr buffer);

    /// <summary>
    /// Save the credentials to the Windows Credential Manager vault
    /// </summary>
    public static void Save(string username, string password)
    {
        var credential = new Credential
        {
            Type = CredTypeGeneric,
            TargetName = Marshal.StringToCoTaskMemUni(Target),
            UserName = Marshal.StringToCoTaskMemUni(username),
            CredentialBlob = Marshal.StringToCoTaskMemUni(password),
            CredentialBlobSize = (password.Length + 1) * 2,
            Persist = CredPersistLocalMachine
        };

        try
        {
            CredWrite(ref credential, 0);
        }
        finally
        {
            Marshal.FreeCoTaskMem(credential.TargetName);
            Marshal.FreeCoTaskMem(credential.UserName);
            Marshal.FreeCoTaskMem(credential.CredentialBlob);
        }
    }

    /// <summary>
    /// Load the saved credentials, or null if none are stored
    /// </summary>
    public static (string Username, string Password)? Load()
    {
        if (!CredRead(Target, CredTypeGeneric, 0, out var credentialPtr))
            return null;

        try
        {
            var credential = Marshal.PtrToStructure<Credential>(credentialPtr);
            var username = Marshal.PtrToStringUni(credential.UserName);
            var password = Marshal.PtrToStringUni(credential.CredentialBlob);

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            return (username, password);
        }
        finally
        {
            CredFree(credentialPtr);
        }
    }

    /// <summary>
    /// Delete any stored credentials
    /// </summary>
    public static void Delete()
    {
        CredDelete(Target, CredTypeGeneric, 0);
    }
}