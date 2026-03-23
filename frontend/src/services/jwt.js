const roleClaimUri = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

export function decodeJwt(token) {
  if (!token) {
    return null;
  }

  const parts = token.split(".");
  if (parts.length < 2) {
    return null;
  }

  try {
    const payload = parts[1].replace(/-/g, "+").replace(/_/g, "/");
    const padded = payload + "=".repeat((4 - (payload.length % 4)) % 4);
    const json = atob(padded);
    return JSON.parse(json);
  } catch {
    return null;
  }
}

export function getRolesFromToken(token) {
  const payload = decodeJwt(token);
  if (!payload) {
    return [];
  }

  const roleValue = payload[roleClaimUri] ?? payload.role ?? payload.roles;
  if (!roleValue) {
    return [];
  }

  const roles = Array.isArray(roleValue) ? roleValue : [roleValue];
  return roles.map((x) => String(x));
}

export function hasRole(roles, role) {
  return roles.some((x) => x.toLowerCase() === role.toLowerCase());
}
