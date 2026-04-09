Write-Host "Starting SSR..."

$node = "node"
$pm2 = ".\node_modules\pm2\bin\pm2"

# ensure env is passed to PM2
$env:DOTENV_CONFIG_PATH = ".env.production"

& $node $pm2 restart ssr --update-env

if ($LASTEXITCODE -ne 0) {
    Write-Host "SSR not running, starting..."
    & $node $pm2 start src/server/ssr-server.js --name ssr --node-args="-r dotenv/config" --update-env
}

Write-Host "SSR ready"
exit 0