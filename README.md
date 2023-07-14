
# Epiksoft Results

A minimal library to return result object instead of throwing exceptions.


## How to use

The only thing you have to do is invoke the builder methods of the Result class. Builder methods also include summaries.

### Success
```csharp
var productList = new List<Product>();
var result = Result.Success();
var dataResult = Result.Success(productList);
```

### Failure
```csharp
var productList = new List<Product>();
var result = Result.Failure();
var dataResult = Result.Failure<List<Product>>();
```

### Checking the result if succeeded or failed
```csharp
if(result.Succeeded)
{
    // do something if success ...
}

if(result.Failed)
{
    // do something if failure ...
}
```

### With message, error, code and, metadata
```csharp
string code = "test_code";
string message = "test";
string otherMessage = "test1";
var dateTime = DateTime.Now;
string key = "dateTime";
object value = dateTime;

var result = Result.Failure()
    .WithError(message, code)
    .WithError(new ResultError(message, code), new ResultError(message, code))
    .WithError(message, code)
    .WithMessage(otherMessage)
    .WithCode(code)
    .WithMetaData(key, value);
```

## Configuration

You can use service collection extension to configure result object.

### Metadata factory
By configuring a metadata factory, you can automatically set the metadata dictionary after each result is created.
```csharp
builder.Services.AddResultOptions(o =>
{
	o.MetadataFactory = () =>
	{
		var dictionary = new Dictionary<string, object>();

		var serverTime = DateTime.Now;
		var requestId = Guid.NewGuid();

		dictionary.Add(nameof(serverTime), serverTime);
		dictionary.Add(nameof(requestId), requestId);

		return dictionary;
	};
});
```

### Serialization options
You can also modify the default JsonSerializerOptions by utilizing the extension method.
```csharp
builder.Services.AddResultOptions(o =>
{
	o.JsonSerializerOptions = new JsonSerializerOptions
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	};

	o.ReturnNotFoundWhenDataIsNull = true;
});
```


## Converting into IActionResult
By calling ToResponse method you can convert the result into an IActionResult. Also you are able to set the http status code.
```csharp

[HttpGet]
public IActionResult Get()
{
    string message = "test message";
    string code = "test_message_code";
    string data = "Ali";

    var result = Result.Success(data)
        .WithMessage(message)
        .WithCode(code)
        .WithHttpStatusCode(HttpStatusCode.Created);

    return result.ToResponse();
}
```





## License

[MIT](https://choosealicense.com/licenses/mit/)

  