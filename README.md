# Reference Data API

The Reference Data API is the London Borough of Hackney (LBH) **MTFH** service that acts as the **single
source of truth for reference data** — the controlled vocabularies and lookup values used across the
platform (categories, sub-categories, codes, values, descriptions and tags).

It is a read-only, Elasticsearch-backed API.

## Stack

- **.NET 10** (`net10.0`) — ASP.NET Core Web API hosted on **AWS Lambda** via `Amazon.Lambda.AspNetCoreServer`
  (the managed `dotnet10` runtime).
- **Elasticsearch** as the datastore, accessed with **NEST**.
- **Swashbuckle** for Swagger/OpenAPI, **FluentValidation** for request validation.
- **MVC API versioning** (URL segment reader), **Newtonsoft.Json** available for JSON handling.
- **AWS X-Ray** for tracing.
- **xUnit** (+ `TestStack.BDDfy` for the E2E stories) and FluentAssertions/Moq.

### API definition

Swagger Hub — https://app.swaggerhub.com/apis/Hackney/referenceDataApi/1.0.0

The running service also serves its own Swagger UI at `/swagger` (and `/swagger/{proxy+}` through API Gateway).

## Endpoints

| Verb | Route | Request | Response |
| --- | --- | --- | --- |
| GET | `/api/v1/reference-data` | Query: `category` (**required**, must not be empty, XSS-checked), `subCategory` (optional, XSS-checked), `includeInactive` (optional bool) | `200` → `ReferenceDataResponseObject`, a dictionary keyed by sub-category of the matching `ReferenceData` records. Also declares `400` and `500`. |
| GET | `/api/v1/healthcheck/ping` | — | `200` → health report written by `HealthCheckResponseWriter`. Unversioned (mapped outside MVC). The registered check pings Elasticsearch. |
| GET | `/swagger` | — | Swagger UI / OpenAPI document, one document per API version. |

`ReferenceData` fields: `Id`, `Category`, `SubCategory`, `Code`, `Value`, `Description`, `IsActive`,
`CreatedAt`, `Tags`.

Versioning uses `UrlSegmentApiVersionReader` with `DefaultApiVersion = 1.0` and
`AssumeDefaultVersionWhenUnspecified = true`; the controller is decorated `[ApiVersion("1.0")]` and
`[MapToApiVersion("1")]`. All endpoints sit behind the API Gateway request authorizer and API key.

## Data sources and external dependencies

| Dependency | Detail |
| --- | --- |
| Elasticsearch index | `reference_data` (hard-coded in `ElasticSearchGateway`). The search query relies on `.keyword` sub-fields via `.Suffix("keyword")`. |
| Index definition | `data/elasticsearch/index.json`. Note the test fixture uses `ReferenceDataApi.Tests/Data/referencedata-index.json`, which declares the `.keyword` sub-fields the query needs. |
| Elasticsearch endpoint | Config key `ElasticSearchDomainUrl`, default `http://localhost:9200`; in Lambda it comes from SSM (`/reference-data-api/{stage}/elasticsearch-domain`). Local development overrides it to `http://localhost:9202`; the Docker stack uses `http://reference-data-elasticsearch:9200`. |
| AWS-managed Elasticsearch | Domain `reference-data-api-es`, `es_version` 7.8, `t3.small.elasticsearch` (development/staging) and `t3.medium.elasticsearch` (production). |
| Local Elasticsearch | `docker.elastic.co/elasticsearch/elasticsearch:7.9.3` with the `analysis-icu` plugin (`data/elasticsearch/Dockerfile`). |
| AWS X-Ray | `AWSXRayRecorder.InitializeInstance(...)` + `app.UseXRay(...)`; `AWSXRayDaemonWriteAccess` on the Lambda role. |
| AWS SSM Parameter Store | Per-stage Elasticsearch endpoint; `/housing-tl/{stage}/aspnetcore-environment`. |
| CloudWatch Synthetics | Production-only canary (`reference-data-health-2`) hitting the health endpoint every 5 minutes. |
| Database | **None.** The service does not use DynamoDB or SQL. `DynamoDb_LocalMode` in `ReferenceDataApi/Dockerfile` and the `DynamoDbCollection` fixture class name are historical leftovers. |

