import { useNavigate } from "react-router-dom";
import { defaultRouteByRole, roleOptions } from "../../../app/roles";

export function LoginPage({ role, onRoleChange }) {
  const navigate = useNavigate();

  function handleSubmit(event) {
    event.preventDefault();
    navigate(defaultRouteByRole[role] ?? "/dashboard/employee");
  }

  return (
    <section className="page-card login-card modern-login">
      <div className="login-hero">
        <p className="eyebrow">Contemporary UI Refresh</p>
        <h1>Welcome Back</h1>
        <p>Use this mocked sign-in to preview role-based panels, color system, and interaction layout.</p>
        <div className="hero-metrics">
          <article>
            <strong>24</strong>
            <span>Open Requests</span>
          </article>
          <article>
            <strong>3.2d</strong>
            <span>Avg Approval</span>
          </article>
          <article>
            <strong>91%</strong>
            <span>Policy Compliance</span>
          </article>
        </div>
      </div>

      <form className="form-grid login-form" onSubmit={handleSubmit}>
        <label>
          Work Email
          <input type="email" placeholder="employee@company.com" required />
        </label>

        <label>
          Password
          <input type="password" placeholder="********" required />
        </label>

        <label>
          Role Preview
          <select value={role} onChange={(event) => onRoleChange(event.target.value)}>
            {roleOptions.map((roleOption) => (
              <option key={roleOption.value} value={roleOption.value}>
                {roleOption.label}
              </option>
            ))}
          </select>
        </label>

        <button type="submit">Enter Workspace</button>
      </form>
    </section>
  );
}
