#!/usr/bin/env bash
set -euo pipefail

echo "Поднимаем контейнер Docker..."
docker compose down --remove-orphans || true
docker compose up --build -d

echo "Список запущенных контейнеров:"
docker compose ps

echo "Логи сервера (Ctrl+C для выхода):"
docker compose logs -f api