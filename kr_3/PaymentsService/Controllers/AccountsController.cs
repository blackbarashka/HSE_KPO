using Common.Models;
using Microsoft.AspNetCore.Mvc;
using PaymentsService.Services;

namespace PaymentsService.Controllers
{
    /// <summary>
    /// Контроллер для управления счетами пользователей в сервисе платежей
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountsController> _logger;
        /// <summary>
        /// Конструктор контроллера счетов
        /// </summary>
        /// <param name="accountService"></param>
        /// <param name="logger"></param>
        public AccountsController(IAccountService accountService, ILogger<AccountsController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }
        /// <summary>
        /// Создание нового счета для пользователя
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateAccount(CreateAccountRequest request)
        {
            try
            {
                var account = await _accountService.CreateAccountAsync(request.UserId);
                return Ok(account);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Ошибка при создании счета");
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Пополнение счета пользователя
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("topup")]
        public async Task<IActionResult> TopUp(TopUpAccountRequest request)
        {
            try
            {
                // Логирование для отладки
                _logger.LogInformation($"Попытка пополнить счет: UserId={request.UserId}, Amount={request.Amount}");

                var account = await _accountService.TopUpAccountAsync(request.UserId, request.Amount);
                return Ok(account);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Ошибка при пополнении счета");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Непредвиденная ошибка при пополнении счета");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }
        /// <summary>
        /// Получение баланса счета пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetBalance(string userId)
        {
            try
            {
                var account = await _accountService.GetAccountAsync(userId);
                if (account == null)
                {
                    _logger.LogWarning($"Счет для пользователя {userId} не найден");
                    return NotFound("Account not found");
                }

                return Ok(account);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при получении счета для пользователя {userId}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }
    }
}
