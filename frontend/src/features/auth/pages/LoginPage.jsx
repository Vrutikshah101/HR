import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { apiClient } from "../../../services/apiClient";
import { saveToken } from "../../../services/tokenStorage";
import { getRolesFromToken } from "../../../services/jwt";
import { defaultRouteByRole, mapPrimaryRole } from "../../../app/roles";

export function LoginPage({ onLoginSuccess }) {
  const navigate = useNavigate();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  async function handleSubmit(event) {
    event.preventDefault();
    setError("");
    setLoading(true);

    try {
      const response = await apiClient.post("/auth/login", { email, password });
      const token = response.data?.accessToken;

      if (!token) {
        throw new Error("Token not found in login response.");
      }

      saveToken(token);
      const roles = getRolesFromToken(token);
      onLoginSuccess(roles);

      const route = defaultRouteByRole(mapPrimaryRole(roles));
      navigate(route, { replace: true });
    } catch (err) {
      const message = err.response?.data?.message ?? "Login failed. Check credentials and try again.";
      setError(message);
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className="page-card login-card modern-login">
      <div className="login-hero">
        <p className="eyebrow">Secure Sign In</p>
        <h1>Welcome Back</h1>
        <p>Login with your credentials. Dashboard access is assigned automatically by your role.</p>
      </div>

      <form className="form-grid login-form" onSubmit={handleSubmit}>
        <label>
          Work Email
          <input type="email" value={email} onChange={(event) => setEmail(event.target.value)} required />
        </label>

        <label>
          Password
          <input type="password" value={password} onChange={(event) => setPassword(event.target.value)} required />
        </label>

        {error ? <p className="error-text">{error}</p> : null}

        <button type="submit" disabled={loading}>{loading ? "Signing In..." : "Login"}</button>
      </form>
    </section>
  );
}
