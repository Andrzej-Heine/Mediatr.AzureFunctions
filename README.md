
# Mediatr.AzureFunctions

.NET-based Azure Functions application that implements mediator pattern.


## Installation



- Microsoft Azure Storage Explorer https://azure.microsoft.com/en-us/products/storage/storage-explorer
- Postman https://www.postman.com/
- Install Visual Studio 2022 https://visualstudio.microsoft.com/pl/downloads/
- Azurite is automatically available with Visual Studio 2022. The Azurite executable is updated as part of Visual Studio new version releases. If you're running an earlier version of Visual Studio, you can install Azurite by using Node Package Manager (npm)
    
## Running Azure Functions locally in Visual Studio 2022
- Open our Azure Function Visual Studio project
- Select the option Debug -> Start Debugging or simply press F5
- This action starts the Azure Functions runtime locally. If a Windows Defender Firewall appears, configure the necessary allow func to communicate options and click Allow access.
- We have created an Azure Function triggered by HTTP, our functions will be available at http://localhost:port/function-name
For example: http://localhost:7118/api/ListBlobs

## Configuration

In `local.settings.json` they are defined:
- `"BlobContainerName": "azure-blobs"` - BLOB container name 
- `"QueueName": "azure-input-queue"` - Queue name

In Microsoft Azure Storage Explorer you need to create a BLOB container called `"azure-blobs"` and Queue called `azure-input-queue`.
We'll need them for testing.


## Libraries and Frameworks

- MediatR - handling requests and responses within azure functions.
- Dependency injection - to manage services and dependencies.
- FluentValidation - a .NET library for building strongly-typed validation rules.
## ProcessQueueMessageFunction - Queue-Triggered Function
This function will be triggered by new messages in Azure Storage Queue called `azure-input-queue`.

```csharp
    [Function("ProcessQueueMessageFunction")]
    [BlobOutput("%BlobContainerName%/{datetime:yyyy}/{datetime:MM}/{datetime:dd}/{datetime:HH}/{datetime:mm}/{rand-guid}.json", Connection = "AzureWebJobsStorage")]
    public string Run([QueueTrigger("%QueueName%", Connection = "AzureWebJobsStorage")] string queueMessage)
    {
        try
        {
            _logger.LogInformation($"C# Queue trigger function processed: {queueMessage}");
            return _mediator.Send(new QueueMessageRequest { QueueMessage = queueMessage }).Result;
        }
        catch (System.Exception ex)
        {
            _logger.LogInformation($"[GetListBlobFunction]: {ex}");
            return ex.Message;
        }
    }
```

We assume that each message will contain a valid JSON object.
I made the following assumptions:
- each message will contain 1 Person object
- a valid JSON object means it will have the following structure:
```
{
	"Name": "Andrzej Heine",
	"Email": "andrzej.heine@gmail.com"
}
```

The file path in Blob Storage follows the pattern: `"%BlobContainerName%/{datetime:yyyy}/{datetime:MM}/{datetime:dd}/{datetime:HH}/{datetime:mm}/{rand-guid}.json"`

- `%BlobContainerName%` - The name of the Blob Storage container is taken from `local.settings.json`
- `"%QueueName%"` - The name of the Azure Storage Queue is taken from `local.settings.json`


The following validations have been defined for the Person object in the directory named Validations:
```csharp
    public class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator()
        {
            RuleFor(x => x.Name).NotEmpty().NotNull().WithMessage("Name is required.");
            RuleFor(x => x.Email).NotEmpty().NotNull().WithMessage("Email is required.");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Enter a valid Email Address.");
        }
    }
```

If the `Name` or `Email` address is blank or the `Email` address is in the wrong format they will be thrown `BadRequestException`.
To make the function testable, the error will be written to a file.

Library was used for validation
called FluentValidation https://docs.fluentvalidation.net/en/latest/
## GetListBlobFunction - HTTP-Triggered Function

This function responds to HTTP GET requests by returning a list of all files currently stored in the Blob Storage container.

```csharp
        [Function("GetListBlobFunction")]
        public async Task<HttpResponseData> GetListBlobs([HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "ListBlobs")]
            HttpRequestData request)
        {
            try
            {
                _logger.LogInformation("GetListBlobFunction");
                var blobs = await _mediator.Send(new GetBlobsRequest());
                var response = request.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(blobs);
                return response;
            }
            catch (System.Exception ex)
            {
                _logger.LogInformation($"[GetListBlobFunction]: {ex}");
                var response = request.CreateResponse(HttpStatusCode.InternalServerError);
                await response.WriteStringAsync(ex.Message);
                return response;
            }
        }
```

