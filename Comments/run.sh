#!/bin/bash

# Replace values in appsettings.json from envirionent variables
# And run .NET webapp

jq \
    --arg defaultConnection "$DEFAULT_CONNECTION" \
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