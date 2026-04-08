Write-Host "Starting SSR..."

$env:ENV_FILE=".env.production"

$pm2 = ".\node_modules\.bin\pm2.ps1"

& $pm2 restart ssr

if ($LASTEXITCODE -ne 0) {
    Write-Host "SSR not running, starting..."
    & $pm2 start src/server/ssr-server.js --name ssr
}

Write-Host "SSR ready"