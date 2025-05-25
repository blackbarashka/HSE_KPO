# Контрольная работа № 2 Синхронное межсервисное взаимодействие. Мусаев Умахан Рашидович БПИ234.


## 1. Общая структура и назначение

Данный репозиторий состоит из нескольких микросервисов и веб-приложений, которые взаимодействуют друг с другом через **Docker Compose**. Основные составляющие:

1. **PostgreSQL (postgres)**  
   - Хранит данные (информация об анализе текста, результаты поиска похожих файлов и т.д.).  
   - Запускается в собственном контейнере, конфигурируется в `docker-compose.yml`.  

2. **FileStorageService (filestore)**  
   - Отвечает за хранение файлов.  
   - Предоставляет API для загрузки, скачивания и просмотра информации о файлах.  
   - Использует собственную базу данных (в том же PostgreSQL, но в отдельной схеме/БД).  

3. **FileAnalysisService (fileanalysis)**  
   - Проводит анализ текста (количество слов, абзацев, символов и т.п.)  
   - Определяет схожесть файлов (по тексту) через алгоритм Левенштейна.  
   - Хранит результаты анализа, предоставляет REST API для запроса этих данных.  
   - Генерирует облако слов (Word Cloud) через сервис `IWordCloudService`. - не реализовано до конца, в связи изменением условия.   

4. **ApiGateway (apigateway)**  
   - Является шлюзом, который объединяет обращения к различным микросервисам по единым конечным точкам.  
   - Использует Ocelot для маршрутизации запросов (конфигурация может находиться в `ocelot.json`).  
   - Открывает порт (например, `8080`) для клиентов, перенаправляя запросы внутрь к нужным сервисам.  

5. **WebApp (webapp)**  
   - Razor Pages-приложение, предоставляющее пользовательский интерфейс.  
   - Отправляет запросы к `apigateway`, чтобы взаимодействовать с `filestore` и `fileanalysis`.  
   - Открывает порт (например, `8081`).  

Все эти сервисы описаны в `docker-compose.yml` и работают вместе при запуске командой `docker-compose up`.

---
## 2. Запуск и настройка через Docker Compose

### 2.1 Предварительные требования

- Установленные Docker и Docker Compose.  
- Свободные порты:  
  - `5432` для PostgreSQL,  
  - `8080`, `8081`, `8082`, `8085` и т.п. для различных сервисов.  

### 2.2 Файл `docker-compose.yml`

В корне репозитория лежит `docker-compose.yml`, где описаны сервисы:

# Запуск
1. Запускаем Docker Desktop.
2. В терминале нашего решения пишем:

`docker-compose up -d`

Для работы программы должна быть база данных, поэтому, если при загрузке файла, программа выводит ошибку:
Все выполнять в терминале решения
3. Установите инструмент dotnet-ef:
   -`dotnet tool install --global dotnet-ef`
4. Команда миграции для двух баз:
-`dotnet ef migrations add AddPendingChanges -p HSE.AntiPlagiat.FileAnalysisService -s HSE.AntiPlagiat.FileAnalysisService`
-`dotnet ef migrations add AddPendingChanges -p HSE.AntiPlagiat.FileStorageService -s HSE.AntiPlagiat.FileStorageService`
5. Последнее:
-`dotnet ef database update -p HSE.AntiPlagiat.FileAnalysisService -s HSE.AntiPlagiat.FileAnalysisService`
-`dotnet ef database update -p HSE.AntiPlagiat.FileStorageService -s HSE.AntiPlagiat.FileStorageService`

6. Сервисы находятся по следующим ссылкам:
   - `http://localhost:8081` — пользовательский интерфейс (Razor Pages в WebApp).  
   - `http://localhost:8080` или `http://localhost:8080\swagger` — API Gateway (при необходимости).  
   - `http://localhost:8082/swagger` — Swagger FileAnalysisService (если конфигурирован).  
   - `http://localhost:8085/swagger` — Swagger FileStorageService (если конфигурирован).
  
   В списке должны присутствовать `postgres`, `filestore`, `fileanalysis`, `apigateway`, `webapp`.


---

## 3. Описание сервисов

### 3.1 FileStorageService

**Назначение:**  
- Хранит загруженные файлы (в папке `/app/Storage` внутри контейнера).  
- Предоставляет API для загрузки (`POST /api/files`), скачивания (`GET /api/files/{id}`) и просмотра списка файлов (`GET /api/files`).  

**Ключевые моменты:**  
- Работает с PostgreSQL для хранения информации о файлах (название, тип, размер, дата загрузки).  
- Сами файлы сохраняются в файловой системе контейнера (volume `file-storage`).  

### 3.2 FileAnalysisService

**Назначение:**  
- Анализирует текст (считает количество слов, абзацев, символов, генерирует облако слов).  
- Находит похожие файлы путём сравнения текстового содержимого (через алгоритм Левенштейна).  
- Возвращает результаты анализа (хранит их в таблицах `AnalysisResults`, `SimilarityResults`).  

**Основные классы и интерфейсы:**  
- `AnalysisController` — контроллер, обрабатывающий запросы на анализ файлов.  
- `TextAnalysisService` — сервис, отвечающий за парсинг текста и логические операции.  
- `SimilarityService` — сервис, вычисляющий процент схожести c помощью алгоритма Левенштейна.  

**API примеры:**  
- `POST /api/analysis/{fileId}` — запускает анализ текста у файла.  
- `GET /api/analysis/{fileId}` — получить сохранённый результат анализа.  
- `POST /api/analysis/compare` — сравнить два файла и получить `SimilarityResult`.  

### 3.3 ApiGateway

