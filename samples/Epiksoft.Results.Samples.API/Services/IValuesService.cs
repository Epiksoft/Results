using Epiksoft.Results.Samples.API.Models;

namespace Epiksoft.Results.Samples.API.Services;

public interface IValuesService
{
    Result CheckValue(int value);
    Result CheckValueData(int value);
}
