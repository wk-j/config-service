using System.Collections.Generic;
using System.Threading.Tasks;
using NJsonSchema;
using NJsonSchema.Validation;
using ConfigEditor.Models;

namespace ConfigEditor {
    public class Validator {
        public static async Task<ICollection<ValidationError>> Schemacheck(string jsonData) {
            var schema = await JsonSchema4.FromTypeAsync<AppSettings>();
            var errors = schema.Validate(jsonData);
            return errors;
        }
    }
}