# Online Store Web API

Backend REST API для интернет-магазина, разработанный на **ASP.NET Core 9** с использованием PostgreSQL. Проект поддерживает JWT-аутентификацию, ролевую авторизацию, управление пользовательскими сессиями и загрузку файлов.

## Технологии

- **C# / ASP.NET Core 9** — разработка REST API.
- **PostgreSQL** — реляционная база данных.
- **Entity Framework Core** — взаимодействие с базой данных.
- **JWT** — аутентификация пользователей.
- **FluentValidation** — валидация данных.
- **AutoMapper** — маппинг объектов.
- **Docker / Docker Compose** — контейнеризация приложения.
- **Nginx** — обратный прокси и балансировка нагрузки.
- **Prometheus / Grafana / Loki** — мониторинг и логирование.
- **xUnit / Moq / FluentAssertions** — тестирование.

## Архитектура

Проект разделён на несколько компонентов:

- **OnlineStore.API** — контроллеры, HTTP-запросы и конфигурация API.
- **OnlineStore.Core** — основные компоненты приложения, работа с базой данных и авторизация.
- **OnlineStore.Services** — бизнес-логика и сервисы.
- **OnlineStore.Infrastructure** — инфраструктурные компоненты.
- **OnlineStore.Tests** — автоматизированные тесты.

## Инфраструктура

Приложение разворачивается с помощью Docker Compose и включает:

- Три экземпляра API для распределения нагрузки.
- PostgreSQL для хранения данных.
- Nginx для балансировки запросов.
- Prometheus для сбора метрик.
- Grafana для визуализации показателей.
- Loki и Promtail для централизованного сбора логов.

## Запуск проекта

**1. Клонирование репозитория**

```bash
git clone https://github.com/resurssss/web_api_onlinestore.git
cd web_api_onlinestore
```

**2. Запуск приложения**

```bash
docker compose up -d
```

**3. Проверка запущенных контейнеров**

```bash
docker compose ps
```

Для просмотра логов:

```bash
docker compose logs -f
```

Перед запуском необходимо настроить SSL-сертификаты и параметры подключения к базе данных, если они не заданы в конфигурации.

## Доступ к сервисам

| Сервис | Адрес |
|---|---|
| API | https://localhost |
| Swagger | https://localhost/swagger |
| Prometheus | http://localhost:9090 |
| Grafana | http://localhost:3000 |

## Тестирование

Для запуска автоматизированных тестов:

```bash
dotnet test OnlineStore.sln
```

## Остановка приложения

```bash
docker compose down
```

Для остановки с удалением данных Docker volumes:

```bash
docker compose down -v
```

