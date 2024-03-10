using FluentValidation;
using IsolatedMediatr.Exception;
using IsolatedMediatr.Models;
using IsolatedMediatr.Requests;
using IsolatedMediatr.Validators;
using MediatR;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Nodes;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace IsolatedMediatr.Handlers
{
   public class ProcessQueueMessageHandler : IRequestHandler<QueueMessageRequest, string>
   {
      private readonly string _newLine = Environment.NewLine;
      private AbstractValidator<Person> _personValidator;

      public ProcessQueueMessageHandler(AbstractValidator<Person> personValidator)
      {
         _personValidator = personValidator;
      }

      public async Task<string> Handle(QueueMessageRequest request, CancellationToken cancellationToken)
      {
         var validationJsonResult = ValidateJson(request.QueueMessage);
         if (validationJsonResult != null)
         {
            return await Task.FromResult("[Invalid Json object] " + validationJsonResult);
         }

         var person = JsonConvert.DeserializeObject<Person>(request.QueueMessage);

         var validationResult = await _personValidator.ValidateAsync(person, cancellationToken);
         if (validationResult.Errors.Any())
         {
            return("[Invalid person object] " + _newLine +
                    GetValidationErrors(validationResult) + _newLine +
                    request.QueueMessage);
         }

         return await Task.FromResult(request.QueueMessage);
      }

      private static string GetValidationErrors(ValidationResult validationResult)
      {
         var validationErrors = new StringBuilder();
         foreach (var error in validationResult.Errors)
         {
            validationErrors.Append(error.ErrorMessage);
         }

         return validationErrors.ToString();
      }

      private static string? ValidateJson(string queueItem)
      {
         try
         {
            JsonNode.Parse(queueItem);
         }
         catch (System.Exception e)
         {
            return e.Message;
         }
         return null;
      }
   }
}
