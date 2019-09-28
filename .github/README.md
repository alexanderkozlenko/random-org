# Anemonis.RandomOrg

[RANDOM.ORG](https://www.random.org) service client based on [Core API (Release 2)](https://api.random.org/json-rpc/2).

| [![](https://img.shields.io/gitter/room/nwjs/nw.js.svg?style=flat-square)](https://gitter.im/anemonis/random-org) | Release | Current |
|---|---|---|
| Artifacts | [![](https://img.shields.io/nuget/vpre/Anemonis.RandomOrg.svg?style=flat-square)](https://www.nuget.org/packages/Anemonis.RandomOrg) | [![](https://img.shields.io/myget/alexanderkozlenko/vpre/Anemonis.RandomOrg.svg?label=myget&style=flat-square)](https://www.myget.org/feed/alexanderkozlenko/package/nuget/Anemonis.RandomOrg) |
| Code Health | | [![](https://img.shields.io/sonar/coverage/random-org?format=long&server=https%3A%2F%2Fsonarcloud.io&style=flat-square)](https://sonarcloud.io/component_measures?id=random-org&metric=coverage&view=list) [![](https://img.shields.io/sonar/violations/random-org?format=long&server=https%3A%2F%2Fsonarcloud.io&style=flat-square)](https://sonarcloud.io/project/issues?id=random-org&resolved=false) |
| Build Status | | [![](https://img.shields.io/azure-devops/build/alexanderkozlenko/github-pipelines/3?label=master&style=flat-square)](https://dev.azure.com/alexanderkozlenko/github-pipelines/_build?definitionId=3&_a=summary) |

## Project Details

| Category | Method | Support |
| :---: | --- | :---: |
| Basic | `getUsage` | Yes |
| Basic | `generateIntegers` | Yes |
| Basic | `generateIntegerSequences` | Yes |
| Basic | `generateDecimalFractions` | Yes |
| Basic | `generateGaussians` | Yes |
| Basic | `generateStrings` | Yes |
| Basic | `generateUUIDs` | Yes |
| Basic | `generateBlobs` | Yes |
| Signed | `generateSignedIntegers` | Yes |
| Signed | `generateSignedIntegerSequences` | Yes |
| Signed | `generateSignedDecimalFractions` | Yes |
| Signed | `generateSignedGaussians` | Yes |
| Signed | `generateSignedStrings` | Yes |
| Signed | `generateSignedUUIDs` | Yes |
| Signed | `generateSignedBlobs` | Yes |
| Signed | `getResult` | No |
| Signed | `verifySignature` | Yes |

- The client supports usage of a custom HTTP message invoker.
- The client supports only `string` as the type for an optional user data parameter.
- The client uses only `base64` as a BLOB format in JSON.
- The client uses only `10` as a base for integers in JSON.

## Code Examples

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

## Quicklinks

- [Contributing Guidelines](./CONTRIBUTING.md)
- [Code of Conduct](./CODE_OF_CONDUCT.md)
