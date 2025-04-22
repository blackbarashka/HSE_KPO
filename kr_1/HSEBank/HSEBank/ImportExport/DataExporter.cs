using Facade;

namespace ImportExport
{
    /// <summary>
    /// Экспортер данных, абстрактный класс.
    /// </summary>
    public abstract class DataExporter
    {
        protected FinancialFacade _facade;

        protected DataExporter(FinancialFacade facade)
        {
            _facade = facade;
        }

        public abstract void Export(string filePath);
    }
}
