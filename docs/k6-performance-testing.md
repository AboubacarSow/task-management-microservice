# k6 Performance Testing Guide

> Performance testing strategy and reference for the Task Management Microservice System.

---

## Table of Contents

1. [Overview](#overview)
2. [Prerequisites](#prerequisites)
3. [Test Types](#test-types)
4. [Project Test Scenarios](#project-test-scenarios)
5. [Load Test Results](#load-test-results)
6. [Running Tests](#running-tests)
7. [Interpreting Results](#interpreting-results)
8. [Known Issues & Root Causes](#known-issues--root-causes)
9. [Observability](#observability)
10. [Roadmap](#roadmap)

---

## Overview

This project uses [k6](https://k6.io) by Grafana Labs for performance testing. k6 runs as a Docker container on the internal Docker network, targeting the **dispatcher service** (port 80 internally) which routes requests to downstream services.

Results are streamed in real time to **InfluxDB** and visualized in **Grafana**.

```
k6 container
  └─► dispatcher-service:80 (Ocelot gateway)
        ├─► authentication-service:5004 (login)
        └─► project-service:5000 (projects CRUD)
```

---

## Prerequisites

Before running any test, ensure the following:

**1. Stack is running**

```bash
make dev
```

**2. Seed user exists**

The `k6-seed` service in `docker-compose.override.yml` automatically creates the test user on startup. Verify it ran:

```bash
docker compose logs k6-seed
# Expected: "Seed user created" or "409 Conflict" (already exists — fine)
```

**3. Verify login works manually**

```bash
curl -X POST http://localhost:8080/login \
  -H "Content-Type: application/x-www-form-urlencoded" \
  --data-urlencode "grant_type=password" \
  --data-urlencode "client_id=postman-client" \
  --data-urlencode "client_secret=postman-secret" \
  --data-urlencode "username=honoredoue@gmail.com" \
  --data-urlencode "password=123456" \
  --data-urlencode "scope=openid profile project_fullpermission offline_access"
```

A successful response returns `{"access_token": "...", "token_type": "Bearer", ...}`. If login fails here, the load test will also fail — fix the issue before running.

---

## Test Types

k6 supports six categories of performance tests. Each answers a different question about the system.

### 1. Smoke Test

**Purpose:** Verify the system works correctly at minimal load — 1 or 2 VUs for a short duration. Run this first before any heavier test.

**Question answered:** Does the system function at all? Are the endpoints reachable and returning correct responses?

**When to use:** After any deployment, before running heavier tests, or when debugging a failing scenario.

```javascript
export const options = {
  vus: 1,
  duration: '30s',
  thresholds: {
    http_req_failed: ['rate<0.01'],
    http_req_duration: ['p(95)<500'],
  },
};
```

```bash
make load-test-smoke
```

---

### 2. Load Test

**Purpose:** Validate system performance under **expected production traffic**. Uses a fixed number of VUs sustained over a defined period.

**Question answered:** Can the system handle the traffic we expect on a normal day?

**When to use:** Regularly, as part of a CI/CD pipeline or before a release.

```javascript
export const options = {
  stages: [
    { duration: '30s', target: 50 },   // ramp up
    { duration: '2m',  target: 50 },   // sustain
    { duration: '30s', target: 0  },   // ramp down
  ],
  thresholds: {
    http_req_failed:   ['rate<0.01'],
    http_req_duration: ['p(95)<500'],
  },
};
```

```bash
make load-test-50
make load-test-100
make load-test-200
```

---

### 3. Stress Test

**Purpose:** Push the system **beyond its expected capacity** in incremental stages to find where it starts to degrade.

**Question answered:** Where does performance degrade? At what point do errors appear?

**When to use:** When capacity planning, or after a significant architecture change.

```javascript
export const options = {
  stages: [
    { duration: '2m', target: 100  },
    { duration: '5m', target: 100  },
    { duration: '2m', target: 200  },
    { duration: '5m', target: 200  },
    { duration: '2m', target: 300  },
    { duration: '5m', target: 300  },
    { duration: '2m', target: 0    },
  ],
  thresholds: {
    http_req_failed:   ['rate<0.1'],
    http_req_duration: ['p(95)<2000'],
  },
};
```

```bash
make load-test-stress
```

---

### 4. Spike Test

**Purpose:** Simulate a **sudden, dramatic surge** in traffic — like a flash sale or viral event — then drop back to normal.

**Question answered:** Does the system recover gracefully after a sudden burst? Are there connection pool issues, memory spikes, or cascading failures?

**When to use:** Before a planned marketing event, product launch, or any anticipated traffic spike.

```javascript
export const options = {
  stages: [
    { duration: '10s', target: 10  },  // baseline
    { duration: '1m',  target: 10  },
    { duration: '10s', target: 500 },  // spike
    { duration: '3m',  target: 500 },
    { duration: '10s', target: 10  },  // recovery
    { duration: '3m',  target: 10  },
    { duration: '10s', target: 0   },
  ],
  thresholds: {
    http_req_failed:   ['rate<0.1'],
    http_req_duration: ['p(95)<2000'],
  },
};
```

```bash
make load-test-spike
```

---

### 5. Soak Test (Endurance)

**Purpose:** Run at **normal load for an extended period** (hours) to detect issues that only appear over time — memory leaks, connection pool exhaustion, gradual performance degradation.

**Question answered:** Is the system stable over time? Does performance degrade after sustained usage?

**When to use:** Before a major release, or periodically (weekly/monthly) to catch slow regressions.

```javascript
export const options = {
  stages: [
    { duration: '5m',  target: 100 },  // ramp up
    { duration: '4h',  target: 100 },  // sustain for 4 hours
    { duration: '5m',  target: 0   },  // ramp down
  ],
  thresholds: {
    http_req_failed:   ['rate<0.01'],
    http_req_duration: ['p(95)<1000'],
  },
};
```

```bash
make load-test-soak
```

> ⚠️ Soak tests require the system to be running stably for hours. Ensure sufficient disk space for InfluxDB and that the IdentityServer signing keys are persisted (see [Known Issues](#known-issues--root-causes)).

---

### 6. Breakpoint Test

**Purpose:** Continuously ramp VUs upward until the system **fails completely**, to find the absolute maximum capacity.

**Question answered:** What is the system's hard limit? At what VU count do we see catastrophic failure?

**When to use:** Once, for capacity planning, after the system is fully stable. Not part of regular CI.

```javascript
export const options = {
  stages: [
    { duration: '2h', target: 10000 }, // ramp to 10k — test will fail before this
  ],
  thresholds: {
    http_req_failed:   ['rate<0.1'],
    http_req_duration: ['p(95)<2000'],
  },
};
```

```bash
make load-test-breakpoint
```

> ⚠️ This test is designed to crash the system. Run it in an isolated environment, never against production.

---

## Project Test Scenarios

The following scenarios are implemented under `infrastructure/k6/scripts/scenarios/`:

| File | VUs | Duration | Make target |
|------|:---:|:--------:|-------------|
| `smoke.js` | 1 | 30s | `make load-test-smoke` |
| `50-users.js` | 50 | 2m | `make load-test-50` |
| `100-users.js` | 100 | 2m | `make load-test-100` |
| `200-users.js` | 200 | 2m | `make load-test-200` |
| `500-users.js` | 500 (ramp) | 6m30s | `make load-test-500` |
| `stress.js` | up to 300 | ~21m | `make load-test-stress` |
| `spike.js` | 10 → 500 → 10 | ~7m | `make load-test-spike` |
| `soak.js` | 100 | 4h | `make load-test-soak` |
| `breakpoint.js` | unlimited ramp | until failure | `make load-test-breakpoint` |

### What each iteration does

Each VU runs the following flow per iteration:

```
1. POST /projects  →  create a uniquely named project  →  expect 201 Created
        sleep 1s
2. GET  /projects  →  list all projects for the user   →  expect 200 OK
        sleep 1s
```

The `setup()` function runs **once** before all VUs start. It registers the test user (safe if already exists) and logs in to obtain a JWT, which is shared with all VUs via the `data` argument.

### Shared helpers (`helpers.js`)

```javascript
import { getToken, runScenario } from '../helpers.js';

export function setup() { return getToken(); }
export default function(data) { runScenario(data.token); }
```

---

## Load Test Results

Results from fixed-VU load tests (2 minutes each at steady state):

| VUs | Duration | Avg (ms) | p90 (ms) | p95 (ms) | Max (ms) | Throughput (req/s) | Error Rate | p95 < 2000ms |
|:---:|:--------:|:--------:|:--------:|:--------:|:--------:|:-----------------:|:----------:|:------------:|
| **50** | 2m00s | 2.96 | 6.03 | 8.55 | 24.19 | 49.8 | ~0%* | ✅ |
| **100** | 2m00s | 3.53 | 7.92 | 12.64 | 36.64 | 99.5 | ~0%* | ✅ |
| **200** | 2m00s | 3.01 | 6.98 | 10.05 | 71.90 | 199.1 | ~0%* | ✅ |
| **500** | 6m30s | 2.61 | 4.88 | 7.87 | 517.98 | 195.4 | ~0%* | ✅ |

> \* Previous runs showed 100% error rate due to IdentityServer key persistence issues (see [Known Issues](#known-issues--root-causes)). Results above reflect corrected runs after fixing permissions on the `/app/dataprotection` volume.

**Key observations:**

- Response time scales excellently — p95 stays under 13 ms even at 200 VUs
- Throughput scales linearly with VUs at lower concurrency (49.8 → 99.5 → 199.1 req/s at 50/100/200 VUs)
- The 500 VU ramp-up test shows a one-off spike of 517 ms — this is the ramp-up phase, not steady state
- All runs pass the `p(95) < 2000ms` threshold with a wide margin

---

## Running Tests

### Quick reference

```bash
make dev              # start all services + seed user
make load-test-smoke  # verify basic functionality (1 VU, 30s)
make load-test-50     # 50 VUs, 2 minutes
make load-test-100    # 100 VUs, 2 minutes
make load-test-200    # 200 VUs, 2 minutes
make load-test-500    # 500 VUs ramp-up, 6m30s
make load-test-stress # stress test — find degradation point
make load-test-spike  # spike test — sudden burst
make load-test-soak   # soak test — 4 hours (long-running)
make load-test-breakpoint  # find absolute limit (destructive)
```

### Recommended order

Always run in this order. Each test builds on confidence from the previous:

```
smoke → load (50) → load (100) → load (200) → stress → spike → soak → breakpoint
```

Never run stress, spike, or breakpoint tests if the smoke test fails.

---

## Interpreting Results

### Thresholds

| Threshold | Description | Our target |
|-----------|-------------|-----------|
| `http_req_failed rate < 0.1` | Less than 10% of requests fail | < 1% in steady state |
| `http_req_duration p(95) < 2000` | 95th percentile under 2 seconds | < 50 ms in practice |
| `error_rate rate < 0.1` | Custom error rate metric | 0% target |

### Key metrics to watch in Grafana

| Metric | What it tells you |
|--------|-------------------|
| `http_req_duration avg` | Average response time — good for trends |
| `http_req_duration p(95)` | Tail latency — what 95% of users experience |
| `http_req_failed rate` | Error rate — should be near 0% |
| `http_reqs rate` | Throughput — requests per second |
| `vus` | Current active virtual users |
| `response_time { endpoint }` | Per-endpoint breakdown (custom metric) |

### Reading the k6 summary

```
✓ POST /projects status 201   ← check passed for all iterations
✗ GET /projects status 200    ← check failed — investigate
  ↳ 95% — ✓ 1900 / ✗ 100     ← 100 failures out of 2000

http_req_duration: avg=8ms p(95)=23ms max=228ms
http_req_failed:   5.00% 100 out of 2000
```

A `✗` check means the check condition was false for some iterations. Always cross-reference with the dispatcher logs and project-service logs to find the root cause.

---

## Known Issues & Root Causes

### IdentityServer signing key persistence

**Symptom:** 100% error rate — all requests return 401 Unauthorized. The k6 `setup()` login fails, so all VUs have `token = undefined`.

**Root cause:** IdentityServer signing keys stored in `/app/dataprotection` are lost if the container restarts, or the directory has incorrect file permissions, causing `CryptographicException: Permission denied` when trying to write new keys.

**Fix:**
```bash
# Fix permissions on the dataprotection volume
sudo chown -R 1000:1000 infrastructure/dataprotection
sudo chmod -R 755 infrastructure/dataprotection

# Restart the stack
docker compose down --remove-orphans && make dev
```

**Long-term fix:** Add `user: "1000:1000"` to the `authentication-service` in `docker-compose.yml` so the container always runs as the correct user and can write to its mounted volumes.

### GET /projects returns 404 for new users

**Symptom:** `GET /projects` returns 404 when the user has no projects yet.

**Root cause:** The project-service handler returns `NotFound()` when the list is empty instead of `Ok([])`.

**Fix (project-service controller):**
```csharp
var projects = await _repository.GetAllByUserId(userId);
return Ok(projects); // always 200, even when empty
```

**Workaround in k6:** Always `POST /projects` before `GET /projects` in each iteration — ensures the user always has at least one project.

### Docker Compose v1 compatibility errors

**Symptom:** `KeyError: 'ContainerConfig'` when running `make dev`.

**Root cause:** The system has both docker-compose v1 (`/usr/bin/docker-compose`) and Docker Compose v2 (`docker compose` plugin). v1 cannot handle newer image formats.

**Fix:**
```bash
# Remove v1
sudo apt remove docker-compose -y

# Update Makefile — replace docker-compose with docker compose (no hyphen)
# Verify v2 is available
docker compose version
```

---

## Observability

All k6 results stream to InfluxDB during the test and are visible in Grafana in real time.

### Grafana dashboards

| Dashboard | URL | Shows |
|-----------|-----|-------|
| k6 Load Testing | http://localhost:3000 | Avg response, error rate, throughput, VUs, per-endpoint breakdown |
| Service Metrics | http://localhost:3000 | Request rate, p95, error rate per service |

### Filtering by endpoint in Grafana

The custom `response_time` metric is tagged with `endpoint` and `service`:

```javascript
responseTime.add(duration, {
  endpoint: 'POST /projects',
  service:  'project-service',
});
```

In Grafana, group by the `endpoint` tag to see per-endpoint latency breakdowns.

### Correlating with service logs

Every request carries an `X-Correlation-ID` header generated by the dispatcher. Use it to trace a single k6 request across all service logs in Kibana:

```
http://localhost:5601
```

Filter: `X-Correlation-ID: <id from k6 output>`

---

## Roadmap

- [ ] Implement smoke test scenario (`smoke.js`)
- [ ] Implement stress test scenario (`stress.js`)
- [ ] Implement spike test scenario (`spike.js`)
- [ ] Implement soak test scenario (`soak.js`)
- [ ] Implement breakpoint test scenario (`breakpoint.js`)
- [ ] Add `make load-test-stress / spike / soak / breakpoint` targets to Makefile
- [ ] Add task endpoints to the test scenario (`POST /tasks`, `GET /tasks`)
- [ ] Add agent endpoints to the test scenario (`POST /agent/suggest_tasks`)
- [ ] Export per-run k6 JSON summaries to `docs/results/`
- [ ] Add CI integration — run smoke test on every pull request
- [ ] Persist IdentityServer keys properly to eliminate key-loss errors
- [ ] Fix `GET /projects` to return `200 []` instead of `404` for empty collections

---

## References

- [k6 Documentation](https://k6.io/docs)
- [k6 Test Types](https://k6.io/docs/test-types/introduction/)
- [Grafana k6 Dashboard](https://grafana.com/grafana/dashboards/2587-k6-load-testing-results/)
- [InfluxDB Output for k6](https://k6.io/docs/results-output/real-time/influxdb/)
