# RSA-Encryption-Bug
Demonstrates problem with the .NET Framework RSA implementations leaving copies of clear text data in memory.

After using either the RSACryptoServiceProvider or the RSACng class to decrypt a previously encrypted string, a copy of the clear text 
data is left behind in memory. These copies seem to be created either by the managed to native marshalling of the output byte array or
by the native libraries used for decryption. The problem also happens when encrypting, but this sample focuses on the decryption flaw.

This .NET console application demonstrates the problem by decrypting a previously encrypted string and then zeroing the returned 
memory buffer. The application then waits for user input so you can take a memory dump and see that the clear text data is still 
resident in memory. Interestingly, it does not exhibit on our Windows 10 developer machines but it does exhibit on our Windows 
Server 2016 Data Center VMs.

To demonstrate the problem using the sample application:

1. Download the strings utility from sysinternals. Run the strings command once with no parameters and accept the EULA.
2. Build the sample application. We normally run the test with a Release build.
3. Start Task Manager.
4. From a console window, cd to the folder containing the console application and then run the following command: _.\ConsoleApp rsa_
5. In Task Manager, locate ConsoleApp.exe and create a memory dump.
6. Search the dump file for the clear text data - 2223000010309703. E.g. from the command prompt run: _find "2223000010309703" C:\Users\admin\AppData\Local\Temp\ConsoleApp.DMP_

This will demonstrate the problem using the RSACryptoServiceProvider. To demonstrate the problem using RSACng, change the command in 
step two to: _.\ConsoleApp cng_

