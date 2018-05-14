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
    --arg IndevApiKey "$INDEV_APIKEY" \
    --arg IndevBasePath "$INDEV_BASEPATH" \
    --arg IndevPublishedChapterFeedPath "$INDEV_CHAPTER" \
    --arg IndevPublishedDetailFeedPath "$INDEV_DETAIL" \
    --arg IndevListFeedPath "$INDEV_LIST" \
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
    .Feeds.ApiKey = $IndevApiKey |
    .Feeds.BasePath = $IndevBasePath |
    .Feeds.Chapter = $IndevPublishedChapterFeedPath |
    .Feeds.Detail = $IndevPublishedDetailFeedPath |
    .Feeds.List = $IndevListFeedPath |
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