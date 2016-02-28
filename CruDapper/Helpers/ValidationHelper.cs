using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CruDapper.Infrastructure;

namespace CruDapper.Helpers
{
    public static class ValidationHelper
    {
        public static void ValidateModel(object model)
        {
            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);

            if (isValid == false)
            {
                throw new Exception("Model is invalid: " +
                                    string.Join(", ", results.Select(s => s.ErrorMessage).ToArray()));
            }
        }
        
        public static bool VerifyIDeletable<T>()
        {
            if (!typeof(IDeletable).IsAssignableFrom(typeof(T)))
                return false;
            return true;
        }

        public static void ValidateList<T>(ref IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                ValidationHelper.ValidateModel(entity);
            }
        }
    }
}