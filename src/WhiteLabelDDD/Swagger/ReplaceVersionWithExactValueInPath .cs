using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace WhiteLabel.WebAPI.Swagger
{
    public class ReplaceVersionWithExactValueInPath : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var paths = new Dictionary<string, OpenApiPathItem>(swaggerDoc.Paths);
            swaggerDoc.Paths.Clear();
            foreach (var path in paths)
            {
                var key = path.Key.Replace("{version}", swaggerDoc.Info.Version);
                var value = path.Value;
                swaggerDoc.Paths.Add(key, value);

            }
        }
    }
}
