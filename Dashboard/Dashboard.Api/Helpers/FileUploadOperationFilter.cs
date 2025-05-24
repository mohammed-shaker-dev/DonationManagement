using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Dashboard.Api
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasFileParameter = context.MethodInfo.GetParameters()
                .Any(p => p.ParameterType == typeof(IFormFile) ||
                         p.ParameterType == typeof(IFormFile[]) ||
                         p.ParameterType == typeof(IEnumerable<IFormFile>));

            if (!hasFileParameter) return;

            // Clear existing parameters for file upload endpoints
            operation.Parameters?.Clear();

            // Set the request body for file upload
            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["file"] = new OpenApiSchema
                                {
                                    Type = "string",
                                    Format = "binary",
                                    Description = "Upload file"
                                }
                            },
                            Required = new HashSet<string> { "file" }
                        }
                    }
                }
            };

            // Ensure the operation consumes multipart/form-data
            operation.RequestBody.Required = true;
        }
    }
}