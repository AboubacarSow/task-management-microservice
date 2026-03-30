import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate, Trend } from 'k6/metrics';

// Custom metrics
const errorRate    = new Rate('error_rate');
const responseTime = new Trend('response_time', true);

// Test configuration — stages simulate ramp up
export const options = {
  stages: [
    { duration: '30s', target: 50  },  // ramp up to 50 users
    { duration: '1m',  target: 50  },  // stay at 50
    { duration: '30s', target: 100 },  // ramp up to 100
    { duration: '1m',  target: 100 },  // stay at 100
    { duration: '30s', target: 200 },  // ramp up to 200
    { duration: '1m',  target: 200 },  // stay at 200
    { duration: '30s', target: 500 },  // ramp up to 500
    { duration: '1m',  target: 500 },  // stay at 500
    { duration: '30s', target: 0   },  // ramp down
  ],
  thresholds: {
    http_req_duration: ['p(95)<2000'],  // 95% of requests under 2s
    error_rate:        ['rate<0.1'],    // error rate under 10%
  },
};

// Get token once before tests
export function setup() {
  const loginRes = http.post('http://localhost:80/login', {
    grant_type:    'password',
    client_id:     'postman-client',
    client_secret: 'postman-secret',
    username:      'johndoe@gmail.com',
    password:      '123456',
    scope:         'openid profile project_fullpermission user_fullpermission offline_access',
  }, {
    headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
  });

  return { token: loginRes.json('access_token') };
}

export default function (data) {
  const params = {
    headers: {
      'Authorization': `Bearer ${data.token}`,
      'Content-Type':  'application/json',
    },
  };

  const getProjects = http.get('http://localhost:80/projects', params);

  check(getProjects, {
    'GET /projects status 200': (r) => r.status === 200,
    'GET /projects response time < 500ms': (r) => r.timings.duration < 500,
  });

  errorRate.add(getProjects.status !== 200);
  responseTime.add(getProjects.timings.duration);

  sleep(1);


  const createProject = http.post(
    'http://localhost:80/projects',
    JSON.stringify({
      name:        `Load Test Project ${Date.now()}`,
      description: 'Created during load test'
    }),
    params
  );

  check(createProject, {
    'POST /projects status 201': (r) => r.status === 201,
    'POST /projects response time < 1000ms': (r) => r.timings.duration < 1000,
  });

  errorRate.add(createProject.status !== 201);
  responseTime.add(createProject.timings.duration);

  sleep(1);
}