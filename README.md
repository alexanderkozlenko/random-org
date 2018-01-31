## Community.RandomOrg

[RANDOM.ORG](https://www.random.org) service client based on [RANDOM.ORG API v2](https://api.random.org/json-rpc/2) (Core APIs).

[![NuGet package](https://img.shields.io/nuget/v/Community.RandomOrg.svg?style=flat-square)](https://www.nuget.org/packages/Community.RandomOrg)

```cs
using (var client = new RandomOrgClient("YOUR_API_KEY_HERE"))
{
    // Generate an integer from the [0,10] range w/o replacement
    var bin = await client.GenerateIntegersAsync(1, 0, 10, false);
    // Generate a decimal fraction with 8 decimal places w/o replacement
    var bdf = await client.GenerateDecimalFractionsAsync(1, 8, false);
    // Generate a number from a Gaussian distribution with mean of 0.0,
    // standard deviation of 1.0, and 8 significant digits
    var bgs = await client.GenerateGaussiansAsync(1, 0.0m, 1.0m, 8);
    // Generate a string with length of 8 from the specified letters w/o replacement
    var bst = await client.GenerateStringsAsync(1, 8, "abcdef", false);
    // Generate an UUID of version 4
    var bud = await client.GenerateUuidsAsync(1);
    // Generate a BLOB with length of 8 bytes
    var bbl = await client.GenerateBlobsAsync(1, 8);
    // Each generation method has a corresponding one for generating random data
    // with signature, which can be verified afterwards
    var sin = await client.GenerateSignedIntegersAsync(1, 0, 10, false);
    var ain = await client.VerifySignatureAsync(sin.Random, sin.Signature);

    Console.WriteLine("Random integer: " + bin.Random.Data[0]);
    Console.WriteLine("Random decimal fraction: " + bdf.Random.Data[0]);
    Console.WriteLine("Random Gaussian number: " + bgs.Random.Data[0]);
    Console.WriteLine("Random string: " + bst.Random.Data[0]);
    Console.WriteLine("Random UUID: " + bud.Random.Data[0]);
    Console.WriteLine("Random BLOB: " + Convert.ToBase64String(bbl.Random.Data[0]));
    Console.WriteLine("Signed data is authentic: " + ain);
}
```

- Signed data verification doesn't require an API key, and thus an empty UUID can be used as API key for this case as well.
- The client respects an advisory delay between generation requests from the server. However, guarantees that it will take no longer than 24 hours.

### Features

- The client has an ability to use a custom `HttpMessageInvoker` instanse to do HTTP requests. A custom message invoker must have at least 2 minutes timeout for a request according to RANDOM.ORG API requirements.
- All operations support cancellation via `CancellationToken`.

### Limitations

- API key usage information doesn't contain information about key creation time and total count of used bits and requests
- Generation and verification of integer sequences support only vector parameters
- A method for retrieving previously generated results is not supported (Signed API)
- `string` is the only supported type for user data optional parameter (Signed API)
- `base64` is the only supported format for BLOBs representation in JSON
- `10` is the only supported base for integers representation in JSON