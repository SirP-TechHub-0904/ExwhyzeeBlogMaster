using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelData.Models
{
    internal class WebForms
    {
    }
    public class WebPageForm
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public bool AllowAnonymousResponses { get; set; }
         

        public ICollection<WebPageFormField> Fields { get; set; } = new List<WebPageFormField>();
        public ICollection<WebPageFormResponse> Responses { get; set; } = new List<WebPageFormResponse>();

    }
    public class WebPageFormField
    {
        public int Id { get; set; }
        public int WebPageFormId { get; set; }

        public string Label { get; set; } = null!;
        public string FieldType { get; set; } = "text"; // text, textarea, email, number, dropdown, checkbox
        public bool IsRequired { get; set; }
        public string? Placeholder { get; set; }
        public string? OptionsJson { get; set; } // for dropdown or checkbox group
    }
    public class WebPageFormResponse
    {
        public int Id { get; set; }
        public int WebPageFormId { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public string? UserId { get; set; } // for logged-in users
        public ICollection<WebPageFormFieldValue> FieldValues { get; set; } = new List<WebPageFormFieldValue>();

    }
    public class WebPageFormFieldValue
    {
        public int Id { get; set; }
        public int WebPageFormResponseId { get; set; }
        public int FieldId { get; set; }
        public string Value { get; set; } = null!;
    }
   
}
