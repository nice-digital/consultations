#!/bin/bash

# Replace values in appsettings.json from envirionent variables
# And run .NET webapp

set -e

jq \
    --arg defaultConnection "$DEFAULT_CONNECTION" \
    --arg loggingLogFilePath "$LOGGING_LOG_FILE_PATH" \
    --arg appSettingsEnvironmentName "$APPSETTINGS_ENVIRONMENT_NAME" \
    --arg appSettingsEnvironmentSecureSite "$APPSETTINGS_ENVIRONMENT_SECURESITE" \
    --arg appSettingsEnvironmentRealm "$APPSETTINGS_ENVIRONMENT_REALM" \
    --arg feedsApikey "$FEEDS_APIKEY" \
    --arg feedsBasePath "$FEEDS_BASEPATH" \
    --arg feedsChapter "$FEEDS_CHAPTER" \
    --arg feedsDetail "$FEEDS_DETAIL" \
    --arg feedsList "$FEEDS_LIST" \
    --arg gilliamClientCertificateBase64 "$GILLIAM_CLIENT_CERTIFICATE_BASE64" \
    --arg gilliamBasePath "$GILLIAM_BASE_PATH" \
    --arg gilliamGetClaimsUrl "$GILLIAM_GET_CLAIMS_URL" \
    --arg gilliamRealm "$GILLIAM_REALM" \
    '
    .ConnectionStrings.DefaultConnection = $defaultConnection |
    .Logging.LogFilePath = $loggingLogFilePath |
    .AppSettings.Environment.Name = $appSettingsEnvironmentName |
    .AppSettings.Environment.SecureSite = $appSettingsEnvironmentSecureSite |
    .AppSettings.Environment.Realm = $appSettingsEnvironmentRealm |
    .Feeds.ApiKey = $feedsApikey |
    .Feeds.BasePath = $feedsBasePath |
    .Feeds.Chapter = $feedsChapter |
    .Feeds.Detail = $feedsDetail |
    .Feeds.List = $feedsList |
    .Gilliam.GilliamBasePath = $gilliamBasePath |
    .Gilliam.GetClaimsUrl = $gilliamGetClaimsUrl |
    .Gilliam.Realm = $gilliamRealm |
    .Gilliam.GilliamClientCertificateBase64 = $gilliamClientCertificateBase64
    '\
    appsettings.json > _appsettings.json \
    && mv _appsettings.json appsettings.json

dotnet Comments.dll

# See https://stackoverflow.com/questions/39082768/what-does-set-e-and-exec-do-for-docker-entrypoint-scripts
exec "$@"