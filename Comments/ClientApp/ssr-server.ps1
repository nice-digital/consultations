Write-Host "Starting SSR..."

$pm2 = ".\node_modules\.bin\pm2.cmd"

$env:DOTENV_CONFIG_PATH = ".env.production"

# Kill existing
Start-Process $pm2 -ArgumentList "kill" -NoNewWindow -Wait

# Start SSR fully detached
Start-Process $pm2 `
  -ArgumentList "start src/server/ssr-server.js --name ssr --node-args=""-r dotenv/config"" --update-env" `
  -WindowStyle Hidden

Write-Host "SSR ready"

# Force exit cleanly
[Environment]::Exit(0)