The response format is a JSON array containing filenames, file paths and size in byte.

For example:
```csharp
[
    {
        "FilePath": "2024/02/25/22/38",
        "FileName": "0bd2c5d5-1220-4cdd-90d4-df7372c8b77d.json",
        "Size": 1474
    },
    {
        "FilePath": "2024/02/25/22/38",
        "FileName": "b163db49-2d03-45ae-b62e-0d0b4840409d.json",
        "Size": 1474
    }
]
```

## xUnit Test project
In project named `Mediatr.AzureFunctions.Tests` there are `PersonValidatorTests` class with tests for for `Person` Class.
```csharp
namespace IsolatedMediatr.Models
{
    public class Person
    {
        public string Name;
        public string Email;
    }
}
```

The following validator is defined:
```csharp
namespace IsolatedMediatr.Validators
{
    public class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator()
        {
            RuleFor(x => x.Name).NotEmpty().NotNull().WithMessage("Name is required.");
            RuleFor(x => x.Email).NotEmpty().NotNull().WithMessage("Email is required.");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Enter a valid Email Address.");
        }
    }
}
```

The purpose of our tests is to check whether the defined validation meets our expectations.

FluentValidation provides some extensions that helped in testing the validator class.
The validator was treated as a "black box" - input data was passed to it, and then it was assessed whether the validation results were correct or incorrect.

Using xUnit we want to ensure that this validator is working correctly by writing the following tests:

```csharp
   public class PersonValidatorTests
   {
      private readonly PersonValidator _validator;

      public PersonValidatorTests()
      {
         _validator = new PersonValidator();
      }

      [Theory]
      [ClassData(typeof(PersonNameTestData))]
      public void Should_have_error_when_name_is_empty(Person recipeName)
      {
         var result = _validator.TestValidate(recipeName);
         result.ShouldHaveValidationErrorFor(person => person.Name);
      }

      [Theory]
      [ClassData(typeof(PersonEmailTestData))]
      public void Should_have_error_when_email_is_empty_or_not_valid(Person recipeName)
      {
         var result = _validator.TestValidate(recipeName);
         result.ShouldHaveValidationErrorFor(person => person.Email);
      }
}
```
If the assertion fails, then a 'ValidationTestException' will be thrown.

To test a larger number of cases, private classes were defined which use the `ClassData` attribute to inject test cases into the defined tests.
```csharp
      private class PersonNameTestData : IEnumerable<object[]>
      {
         public IEnumerator<object[]> GetEnumerator()
         {
            yield return new object[] { new Person() { Name = "" } };
            yield return new object[] { new Person() { Name = string.Empty } };
            yield return new object[] { new Person() { Name = null } };
         }

         IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
      }

      private class PersonEmailTestData : IEnumerable<object[]>
      {
         public IEnumerator<object[]> GetEnumerator()
         {
            yield return new object[] { new Person() { Email = "" } };
            yield return new object[] { new Person() { Email = string.Empty } };
            yield return new object[] { new Person() { Email = null } };
            yield return new object[] { new Person() { Email = "Andrzej.Heine" } };
            yield return new object[] { new Person() { Email = "@gmail" } };
            yield return new object[] { new Person() { Email = "@gmail.com" } };
            yield return new object[] { new Person() { Email = "Andrzej.Heinegmail.com" } };

         }

         IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
      }
   }
```


On the other hand, we want to make sure that if the input data is correct, the validator will not report an error. For this purpose, we provide the correct data set as input.
```csharp
      [Fact]
      public void Should_not_have_error_when_name_is_specified()
      {
         var person = new Person { Name = "Andrzej Heine" };
         var result = _validator.TestValidate(person);
         result.ShouldNotHaveValidationErrorFor(p => p.Name);
      }

      [Fact]
      public void Should_not_have_error_when_email_is_specified()
      {
         var person = new Person { Email = "Andrzej.Heine@gmail.com" };
         var result = _validator.TestValidate(person);
         result.ShouldNotHaveValidationErrorFor(p => p.Email);
      }
```
