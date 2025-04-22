using Facade;

namespace ImportExport
{
    /// <summary>
    /// Импортер данных, абстрактный класс.
    /// </summary>
    public abstract class DataImporter
    {
        protected FinancialFacade facade;

        protected DataImporter(FinancialFacade facade)
        {
            this.facade = facade;
        }

        public abstract void Import(string filePath);
    }
}
