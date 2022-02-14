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
    --arg appSettingsEnvironmentAccountsEnv "$ACCOUNTS_ENVIRONMENT" \
    --arg indevApiKey "$INDEV_APIKEY" \
    --arg indevBasePath "$INDEV_BASEPATH" \
    --arg indevPublishedChapterFeedPath "$INDEV_PUBLISHED_CHAPTER" \
    --arg indevDraftPreviewChapterFeedPath "$INDEV_DRAFT_PREVIEW_CHAPTER" \
    --arg indevPublishedDetailFeedPath "$INDEV_PUBLISHED_DETAIL" \
    --arg indevDraftPreviewDetailFeedPath "$INDEV_DRAFT_PREVIEW_DETAIL" \
    --arg indevPublishedPreviewDetailFeedPath "$INDEV_PUBLISHED_PREVIEW_DETAIL" \
    --arg indevListFeedPath "$INDEV_LIST" \
    --arg indevIdamApiIdentifier "$INDEV_IDAM_CONFIG_APIIDENTIFIER" \
    --arg indevIdamClientId "$INDEV_IDAM_CONFIG_CLIENTID" \
    --arg indevIdamClientSecret "$INDEV_IDAM_CONFIG_CLIENTSECRET" \
    --arg indevIdamDomain "$INDEV_IDAM_CONFIG_DOMAIN" \
    --arg gilliamClientCertificateBase64 "$GILLIAM_CLIENT_CERTIFICATE_BASE64" \
    --arg gilliamBasePath "$GILLIAM_BASE_PATH" \
    --arg gilliamGetClaimsUrl "$GILLIAM_GET_CLAIMS_URL" \
    --arg gilliamRealm "$GILLIAM_REALM" \
    --arg webAppApiIdentifier "$WEBAPP_API_IDENTIFIER" \
    --arg webAppClientId "$WEBAPP_CLIENTID" \
    --arg webAppClientSecret "$WEBAPP_CLIENTSECRECT" \
    --arg webAppAuthorisationServiceUri "$WEBAPP_AUTH_SERVICE_URI" \
    --arg googleTrackingId "$WEBAPP_GOOGLE_TRACKING_ID" \
    --arg webAppDomain "$WEBAPP_DOMAIN" \
    --arg encryptionKey "$ENCRYPTION_KEY" \
    --arg encryptionIV "$ENCRYPTION_IV" \
    --arg adminRole "$ADMINROLE" \
    --arg teamRoles1 "$TEAMROLES1" \
    --arg teamRoles2 "$TEAMROLES2" \
    --arg teamRoles3 "$TEAMROLES3" \
    --arg OrgCommenting "$ORG_COMMENTING" \
    --arg RedisConnectionString "$REDIS_CONNECTION_STRING" \
    '
    .ConnectionStrings.DefaultConnection = $defaultConnection |
    .Logging.LogFilePath = $loggingLogFilePath |
    .AppSettings.Environment.Name = $appSettingsEnvironmentName |
    .AppSettings.Environment.SecureSite = $appSettingsEnvironmentSecureSite |
    .AppSettings.Environment.Realm = $appSettingsEnvironmentRealm |
    .AppSettings.Environment.AccountsEnvironment = $appSettingsEnvironmentAccountsEnv |
    .Feeds.ApiKey = $indevApiKey |
    .Feeds.IndevBasePath = $indevBasePath |
    .Feeds.IndevPublishedChapterFeedPath = $indevPublishedChapterFeedPath |
    .Feeds.IndevDraftPreviewChapterFeedPath = $indevDraftPreviewChapterFeedPath |
    .Feeds.IndevPublishedDetailFeedPath = $indevPublishedDetailFeedPath |
    .Feeds.IndevDraftPreviewDetailFeedPath = $indevDraftPreviewDetailFeedPath |
    .Feeds.IndevPublishedPreviewDetailFeedPath = $indevPublishedPreviewDetailFeedPath |
    .Feeds.IndevListFeedPath = $indevListFeedPath |
    .Feeds.IndevIDAMConfig.APIIdentifier = $indevIdamApiIdentifier |
    .Feeds.IndevIDAMConfig.ClientId = $indevIdamClientId |
    .Feeds.IndevIDAMConfig.ClientSecret = $indevIdamClientSecret |
    .Feeds.IndevIDAMConfig.Domain = $indevIdamDomain |
    .Gilliam.GilliamBasePath = $gilliamBasePath |
    .Gilliam.GetClaimsUrl = $gilliamGetClaimsUrl |
    .Gilliam.Realm = $gilliamRealm |
    .Gilliam.GilliamClientCertificateBase64 = $gilliamClientCertificateBase64 |
    .WebAppConfiguration.ApiIdentifier = $webAppApiIdentifier |
    .WebAppConfiguration.ClientId = $webAppClientId |
    .WebAppConfiguration.ClientSecret = $webAppClientSecret |
    .WebAppConfiguration.AuthorisationServiceUri = $webAppAuthorisationServiceUri |
    .WebAppConfiguration.Domain = $webAppDomain |
    .WebAppConfiguration.GoogleTrackingId = $googleTrackingId |
    .Encryption.Key = $encryptionKey |
    .Encryption.IV = $encryptionIV |
    .FeatureManagement.OrganisationalCommenting = true |
    .ConsultationList.DownloadRoles.AdminRoles |= .+ [$adminRole] |
    .ConsultationList.DownloadRoles.TeamRoles |= .+ [$teamRoles1] |
    .ConsultationList.DownloadRoles.TeamRoles |= .+ [$teamRoles2] |
    .ConsultationList.DownloadRoles.TeamRoles |= .+ [$teamRoles3] |
    .WebAppConfiguration.RedisServiceConfiguration.ConnectionString = $RedisConnectionString |
    .WebAppConfiguration.RedisServiceConfiguration.Enabled = true
    '\
    appsettings.json > _appsettings.json \
    && mv _appsettings.json appsettings.json

replace "#{GlobalNav:Script}" "$REACT_APP_GLOBAL_NAV_SCRIPT" ClientApp/build/index.html
replace "#{GlobalNav:ScriptIE8}" "$REACT_APP_GLOBAL_NAV_SCRIPT_IE8" ClientApp/build/index.html
replace "#{AppSettings:Environment:AccountsEnvironment}" "$REACT_APP_ACCOUNTS_ENVIRONMENT" ClientApp/build/index.html

dotnet Comments.dll

# See https://stackoverflow.com/questions/39082768/what-does-set-e-and-exec-do-for-docker-entrypoint-scripts
exec "$@"