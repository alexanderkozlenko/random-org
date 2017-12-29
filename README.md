## Community.RandomOrg

A client for the [RANDOM.ORG](https://www.random.org) service.

[![NuGet package](https://img.shields.io/nuget/v/Community.RandomOrg.svg?style=flat-square)](https://www.nuget.org/packages/Community.RandomOrg)

The client supports all RANDOM.ORG [Core API v2](https://api.random.org/json-rpc/2) methods with operation cancellation ability.

```cs
using (var client = new RandomOrgClient("YOUR_API_KEY_HERE"))
{
    // Generate an integer from the [0,10] range without replacement
    var rii = await client.GenerateIntegersAsync(1, 0, 10, false);
    // Generate a decimal fraction with 8 decimal places without replacement
    var rif = await client.GenerateDecimalFractionsAsync(1, 8, false);
    // Generate a number from a Gaussian distribution with mean of 0.0,
    // standard deviation of 1.0, and 8 significant digits
    var rig = await client.GenerateGaussiansAsync(1, 0.0m, 1.0m, 8);
    // Generate a string with length of 5 from the specified letters
    var ris = await client.GenerateStringsAsync(1, 5, "abcde", false);
    // Generate an UUID
    var riu = await client.GenerateUuidsAsync(1);
    // Generate a BLOB with length of 8 bytes (64 bits)
    var rib = await client.GenerateBlobsAsync(1, 64);
    // Each generation method has a corresponding one for generating random data
    // with signature, which can be verified afterwards
    var srii = await client.GenerateSignedIntegersAsync(1, 0, 10, false);
    // Signature verification can be executed without specifying an API key
    var vrii = await client.VerifySignatureAsync(srii.Random, srii.Signature);

    Console.WriteLine("Integer: " + rii.Random.Data[0]);
    Console.WriteLine("Decimal fraction: " + rif.Random.Data[0]);
    Console.WriteLine("Gaussian number: " + rig.Random.Data[0]);
    Console.WriteLine("Dtring: " + ris.Random.Data[0]);
    Console.WriteLine("UUID: " + riu.Random.Data[0]);
    Console.WriteLine("BLOB: " + Convert.ToBase64String(rib.Random.Data[0]));
    Console.WriteLine("Signed integer: " + srii.Random.Data[0]);
    Console.WriteLine("Signed integer (verification): " + vrii);
}
```