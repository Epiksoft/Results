
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
## License

[MIT](https://choosealicense.com/licenses/mit/)

  