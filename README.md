## Community.RandomOrg

A client for the [RANDOM.ORG](https://www.random.org) service ([Core API v2](https://api.random.org/json-rpc/2)).

[![NuGet package](https://img.shields.io/nuget/v/Community.RandomOrg.svg?style=flat-square)](https://www.nuget.org/packages/Community.RandomOrg)

```cs
using (var client = new RandomOrgClient("YOUR_API_KEY_HERE"))
{
    // Generate an integer from the [0,10] range without replacement
    var bin = await client.GenerateIntegersAsync(1, 0, 10, false);
    // Generate a decimal fraction with 8 decimal places without replacement
    var bdf = await client.GenerateDecimalFractionsAsync(1, 8, false);
    // Generate a number from a Gaussian distribution with mean of 0.0,
    // standard deviation of 1.0, and 8 significant digits
    var bgs = await client.GenerateGaussiansAsync(1, 0.0m, 1.0m, 8);
    // Generate a string with length of 5 from the specified letters
    var bst = await client.GenerateStringsAsync(1, 5, "abcde", false);
    // Generate an UUID
    var bud = await client.GenerateUuidsAsync(1);
    // Generate a BLOB with length of 8 bytes (64 bits)
    var bbl = await client.GenerateBlobsAsync(1, 64);
    // Each generation method has a corresponding one for generating random data
    // with signature, which can be verified afterwards
    var sin = await client.GenerateSignedIntegersAsync(1, 0, 10, false);
    // Signature verification can be executed without specifying an API key
    var vin = await client.VerifySignatureAsync(sin.Random, sin.Signature);

    Console.WriteLine("Random integer: " + bin.Random.Data[0]);
    Console.WriteLine("Random decimal fraction: " + bdf.Random.Data[0]);
    Console.WriteLine("Random gaussian number: " + bgs.Random.Data[0]);
    Console.WriteLine("Random string: " + bst.Random.Data[0]);
    Console.WriteLine("Random UUID: " + bud.Random.Data[0]);
    Console.WriteLine("Random BLOB: " + Convert.ToBase64String(bbl.Random.Data[0]));
    Console.WriteLine("Signed data verification: " + vin);
}
```

### Limitations

- `string` is the only supported type for user data optional parameter (Signed API)
- `getResult` method is not supported (Signed API)
- `base64` is the only supported format for BLOBs representation in JSON
- `10` is the only supported base for integers representation in JSON