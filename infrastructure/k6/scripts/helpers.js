import http from 'k6/http';
import { check, sleep } from 'k6';

export function getToken() {
  const res = http.post('http://localhost:8080/login', {
    grant_type:    'password',
    client_id:     'postman-client',
    client_secret: 'postman-secret',
    username:      'user@example.com',
    password:      'string',
    scope:         'openid profile project_fullpermission offline_access',
  }, {
    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
    tags: { service: 'auth-service', endpoint: 'POST /login' }
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

  const res = http.get('http://dispatcher-service:8080/projects', params);

  check(res, {
    'status 200':             (r) => r.status === 200,
    'response time < 500ms':  (r) => r.timings.duration < 500,
  });

  sleep(1);
}