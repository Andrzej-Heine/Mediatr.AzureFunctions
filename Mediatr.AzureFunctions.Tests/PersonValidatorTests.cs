using FluentValidation.TestHelper;
using IsolatedMediatr.Models;
using IsolatedMediatr.Validators;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace Mediatr.AzureFunctions.Tests
{
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
}