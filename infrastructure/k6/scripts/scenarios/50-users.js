<<<<<<< HEAD
import { getToken, runScenario } from '../helpers.js';


export const options = {
  vus:      50,
  duration: '2m',
  thresholds: {
    http_req_duration: ['p(95)<2000'],
    http_req_failed:   ['rate<0.1'],
  },
};

export function setup() {
  return getToken();
}

export default function(data) {
  runScenario(data.token);
=======
import { getToken, runScenario } from '../helpers.js';


export const options = {
  vus:      50,
  duration: '2m',
  thresholds: {
    http_req_duration: ['p(95)<2000'],
    http_req_failed:   ['rate<0.1'],
  },
};

export function setup() {
  return getToken();
}

export default function(data) {
  runScenario(data.token);
>>>>>>> 05b451b (new_update)
}