## Setup

### Prerequisites

1. [Docker][docker-download] with the **`docker compose`** v2 plugin.
2. A **`LBHPACKAGESTOKEN`** with `read:packages` access to the `LBHackney-IT` GitHub Packages feed — the
   `Hackney.*` packages are not published to nuget.org:
   ```sh
   export LBHPACKAGESTOKEN=<token>
   ```
3. Optionally copy [`.env.sample`](.env.sample) to `.env` and set `LBHPACKAGESTOKEN` there instead of
   exporting it — `docker compose` reads `.env` for the values referenced in `docker-compose.yml`. The file
   also lists every environment variable the API reads at runtime; those are **not** loaded from `.env`, so
   export them (or add them to the compose service) if you need to override the defaults.

### Build and run

```sh
git clone git@github.com:LBHackney-IT/reference-data-api.git
cd reference-data-api
make setup     # docker compose build
make build     # build the API image
make serve     # start the API (port 3000) and its Elasticsearch instance
make shell     # bash inside the API container
```

A local Elasticsearch is provided by the `reference-data-elasticsearch` service in `docker-compose.yml`.

## Testing

```sh
make test
```

`make test` builds the test image (`ReferenceDataApi.Tests/Dockerfile`) and runs the xUnit suite inside it
against the compose-provided Elasticsearch instance. Coverage is collected with `coverlet.msbuild`
(`/p:CollectCoverage=true /p:CoverletOutputFormat=opencover`).

The suite includes unit tests for the gateways, query orchestrator, use case, validators, factories and
controllers, plus BDDfy E2E stories that exercise the real HTTP pipeline against Elasticsearch.

### Agreed testing approach

- xUnit as the test framework, `TestStack.BDDfy` for E2E stories, FluentAssertions and Moq.
- Tests should be independent of each other.
- E2E tests run against a real (containerised) Elasticsearch instance.
- Unit and E2E tests run in CI.
- Test coverage should never go down.

## Deployment

CircleCI (`.circleci/config.yml`) gates deployment on a code-formatting check (`dotnet-format --check`), the
Docker test suite and a `sonarcloud/scan` step, then deploys infrastructure with **Terraform** and the Lambda
with the **Serverless Framework**. Changes are reviewed and approved on a branch before merging.

**Stages and triggers:**

- **Feature branches** (anything except `master` and `release`) — formatting check and tests only.
- **`master`** — formatting, tests, then Terraform apply to development and deployment to **development**.
- **`release`** — tests, then Terraform apply to staging and deployment to **staging**. Production follows only
  after the `permit-production-terraform-release` and `permit-production-release` manual approvals.
- **`release`** — the separate `deploy-terraform-pre-production` and `deploy-code-pre-production` workflows
  deploy **pre-production**, each behind its own manual approval.

**Packaging and runtime:**

- `build.sh` installs `Amazon.Lambda.Tools` and runs
  `dotnet lambda package --configuration release --framework net10.0 --output-package ./bin/release/net10.0/reference-data-api.zip`.
  (`build.cmd` is the Windows equivalent for local use.)
- The deploy job runs `sls deploy --stage <stage> --conceal`.
- `serverless.yml` declares `runtime: dotnet10`, consumes that artifact, exposes `ANY /{proxy+}` plus
  `GET /swagger/{proxy+}` behind the request authorizer, and attaches the `Platform_APIs_Web_ACL` WAF.

## Contacts

**Housing product team** — [mmh-project-team@hackney.gov.uk](mailto:mmh-project-team@hackney.gov.uk)

[docker-download]: https://www.docker.com/products/docker-desktop
[AWS-CLI]: https://aws.amazon.com/cli/
