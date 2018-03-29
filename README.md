# Azure Functions

| | Linux | Windows |
|:-:|:-:|:-:|
| **Build Status** | [![Build status](https://img.shields.io/travis/martincostello/azure-functions/master.svg)](https://travis-ci.org/martincostello/azure-functions) | [![Build status](https://img.shields.io/appveyor/ci/martincostello/azure-functions/master.svg)](https://ci.appveyor.com/project/martincostello/azure-functions) [![codecov](https://codecov.io/gh/martincostello/azure-functions/branch/master/graph/badge.svg)](https://codecov.io/gh/martincostello/azure-functions) |
| **Build History** | [![Build history](https://buildstats.info/travisci/chart/martincostello/azure-functions?branch=master&includeBuildsFromPullRequest=false)](https://travis-ci.org/martincostello/azure-functions) |  [![Build history](https://buildstats.info/appveyor/chart/martincostello/azure-functions?branch=master&includeBuildsFromPullRequest=false)](https://ci.appveyor.com/project/martincostello/azure-functions) |

## Overview

A repository containing my custom Azure Functions.

## Functions

### DNSimpleWebhook

#### Configuration

| **Key** | **Default Value** | **Description**
|:--|:-:|:-:|
| `CERTIFICATE_PASSWORD` | _None_ | _TODO_ |
| `CERTIFICATE_STORE_CONNECTION` | _None_ | _TODO_ |
| `DNSIMPLE_URL` | `https://api.dnsimple.com` | _TODO_ |
| `DNSIMPLE_TOKEN` | _None_ | _TODO_ |

### BindPrivateCertificate

#### Configuration

| **Key** | **Default Value** | **Description**
|:--|:-:|:-:|
| `AZURE_CREDENTIALS_FILE` | `%USERPROFILE%\.azure\credentials.json` | _TODO_ |
| `AZURE_SUBSCRIPTION_ID` | _None_ | _TODO_ |
| `CERTIFICATE_PASSWORD` | _None_ | _TODO_ |
| `CERTIFICATE_STORE_CONNECTION` | _None_ | _TODO_ |
| `USE_MANAGED_SERVICE_IDENTITY` | `false` | _TODO_ |

You must also set `WEBSITE_LOAD_CERTIFICATES` to `*` in the function's Application settings in the Azure portal so that the private keys for X.509 certificates can be loaded.
