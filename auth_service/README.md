# Auth Service

Сервис авторизации и аутентификации для платформы такси. Написан на **.NET 10** с использованием минимального API.

## 📋 Обзор

Микросервис предоставляет функционал регистрации, входа, управления сессиями и OAuth-аутентификации для пассажиров, водителей и администраторов системы.

## 🛠 Технологический стек

| Компонент | Технология |
|-----------|------------|
| Фреймворк | ASP.NET Core 10 |
| База данных | PostgreSQL 17 |
| Кэш | Redis 8.4 |
| ORM | Entity Framework Core 10 |
| Хеширование паролей | BCrypt.Net |
| JWT токены | System.IdentityModel.Tokens.Jwt |

## 📁 Структура проекта

```
AuthService/
├── src/
│   ├── Clients/          # OAuth-клиенты (Google, Apple)
│   ├── Controllers/      # API контроллеры
│   │   ├── AuthController.cs
│   │   └── OAuthController.cs
│   ├── Data/             # DbContext
│   ├── DTOs/             # Объекты передачи данных
│   ├── Enums/            # Перечисления (Role)
│   ├── Models/           # Доменные модели
│   ├── Repositories/     # Репозитории (DB + Cache)
│   ├── Services/         # Бизнес-логика
│   └── Utils/            # Утилиты (генераторы токенов)
├── Tests/
│   └── UnitTests/        # Модульные тесты
└── Migrations/           # EF Core миграции
```

## 🚀 Запуск

### Через Docker Compose

```bash
docker-compose up -d
```

Запускает:
- **PostgreSQL** на порту `5432`
- **Redis** на порту `6379`

### Переменные окружения

Необходимо настроить в `appsettings.Development.json` или через User Secrets:

```json
{
  "ConnectionStrings": {
    "PostgresConnectionString": "Host=localhost;Port=5432;Database=taxi-auth-db;Username=postgres;Password=postgres",
    "RedisConnectionString": "localhost:6379"
  },
  "Jwt": {
    "SecretKey": "your-secret-key-here",
    "ExpirationMinutes": 60
  },
  "Client": {
    "Url": "http://localhost:3000"
  },
  "OAuth": {
    "GoogleClientId": "...",
    "GoogleClientSecret": "...",
    "AppleClientId": "...",
    "AppleTeamId": "...",
    "AppleKeyId": "...",
    "ApplePrivateKey": "..."
  }
}
```

## 📡 API Endpoints

### Регистрация и аутентификация

| Метод | Endpoint | Описание | Статус |
|-------|----------|----------|--------|
| POST | `/api/v1/auth/register` | Регистрация нового пользователя | ✅ Реализовано |
| POST | `/api/v1/auth/login` | Вход по email/паролю | ⏳ В разработке |
| POST | `/api/v1/auth/logout` | Выход из системы | ⏳ В разработке |
| POST | `/api/v1/auth/refresh` | Обновление access-токена | ⏳ В разработке |

### Управление email

| Метод | Endpoint | Описание | Статус |
|-------|----------|----------|--------|
| POST | `/api/v1/auth/email/send-verification-code` | Отправка кода подтверждения | ⏳ В разработке |
| POST | `/api/v1/auth/email/verify` | Подтверждение email | ⏳ В разработке |

### Безопасность

| Метод | Endpoint | Описание | Статус |
|-------|----------|----------|--------|
| PATCH | `/api/v1/auth/change-password` | Смена пароля | ⏳ В разработке |

### OAuth

| Метод | Endpoint | Описание | Статус |
|-------|----------|----------|--------|
| GET | `/api/v1/oauth/google` | Вход через Google | ⏳ В разработке |
| GET | `/api/v1/oauth/apple` | Вход через Apple | ⏳ В разработке |

## 📊 Модель данных

### Credentials (credentials)

| Поле | Тип | Описание |
|------|-----|----------|
| Id | int | Первичный ключ |
| Email | string | Уникальный email |
| PasswordHash | string | Хеш пароля (BCrypt) |
| GoogleOAuthId | string? | ID Google OAuth |
| AppleOAuthId | string? | ID Apple OAuth |
| Role | Role | Роль пользователя |

### Роли (Role)

- `Passenger` — пассажир
- `Rider` — водитель
- `Admin` — администратор

## 🧪 Тестирование

```bash
cd AuthService/Tests
dotnet test
```

Используются **xUnit** и **Moq** для модульного тестирования.
 