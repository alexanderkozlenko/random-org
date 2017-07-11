`RandomOrgClient` supports all [RANDOM.ORG](https://random.org) API methods, available through JSON-RPC protocol. A client instance is created with an API key and optional `HttpMessageInvoker` instance, all methods support operation cancellation via a `CancellationToken`.

```cs
using (var client = new RandomOrgClient("00000000-0000-0000-0000-000000000000"))
{
    // Get current API key's status and available bits

    var usage = await client.GetUsageAsync(CancellationToken.None);

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
    var vrii = await client.VerifySignatureAsync(srii.Random, srii.Signature);

    Console.WriteLine($"Key status: {usage.Status}, bits left: {usage.BitsLeft}");
    Console.WriteLine($"Random integer: {rii.Random.Data[0]}");
    Console.WriteLine($"Random decimal fraction: {rif.Random.Data[0]}");
    Console.WriteLine($"Random Gaussian number: {rig.Random.Data[0]}");
    Console.WriteLine($"Random string: {ris.Random.Data[0]}");
    Console.WriteLine($"Random UUID: {riu.Random.Data[0]}");
    Console.WriteLine($"Random BLOB: {Convert.ToBase64String(rib.Random.Data[0])}");
    Console.WriteLine($"Signed random integer (data): {rii.Random.Data[0]}");
    Console.WriteLine($"Signed random integer (authenticity): {vrii}");
}
```