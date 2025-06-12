using Microsoft.OpenApi.Models;

namespace ApiGateway.Extensions
{
    /// <summary>
    /// Расширения для Swagger, позволяющие добавлять операции в документацию.
    /// </summary>
    public static class SwaggerExtensions
    {
        /// <summary>
        /// Добавляет операцию в Swagger документацию.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="path"></param>
        /// <param name="operationType"></param>
        /// <param name="operation"></param>
        public static void AddOperation(this Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options,
            string path,
            OperationType operationType,
            OpenApiOperation operation)
        {
            options.DocumentFilter<SwaggerAddOperationFilter>(path, operationType, operation);
        }
    }
    /// <summary>
    /// Класс для добавления операции в Swagger документацию.
    /// </summary>
    public class SwaggerAddOperationFilter : Swashbuckle.AspNetCore.SwaggerGen.IDocumentFilter
    {
        private readonly string _path;
        private readonly OperationType _operationType;
        private readonly OpenApiOperation _operation;
        /// <summary>
        /// Инициализирует новый экземпляр класса SwaggerAddOperationFilter.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="operationType"></param>
        /// <param name="operation"></param>
        public SwaggerAddOperationFilter(string path, OperationType operationType, OpenApiOperation operation)
        {
            _path = path;
            _operationType = operationType;
            _operation = operation;
        }
        /// <summary>
        /// Применяет изменения к Swagger документации.
        /// </summary>
        /// <param name="swaggerDoc"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiDocument swaggerDoc, Swashbuckle.AspNetCore.SwaggerGen.DocumentFilterContext context)
        {
            if (!swaggerDoc.Paths.ContainsKey(_path))
            {
                swaggerDoc.Paths.Add(_path, new OpenApiPathItem());
            }

            swaggerDoc.Paths[_path].Operations[_operationType] = _operation;
        }
    }
}
