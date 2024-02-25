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

        public async Task<string> Handle(QueueMessageRequest request, CancellationToken cancellationToken)
        {
            ValidateJson(request.QueueMessage);
            var person = JsonConvert.DeserializeObject<Person>(request.QueueMessage);

            var validator = new PersonValidator();
            var validationResult = await validator.ValidateAsync(person, cancellationToken);

            if (validationResult.Errors.Any())
            {
                throw new BadRequestException("Invalid person object." + _newLine +
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

        private static void ValidateJson(string queueItem)
        {
            try
            {
                JsonNode.Parse(queueItem);
            }
            catch (FormatException fe)
            {
                throw new BadRequestException($"Invalid json format: {fe}");
            }
#pragma warning disable IDE0059
            catch (System.Exception e)
#pragma warning restore IDE0059
            {
                throw new BadRequestException($"[ValidateJson]: {e}");
            }
        }
    }
}