**Назначение:**  
- Служит единым входным пунктом, перенаправляя запросы к `filestore` и `fileanalysis`.  
- Конфигурация маршрутов описывается в файле `ocelot.json`.  
- Открывает порт `8080` внешнему миру.  

### 3.4 WebApp (Razor Pages)

**Назначение:**  
- Интерфейс для пользователя (фронтенд).  
- Показывает список файлов, даёт возможность загрузить новый, запустить анализ на плагиат и т.д.  
- Обращается к `apigateway` на `http://apigateway` (из Docker-сети) или `http://localhost:8080` (извне контейнера).  

---

## 4. Работа с базой данных

### 4.1 PostgreSQL volume

- В `docker-compose.yml` прописан `volume: postgres-data:/var/lib/postgresql/data`, где хранятся файлы БД.  
- Чтобы **очистить** базу, можно удалить этот volume:
  
1.`docker-compose down docker volume rm <имя-volume>`

2.`docker-compose up -d`

После этого будет создана новая пустая база.

### 4.2 Применение миграций (если используете EF Core Migrations)

Для **FileAnalysisService**:

1. Перейдите в папку проекта `HSE.AntiPlagiat.FileAnalysisService`.  
2. Сгенерируйте миграцию, если изменялись модели:
 -  `dotnet ef migrations add <MigrationName> -p HSE.AntiPlagiat.FileStorageService -s HSE.AntiPlagiat.FileStorageService`
 -  `dotnet ef migrations add <MigrationName> -p HSE.AntiPlagiat.FileAnalysisService -s HSE.AntiPlagiat.FileAnalysisService`
3. Примените миграцию:
   - `dotnet ef database update -p HSE.AntiPlagiat.FileStorageService -s HSE.AntiPlagiat.FileStorageService`
   - `dotnet ef database update -p HSE.AntiPlagiat.FileAnalysisService -s HSE.AntiPlagiat.FileAnalysisService`

---

## 5. Тестирование

### 5.1 xUnit-тесты

В папке `AnalysisTests` содержатся тестовые классы (пример):

- `AnalysisControllerTests.cs` — проверяет, корректно ли `AnalysisController` возвращает результаты и обрабатывает запросы.  
- `TextAnalysisServiceTests.cs` — проверяет логику подсчёта слов, абзацев, символов, генерацию `WordCloudUrl`.  
- `SimilarityServiceTests.cs` — проверка метода сравнения файлов.  

### 5.2 Запуск тестов

Выполните в корне репозитория:

`dotnet test`



---

## 6. Swagger (автоматическая документация)
При запуске сервиса **FileAnalysisService** на  посмотрите по адресу:
- `http://localhost:8082/swagger`
При запуске сервиса **FileStorageService**:
-`http://localhost:8085/swagger`

`ApiGateway` также имеет свой Swagger, доступный по `http://localhost:8080/swagger`.  

---

## 7. Взаимодействие сервисов

1. **WebApp** -> (через ApiGateway) -> **FileAnalysisService** / **FileStorageService**  
2. **FileAnalysisService** при необходимости запрашивает содержимое файлов через `FileStorageService`.  
3. Все сервисы используют базу PostgreSQL для хранения своих таблиц.

---

## 8. Частые проблемы
- **Отсутствуют таблицы**: возможно, не были применены EF Migrations. Всегда проверяйте с помощью `dotnet ef database update`.  
- **Недоступен Gateway**: проверьте, запущен ли контейнер `apigateway` и не конфликтует ли порт `8080`.  

---

## 9. Заключение

В данной системе микросервисы взаимодействуют между собой для решения задач:
- Хранение исходных файлов (FileStorageService),  
- Анализ текста на плагиат и метрики (FileAnalysisService),  
- Объединение маршрутов (ApiGateway),  
- Предоставление интерфейса пользователю (WebApp),  
- Хранение всех данных в PostgreSQL (Postgres).  

Запускайте всё через `docker-compose up -d`, после чего вы сможете использовать веб-интерфейс по адресу `http://localhost:8081`. Результаты доступны через Swagger или обращениями к API Gateway.

**Спасибо за использование данного проекта!** Если возникнут вопросы или пожелания по улучшению, вносите изменения либо пишите новые issue в репозиторий.


---

# Краткий гайд с картинками по работе программы:
### Основной сайт **WebApp**:

![image](https://github.com/user-attachments/assets/19d23e32-0b16-4956-8bee-14e2d871cf01)

Тут можно загрузить файл и проанализировать его:

![image](https://github.com/user-attachments/assets/fbe0228b-401b-4a77-97ce-89d784823dad)


### FileStorageService Swagger:
![image](https://github.com/user-attachments/assets/d7b384e7-b77c-4f42-befe-18b73e62197b)

Тут можно:
1. Загрузить файл
   
   ![image](https://github.com/user-attachments/assets/b8e9540b-5fbe-45b8-b442-ec6426596fda)

3. Получить данные о всех файлах:

   ![image](https://github.com/user-attachments/assets/7213b481-417f-4ace-96f1-32a3a4a9c8fa)

4. Получить данные о конктретном файле, указав его id.
5. Получить тип информации из конкретного файла.

### FileAnalysisService Swagger:

![image](https://github.com/user-attachments/assets/75ea95da-ed0c-41a0-8c3b-4d4df4894c6b)

Тут можно:
1. Проанализировать файл из базы данных по его id.
   
   ![image](https://github.com/user-attachments/assets/f098fdc2-0836-44b8-9603-1ee52d9de5ca)

3. Получить результат анализа по id файла.
4. Сравнить два файла по их id:
5. 
   ![image](https://github.com/user-attachments/assets/508a2a5b-ca56-43e6-9e37-c1a3935a6700)

   **Тут я провел сравнение почти идентичных файлов и как мы видим нам вывело процент плагиата 99.9**



   
