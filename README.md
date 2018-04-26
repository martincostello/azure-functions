# Azure Functions

| | Linux | Windows |
|:-:|:-:|:-:|
| **Build Status** | [![Build status](https://img.shields.io/travis/martincostello/azure-functions/master.svg)](https://travis-ci.org/martincostello/azure-functions) | [![Build status](https://img.shields.io/appveyor/ci/martincostello/azure-functions/master.svg)](https://ci.appveyor.com/project/martincostello/azure-functions) [![codecov](https://codecov.io/gh/martincostello/azure-functions/branch/master/graph/badge.svg)](https://codecov.io/gh/martincostello/azure-functions) |
| **Build History** | [![Build history](https://buildstats.info/travisci/chart/martincostello/azure-functions?branch=master&includeBuildsFromPullRequest=false)](https://travis-ci.org/martincostello/azure-functions) |  [![Build history](https://buildstats.info/appveyor/chart/martincostello/azure-functions?branch=master&includeBuildsFromPullRequest=false)](https://ci.appveyor.com/project/martincostello/azure-functions) |

## Overview

A repository containing my custom Azure Functions.

## Functions

### DNSimpleWebhook

A function that acts as a receiver for webhooks from the [DNSimple API](https://dnsimple.com/webhooks). Only `v2` of the API and the `certificate.issue` payload are currently supported.

The function queries the DNSimple API for the associated certificate re-issue (such as for a [LetsEncrypt](https://letsencrypt.org/) TLS certificate) to obtain the certificate chain, public, and private keys, which are then uploaded as blobs to an Azure storage account container. A PFX file is also generated using the public and private key and uploaded to the same blob container.

Files are stored in the `certificates` container in the configured storage account using the naming convention shown below where the files are available from the DNSimple API.

  * `{commonName}_{thumbprint}_{timestamp}.cert.pem`
  * `{commonName}_{thumbprint}_{timestamp}.chain.pem`
  * `{commonName}_{thumbprint}_{timestamp}.chain.{index}.pem`
  * `{commonName}_{thumbprint}_{timestamp}.root.pem`
  * `{commonName}_{thumbprint}_{timestamp}.privkey.pem`
  * `{commonName}_{thumbprint}_{timestamp}.privkey.pfx`


  1. `{commonName}` is the Common Name (`CN={commonName}`) value from the certificate with `.` characters replaced by `_`.
  1. `{thumbprint}` is the lower-case thumbprint of the certificate.
  1. `{timestamp}` is the _Not Before_ UTC date of the certificate in the format `yyy-MM-dd`.

#### Configuration

| **Key** | **Default Value** | **Description**
|:--|:-:|:-:|
| `CERTIFICATE_PASSWORD` | _None_ | The password to use for generated `.pfx` files uploaded to blob storage. |
| `CERTIFICATE_STORE_CONNECTION` | _None_ | The connection string for the blob storage account to upload certificate files to. |
| `DNSIMPLE_URL` | `https://api.dnsimple.com` | The URL of the DNSimple API. |
| `DNSIMPLE_TOKEN` | _None_ | The access token to use for the DNSimple API. |

### BindPrivateCertificate

A function that is invoked when blobs are created/updated in the `certificates` container of the configured Azure storage account that match the naming convention `{name}.privkey.pfx` and binds them to any App Service instances in the configured Azure subscription.

The certificate is bound to the TLS/SSL bindings associated with the Common Name and any Subject Alternate Names associated with the certificate for an App Service instance provided that:

  1. The _Not Before_ timestamp of the certificate is in the past.
  1. The _Not After_ timestamp of the certificate is in the future.
  1. The host name has an existing TLS binding.
  1. The _Thumbprint_ of the certificate differs from the currently configured certificate for the binding.

#### Configuration

| **Key** | **Default Value** | **Description**
|:--|:-:|:-:|
| `AZURE_CREDENTIALS_FILE` | `%USERPROFILE%\.azure\credentials.json` | The path to the Azure credentials file to use to authenticate with Azure Resource Management APIs if not using Service Principal or Managed Service Identity authentication. |
| `AZURE_SUBSCRIPTION_ID` | _None_ | The Id of the Azure subscription to configure App Services instances in. |
| `CERTIFICATE_PASSWORD` | _None_ | The password associated with the X.509 certificates stored in the Azure storage account. |
| `CERTIFICATE_STORE_CONNECTION` | _None_ | The connection string for the blob storage account which X.509 certificates are stored in. |
| `SERVICE_PRINCIPAL_CLIENT_ID` | _None_ | The client Id to use for Service Principal authentication. |
| `SERVICE_PRINCIPAL_CLIENT_SECRET` | _None_ | The client secret to use for Service Principal authentication. |
| `SERVICE_PRINCIPAL_TENANT_ID` | _None_ | The tenant Id to use for Service Principal authentication. |
| `USE_MANAGED_SERVICE_IDENTITY` | `false` | Whether to use Managed Service Identity authentication with Azure Resource Management APIs. |
| `USE_SERVICE_PRINCIPAL` | `true` | Whether to use a Service Principal for authentication with Azure Resource Management APIs. |

You must also set `WEBSITE_LOAD_CERTIFICATES` to `*` in the function's Application settings in the Azure portal so that the private keys for X.509 certificates can be loaded.
