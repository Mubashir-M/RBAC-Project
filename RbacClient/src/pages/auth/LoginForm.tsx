import React, { useState } from "react";
import "./LoginForm.css";

interface LoginFormProps {
  onRegisterClick: () => void; // Function to call when "Register Now!" is clicked
}

const LoginForm: React.FC<LoginFormProps> = ({ onRegisterClick }) => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setError("");

    if (username === "user" && password === "password") {
      alert("Login successful!"); // Replace with actual auth logic (e.g., redirect)
    } else {
      setError("Invalid username or password.");
    }
  };

  return (
    <div className="login-card">
      <h2 className="login-title">Log In</h2>
      <form className="login-form" onSubmit={handleSubmit}>
        <div className="form-group">
          <input
            type="text"
            id="username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            placeholder="Username"
            required
          />
        </div>
        <div className="form-group">
          <input // Changed type to "password" for password field
            type="password"
            id="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="password"
            required
          />
        </div>
        {error && <p className="error-message">{error}</p>}
        <button type="submit" className="login-button">
          Log In
        </button>
      </form>

      <div className="register-section">
        <p>Don't have an account?</p>
        <button className="register-button" onClick={onRegisterClick}>
          Register Now!
        </button>
      </div>
    </div>
  );
};

export default LoginForm;
