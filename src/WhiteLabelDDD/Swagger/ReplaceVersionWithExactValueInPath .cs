using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace WhiteLabel.WebAPI.Swagger
{
    public abstract class ReplaceVersionWithExactValueInPath : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var paths = new Dictionary<string, OpenApiPathItem>(swaggerDoc.Paths);
            swaggerDoc.Paths.Clear();
            foreach (var (s, value) in paths)
            {
                var key = s.Replace("{version}", swaggerDoc.Info.Version);
                swaggerDoc.Paths.Add(key, value);
            }
        }
    }
}
