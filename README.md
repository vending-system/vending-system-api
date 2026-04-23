# 🏪 Vending System API

REST API для управления сетью вендинговых аппаратов. Разработан как бэкенд-компонент информационной системы для франчайзеров и франчайзи.

## 🛠 Технологии

- **ASP.NET Core** (.NET 9)
- **Entity Framework Core** — ORM и миграции
- **PostgreSQL / MS SQL Server** — база данных
- **JWT** — аутентификация и авторизация
- **Swagger / OpenAPI** — документация эндпоинтов

---

## 📦 Структура проекта

```
ApiVending/
├── Controllers/        # HTTP-контроллеры (эндпоинты)
├── Services/           # Бизнес-логика
├── Models/             # Модели БД (EF Core)
├── DTO/                # Data Transfer Objects
├── Classes/            # Вспомогательные классы
├── Migrations/         # Миграции базы данных
├── Program.cs          # Точка входа, конфигурация
└── appsettings.json    # Конфигурация (не хранить секреты!)
```

---

## 🚀 Быстрый старт

### 1. Клонировать репозиторий

```bash
git clone https://github.com/vending-system/vending-system-api.git
cd vending-system-api
```

### 2. Настроить подключение к БД

Создай `appsettings.Development.json` (не коммитить!):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=vending_db;Username=postgres;Password=yourpassword"
  },
  "Jwt": {
    "Key": "your-secret-key-min-32-chars",
    "Issuer": "VendingApi",
    "Audience": "VendingClient"
  }
}
```

### 3. Применить миграции

```bash
dotnet ef database update
```

### 4. Запустить

```bash
dotnet run
```

Swagger UI доступен по адресу: `https://localhost:5001/swagger`

---

## 📋 Основные эндпоинты

### Авторизация
| Метод | Путь | Описание |
|-------|------|----------|
| POST | `/api/auth/login` | Вход, получение JWT токена |
| POST | `/api/auth/refresh-token` | Обновление токена |
| POST | `/api/auth/logout` | Инвалидация токена |

### Торговые аппараты
| Метод | Путь | Описание |
|-------|------|----------|
| GET | `/api/machines` | Список всех ТА (с пагинацией) |
| GET | `/api/machines/{id}` | Информация о конкретном ТА |
| POST | `/api/machines` | Добавить новый ТА |
| PUT | `/api/machines/{id}` | Обновить данные ТА |
| DELETE | `/api/machines/{id}` | Удалить ТА |
| GET | `/api/machines/realtime` | Эмуляция realtime-данных (статус связи, загрузка, деньги) |

### Товары
| Метод | Путь | Описание |
|-------|------|----------|
| GET | `/api/products` | Список товаров |
| POST | `/api/products` | Добавить товар |
| PUT | `/api/products/{id}` | Обновить товар |
| DELETE | `/api/products/{id}` | Удалить товар |

### Продажи
| Метод | Путь | Описание |
|-------|------|----------|
| GET | `/api/sales` | История продаж |
| GET | `/api/sales/summary` | Сводка по продажам |
| GET | `/api/sales/dynamics` | Динамика продаж за 10 дней |

### Обслуживание
| Метод | Путь | Описание |
|-------|------|----------|
| GET | `/api/maintenance` | Записи об обслуживании |
| POST | `/api/maintenance` | Добавить запись об обслуживании |

### Пользователи
| Метод | Путь | Описание |
|-------|------|----------|
| GET | `/api/users` | Список пользователей |
| GET | `/api/users/{id}` | Информация о пользователе |
| PUT | `/api/users/{id}` | Обновить пользователя |

---

## 🗄 Модель данных

Система включает следующие основные сущности:

- **Machine** — торговый аппарат (статус, модель, адрес, серийный номер, даты поверок и обслуживания)
- **Product** — товар (название, цена, количество, минимальный запас)
- **Sale** — продажа (аппарат, товар, сумма, метод оплаты, дата)
- **User** — пользователь (ФИО, email, роль)
- **MaintenanceRecord** — запись об обслуживании
- **Company** — компания/франчайзи

---

## 🔐 Авторизация

API использует JWT Bearer токены. Для доступа к защищённым эндпоинтам добавь заголовок:

```
Authorization: Bearer <your_token>
```

Роли пользователей: `Admin`, `Operator`, `Engineer`

---

## 🔧 Требования

- .NET 9 SDK
- PostgreSQL 14+ или MS SQL Server 2019+
- Git

---

## 📁 Связанные репозитории

- [vending-desktop](https://github.com/vending-system/vending-desktop) — десктопный клиент на Avalonia UI
