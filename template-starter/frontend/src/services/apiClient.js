const API_BASE = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5001";

export async function apiGet(path) {
  const response = await fetch(`${API_BASE}${path}`);
  if (!response.ok) {
    throw new Error(`Request failed: ${response.status}`);
  }
  return response.json();
}
