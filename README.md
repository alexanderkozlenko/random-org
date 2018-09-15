## Anemonis.RandomOrg

[RANDOM.ORG](https://www.random.org) service client based on [RANDOM.ORG API v2](https://api.random.org/json-rpc/2) (Core API).

[![NuGet package](https://img.shields.io/nuget/v/Anemonis.RandomOrg.svg?style=flat-square)](https://www.nuget.org/packages/Anemonis.RandomOrg)

Category | Method | Support
:---: | --- | :---:
Basic | `getUsage` | Yes
Basic | `generateIntegers` | Yes
Basic | `generateIntegerSequences` | Yes
Basic | `generateDecimalFractions` | Yes
Basic | `generateGaussians` | Yes
Basic | `generateStrings` | Yes
Basic | `generateUUIDs` | Yes
Basic | `generateBlobs` | Yes
Signed | `generateSignedIntegers` | Yes
Signed | `generateSignedIntegerSequences` | Yes
Signed | `generateSignedDecimalFractions` | Yes
Signed | `generateSignedGaussians` | Yes
Signed | `generateSignedStrings` | Yes
Signed | `generateSignedUUIDs` | Yes
Signed | `generateSignedBlobs` | Yes
Signed | `getResult` | No
Signed | `verifySignature` | Yes

### Important Features

- The client supports operation cancellation via cancellation token.
- The client supports usage of a custom HTTP message invoker.

### Characteristics

- Signed data verification can be invoked with the anonymous key (the empty UUID).
- The client respects service advisory delay between generation requests not longer than `24` hours.
- A custom message invoker must have at least `2`-minute timeout for a request due to service requirements.

### Limitations

- API key usage information does not contain information about key creation time and total count of used bits / requests.
- Generation and verification of integer sequences support only vector parameters.
- The `string` is the only supported type for the optional user data parameter.
- The `base64` is the only supported format for BLOBs in JSON.
- The `10` is the only supported base for integers in JSON.

### Usage Examples

```cs
var client = new RandomOrgClient("YOUR_API_KEY_HERE");

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
// Generate an integer from the [0,10] range w/o replacement with signature
var sin = await client.GenerateSignedIntegersAsync(1, 0, 10, false);
// Verify the signature of the previously generated random integer
var ain = await client.VerifySignatureAsync(sin.Random, sin.Signature);
// Get usage information of the current API key
var usg = await client.GetUsageAsync();

Console.WriteLine("Random integer: " + bin.Random.Data[0]);
Console.WriteLine("Random decimal fraction: " + bdf.Random.Data[0]);
Console.WriteLine("Random Gaussian number: " + bgs.Random.Data[0]);
Console.WriteLine("Random string: " + bst.Random.Data[0]);
Console.WriteLine("Random UUID: " + bud.Random.Data[0]);
Console.WriteLine("Random BLOB: " + Convert.ToBase64String(bbl.Random.Data[0]));
Console.WriteLine("Signed data is authentic: " + ain);
Console.WriteLine("Daily quota requests left: " + usg.RequestsLeft);
```