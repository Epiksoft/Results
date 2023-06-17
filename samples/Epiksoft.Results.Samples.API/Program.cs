using Epiksoft.Results.Samples.API.Services;
using Epiksoft.Results;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IValuesService, ValuesManager>();

builder.Services.AddResultOptions(o =>
{
	o.JsonSerializerOptions = new JsonSerializerOptions
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	};

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
