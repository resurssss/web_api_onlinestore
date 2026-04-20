@echo off
echo  TEST: HTTPS + TLS + BALANCING + STATIC
echo.

echo [1] HTTPS check:
curl -k -s https://localhost/api/products/env | findstr instance_id
echo.

echo [2] Headers check:
curl -k -i https://localhost/api/products 2>nul | findstr "X-Instance-Id"
echo.

echo [3] Round-robin (20 requests):
for /l %%i in (1,1,20) do (
    curl -k -s https://localhost/api/products/env 2>nul | findstr "Instance"
)
echo.

echo [4] Static files:
curl -k -s https://localhost/static/style.css
echo.

echo [5] HTTP redirect:
curl -i http://localhost/api/products 2>nul | findstr "Location"
echo.

echo [6] Direct access block:
curl http://localhost:5000/api/products 2>&1 | findstr "Failed"
if errorlevel 1 echo OK - port 5000 blocked
echo.


pause