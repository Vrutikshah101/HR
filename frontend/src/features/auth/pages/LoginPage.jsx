import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { apiClient } from "../../../services/apiClient";
import { saveToken } from "../../../services/tokenStorage";
import { getRolesFromToken } from "../../../services/jwt";
import { defaultRouteByRole, mapPrimaryRole } from "../../../app/roles";
import { trackActivity } from "../../../services/activityTracker";
import { UserIcon } from "../../../components/icons";

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
      trackActivity("LOGIN_SUCCESS", `Logged in as ${email}.`);
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
    <section className="auth-layout">
      <aside className="auth-showcase">
        <p className="auth-kicker">Leave Management System</p>
        <h1>Build faster approvals with one secure workspace</h1>
        <p>
          Track leave balances, approvals, and reports with role-based dashboards
          designed for employees, HR teams, and administrators.
        </p>
        <div className="auth-pill-row">
          <span>Live API</span>
          <span>Role-Based Access</span>
          <span>Audit Ready</span>
        </div>
      </aside>

      <form className="auth-card" onSubmit={handleSubmit}>
        <div className="auth-card-head">
          <div className="auth-logo">
            <UserIcon width="24" height="24" />
          </div>
          <div>
            <h2>Welcome Back</h2>
            <p>Sign in to continue to your dashboard</p>
          </div>
        </div>

        <label>
          Work Email
          <input type="email" value={email} onChange={(event) => setEmail(event.target.value)} required />
        </label>

        <label>
          Password
          <input type="password" value={password} onChange={(event) => setPassword(event.target.value)} required />
        </label>

        <div className="auth-row">
          <span className="muted-small">Need help? Contact your HR admin.</span>
          <button type="button" className="link-button">Forgot Password?</button>
        </div>

        {error ? <p className="error-text">{error}</p> : null}

        <button type="submit" className="login-submit" disabled={loading}>
          {loading ? "Signing In..." : "Login to Portal"}
        </button>

        <p className="support-line">Helpdesk: support@leave.local</p>
      </form>
    </section>
  );
}
