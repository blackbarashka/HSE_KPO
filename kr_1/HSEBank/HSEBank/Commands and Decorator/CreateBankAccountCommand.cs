using Facade;

namespace Commands
{
    /// <summary>
    /// Команда создания банковского счета
    /// </summary>
    public class CreateBankAccountCommand : ICommand
    {
        private readonly FinancialFacade _facade;
        private readonly string _name;
        private readonly decimal _balance;

        public CreateBankAccountCommand(FinancialFacade facade, string name, decimal balance)
        {
            _facade = facade;
            _name = name;

            _balance = balance;
        }
        /// <summary>
        /// Выполнение команды.
        /// </summary>
        public void Execute()
        {
            _facade.CreateBankAccount(_name, _balance);
        }
    }
}
