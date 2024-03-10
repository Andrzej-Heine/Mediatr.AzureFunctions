using IsolatedMediatr.Requests;
using IsolatedMediatr.Validators;
using System.Collections;
using IsolatedMediatr.Handlers;

namespace Mediatr.AzureFunctions.Tests
{
   public class ProcessQueueMessageHandlerTests
   {
      private readonly PersonValidator _personValidator;

      public ProcessQueueMessageHandlerTests()
      {
         _personValidator = new PersonValidator();
      }

      [Theory]
      [ClassData(typeof(QueueMessageRequestTestData_InvalidJsonObject))]
      public async Task ProcessQueueMessageHandler_ValidateJson_InvalidJsonObject(QueueMessageRequest queueMessageRequest)
      {
         //Arrange
         var handler = new ProcessQueueMessageHandler(_personValidator);

         //Act
         var result = await handler.Handle(queueMessageRequest, new CancellationToken());

         //Assert
         Assert.StartsWith("[Invalid Json object]", result);
      }

      private class QueueMessageRequestTestData_InvalidJsonObject : IEnumerable<object[]>
      {
         public IEnumerator<object[]> GetEnumerator()
         {
            yield return new object[] { new QueueMessageRequest() { QueueMessage = "" } };
            yield return new object[] { new QueueMessageRequest() { QueueMessage = string.Empty } };
            yield return new object[] { new QueueMessageRequest() { QueueMessage = null } };
         }

         IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
      }

      [Theory]
      [ClassData(typeof(QueueMessageRequestTestData_InvalidPersonObject))]
      public async Task ProcessQueueMessageHandler_PersonValidator_InvalidPersonObject(QueueMessageRequest queueMessageRequest)
      {
         //Arrange
         var handler = new ProcessQueueMessageHandler(_personValidator);

         //Act
         var result = await handler.Handle(queueMessageRequest, new CancellationToken());

         //Assert
         Assert.StartsWith("[Invalid person object]", result);
      }

      private class QueueMessageRequestTestData_InvalidPersonObject : IEnumerable<object[]>
      {
         public IEnumerator<object[]> GetEnumerator()
         {
            yield return new object[] { new QueueMessageRequest() { QueueMessage = "{}" } };
            yield return new object[] { new QueueMessageRequest() { QueueMessage = "{ \"Name\" : \"Andrzej Heine\"} " } };
            yield return new object[] { new QueueMessageRequest() { QueueMessage = "{ \"Namev2\" : \"Andrzej Heine\"} " } };
            yield return new object[] { new QueueMessageRequest() { QueueMessage = "{ \"Email\" : \"Andrzej.Heine@gmail.com  \"} " } };
            yield return new object[] { new QueueMessageRequest() { QueueMessage = "{ \"Emailv2\" : \"Andrzej.Heine@gmail.com  \"} " } };
         }

         IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
      }
   }
}
