import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate, Trend } from 'k6/metrics';

const errorRate = new Rate('error_rate');
const responseTime = new Trend('response_time', true);

const BASE_URL = 'http://dispatcher-service:8080';

export const options = {
  stages: [
    { duration: '30s', target: 50 },
    { duration: '1m',  target: 50 },
    { duration: '30s', target: 100 },
    { duration: '1m',  target: 100 },
    { duration: '30s', target: 200 },
    { duration: '1m',  target: 200 },
    { duration: '30s', target: 500 },
    { duration: '1m',  target: 500 },
    { duration: '30s', target: 0 },
  ],
  thresholds: {
    http_req_duration: ['p(95)<2000'],
    http_req_failed: ['rate<0.1'],
    error_rate: ['rate<0.1'],
  },
};

export function setup() {
  const loginRes = http.post(
    `${BASE_URL}/login`,
    {
      grant_type: 'password',
      client_id: 'postman-client',
      client_secret: 'postman-secret',
      username: 'johndoe@gmail.com',
      password: '123456',
      scope: 'openid profile project_fullpermission user_fullpermission offline_access',
    },
    {
      headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
      tags: { endpoint: 'login', service: 'dispatcher' },
    }
  );

  check(loginRes, {
    'login status 200': (r) => r.status === 200,
    'login token exists': (r) => !!r.json('access_token'),
  });

  return { token: loginRes.json('access_token') };
}

export default function (data) {
  const params = {
    headers: {
      Authorization: `Bearer ${data.token}`,
      'Content-Type': 'application/json',
    },
    tags: {
      service: 'dispatcher',
    },
  };

  const getProjects = http.get(`${BASE_URL}/projects`, {
    ...params,
    tags: { ...params.tags, endpoint: 'GET /projects' },
  });

  check(getProjects, {
    'GET /projects status 200': (r) => r.status === 200,
    'GET /projects < 500ms': (r) => r.timings.duration < 500,
  });

  errorRate.add(getProjects.status !== 200);
  responseTime.add(getProjects.timings.duration, { endpoint: 'GET /projects', service: 'dispatcher' });

  sleep(1);

  const createProject = http.post(
    `${BASE_URL}/projects`,
    JSON.stringify({
      name: `Load Test Project ${Date.now()}`,
      description: 'Created during load test',
    }),
    {
      ...params,
      tags: { ...params.tags, endpoint: 'POST /projects' },
    }
  );

  check(createProject, {
    'POST /projects status 201': (r) => r.status === 201,
    'POST /projects < 1000ms': (r) => r.timings.duration < 1000,
  });

  errorRate.add(createProject.status !== 201);
  responseTime.add(createProject.timings.duration, { endpoint: 'POST /projects', service: 'dispatcher' });

  sleep(1);
}