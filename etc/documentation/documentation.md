### Sample of usage

```cs
using (var client = new RandomOrgClient("00000000-0000-0000-0000-000000000000"))
{
    var uuids = await client.GenerateUuidsAsync(1);

    Console.WriteLine($"Random UUID: {uuids.Random.Data[0]}");
}
```