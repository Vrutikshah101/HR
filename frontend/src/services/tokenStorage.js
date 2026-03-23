export const tokenKey = "leave_access_token";

export function saveToken(token) {
  localStorage.setItem(tokenKey, token);
}

export function getToken() {
  return localStorage.getItem(tokenKey);
}

export function clearToken() {
  localStorage.removeItem(tokenKey);
}
