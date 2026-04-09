Write-Host "Starting SSR..."

$node = "node"
$pm2 = ".\node_modules\pm2\bin\pm2"

& $node $pm2 restart ssr

if ($LASTEXITCODE -ne 0) {
    Write-Host "SSR not running, starting..."
    & $node $pm2 start src/server/ssr-server.js --name ssr
}

exit 0

Write-Host "SSR ready"