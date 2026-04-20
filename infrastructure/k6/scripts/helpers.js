<<<<<<< HEAD
import http from 'k6/http';
import { check, sleep } from 'k6';

export function getToken() {
  const res = http.post('http://dispatcher-service:80/login', {
    grant_type:    'password',
    client_id:     'postman-client',
    client_secret: 'postman-secret',
    username:      'user@example.com',
    password:      'string',
    scope:         'openid profile project_fullpermission offline_access',
  }, {
    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
    tags: { service: 'authentication-service', endpoint: 'POST /login' }
  });

  return { token: res.json('access_token') };
}

export function runScenario(token) {
  const params = {
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type':  'application/json',
    },
    tags: {
      service: 'project-service',
      endpoint: 'GET /projects'
    }
  };

  const res = http.get('http://dispatcher-service:80/projects', params);

  check(res, {
    'status 200':             (r) => r.status === 200,
    'response time < 500ms':  (r) => r.timings.duration < 500,
  });

  sleep(1);
=======
import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate, Trend } from 'k6/metrics';
import { TEST_USER } from './load-test.js';

const errorRate = new Rate('error_rate');
const responseTime = new Trend('response_time', true);

const BASE_URL = 'http://dispatcher-service:80';

export function getToken() {
  // Register user (safe if already exists)
  http.post(
    `${BASE_URL}/users`,
    JSON.stringify({ email: TEST_USER.email, password: TEST_USER.password }),
    { headers: { 'Content-Type': 'application/json' } }
  );

  const res = http.post(
    `${BASE_URL}/login`,
    {
      grant_type:    'password',
      client_id:     'postman-client',
      client_secret: 'postman-secret',
      username:      TEST_USER.email,
      password:      TEST_USER.password,
      scope:         'openid profile project_fullpermission offline_access',
    },
    {
      headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
      tags: { service: 'authentication-service', endpoint: 'POST /login' },
    }
  );

  if (res.status !== 200) {
    throw new Error(`Login failed: ${res.status} — ${res.body}`);
  }

  return { token: res.json('access_token') };
}

export function runScenario(token) {
  const params = {
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
  };

  // 1. Create a project first
  const createRes = http.post(
    `${BASE_URL}/projects`,
    JSON.stringify({
      name: `Project ${Math.random().toString(36).substring(7)}`,
      description: `Description for project ${Math.random().toString(36).substring(7)}`,
    }),
    {
      ...params,
      tags: { service: 'project-service', endpoint: 'POST /projects' },
    }
  );

  check(createRes, {
    'POST /projects status 201': (r) => r.status === 201,
    'POST /projects < 1000ms':   (r) => r.timings.duration < 1000,
  });

  errorRate.add(createRes.status !== 201);
  responseTime.add(createRes.timings.duration, {
    endpoint: 'POST /projects',
    service: 'project-service',
  });

  
  sleep(1);

  const getRes = http.get(
    `${BASE_URL}/projects/me`,
    {
      ...params,
      tags: { service: 'project-service', endpoint: 'GET /projects/me' },
    }
  );

  check(getRes, {
    'GET /projects/me/ status 200': (r) => r.status === 200,
    'GET /projects/me/ < 500ms':    (r) => r.timings.duration < 500,
  });

  errorRate.add(getRes.status !== 200);
  responseTime.add(getRes.timings.duration, {
    endpoint: 'GET /projects/me',
    service: 'project-service',
  });

  sleep(1);
>>>>>>> 05b451b (new_update)
}