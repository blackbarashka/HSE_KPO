// Функция для подключения к SignalR хабу и обработки уведомлений
function connectToOrdersHub(userId) {
    // Создаем подключение к хабу через API Gateway
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/ordershub") // API Gateway проксирует на OrdersService
        .withAutomaticReconnect()
        .build();

    // Обработчик события изменения статуса заказа
    connection.on("OrderStatusChanged", function (order) {
        console.log("Получено уведомление об изменении статуса заказа:", order);

        // Показываем уведомление
        showToast(`Заказ ${order.id}: статус изменен на ${getStatusText(order.status)}`,
            order.status === "Finished" ? "success" :
                order.status === "Cancelled" ? "error" : "info");

        // Если пользователь находится на странице заказов - обновляем UI
        if (window.updateOrderStatus) {
            window.updateOrderStatus(order);
        }
    });

    // Запускаем подключение
    connection.start()
        .then(function () {
            console.log("SignalR подключение установлено");
            // Подписываемся на обновления для конкретного пользователя
            return connection.invoke("SubscribeToOrderUpdates", userId);
        })
        .then(function () {
            console.log(`Подписка на обновления для пользователя ${userId} установлена`);
        })
        .catch(function (err) {
            console.error("Ошибка подключения к SignalR:", err);
            // Попробуем переподключиться через 5 секунд
            setTimeout(() => connectToOrdersHub(userId), 5000);
        });

    return connection;
}

// Функция для отображения всплывающих уведомлений
function showToast(message, type) {
    // Создаем элемент для уведомления
    const toast = document.createElement("div");
    toast.className = `toast toast-${type || 'info'}`;
    toast.style.position = "fixed";
    toast.style.top = "20px";
    toast.style.right = "20px";
    toast.style.minWidth = "300px";
    toast.style.backgroundColor = "white";
    toast.style.padding = "15px";
    toast.style.borderRadius = "4px";
    toast.style.boxShadow = "0 4px 8px rgba(0,0,0,0.2)";
    toast.style.zIndex = "9999";
    toast.style.animation = "fadeIn 0.5s, fadeOut 0.5s 4.5s";

    // Устанавливаем цвет границы в зависимости от типа уведомления
    if (type === "success") {
        toast.style.borderLeft = "5px solid #28a745";
    } else if (type === "error") {
        toast.style.borderLeft = "5px solid #dc3545";
    } else {
        toast.style.borderLeft = "5px solid #17a2b8";
    }

    // Добавляем содержимое
    toast.innerHTML = `
        <div style="display:flex;justify-content:space-between;align-items:center">
            <strong>${message}</strong>
            <button style="background:transparent;border:none;font-size:16px;cursor:pointer" 
                    onclick="this.parentElement.parentElement.remove()">×</button>
        </div>
    `;

    // Добавляем на страницу
    document.body.appendChild(toast);

    // Удаляем через 5 секунд
    setTimeout(() => {
        if (toast.parentElement) {
            toast.remove();
        }
    }, 5000);
}

// Вспомогательная функция для получения русского названия статуса
function getStatusText(status) {
    switch (status) {
        case "New": return "Новый";
        case "Processing": return "В обработке";
        case "Finished": return "Выполнен";
        case "Cancelled": return "Отменен";
        default: return status;
    }
}

// Добавляем стили для анимации
document.head.insertAdjacentHTML('beforeend', `
<style>
@keyframes fadeIn {
    from { opacity: 0; transform: translateY(-20px); }
    to { opacity: 1; transform: translateY(0); }
}
@keyframes fadeOut {
    from { opacity: 1; transform: translateY(0); }
    to { opacity: 0; transform: translateY(-20px); }
}
</style>
`);

// Инициализируем подключение при загрузке страницы
document.addEventListener("DOMContentLoaded", function () {
    // Получаем ID пользователя (в реальном приложении из авторизации)
    const userId = "user-123";

    if (userId) {
        // Подключаемся к хабу и сохраняем подключение
        window.hubConnection = connectToOrdersHub(userId);
    }
});