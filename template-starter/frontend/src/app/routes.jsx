import { Link, Route, Routes } from "react-router-dom";
import SamplePage from "../features/sample/pages/SamplePage";

export function AppRoutes() {
  return (
    <div className="app-shell">
      <header className="topbar">
        <h1>Template Starter</h1>
        <nav>
          <Link to="/">Home</Link>
          <Link to="/sample">Sample</Link>
        </nav>
      </header>
      <main className="content">
        <Routes>
          <Route path="/" element={<p>Start building your modules here.</p>} />
          <Route path="/sample" element={<SamplePage />} />
        </Routes>
      </main>
    </div>
  );
}
