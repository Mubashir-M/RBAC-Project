import React, { useState } from "react";
import "./LoginForm.css";
import api from "../../api/axios";
import { setCredentials } from "../../features/userSlice";
import { useDispatch } from "react-redux";

interface LoginFormProps {
  onRegisterClick: () => void; // Function to call when "Register Now!" is clicked
}

const LoginForm: React.FC<LoginFormProps> = ({ onRegisterClick }) => {
  const dispatch = useDispatch();
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");

    try {
      const loginRes = await api.post("auth/login", {
        username,
        password,
      });

      const { token, user } = loginRes.data;

      if (token && user) {
        dispatch(setCredentials({ user, token }));
        localStorage.setItem("token", token);
        localStorage.setItem("user", JSON.stringify(user));
        setUsername("");
        setPassword("");
      }
    } catch (err) {
      console.error("Login failed:", err);
      setError("Invalid username or password.");
    }
  };

  return (
    <div className="login-card">
      <h2 className="login-title">Log In</h2>
      <form className="login-form" onSubmit={handleLogin}>
        <div className="form-group">
          <input
            type="text"
            id="username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            placeholder="Username"
            required
            autoComplete="username"
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
            autoComplete="current-password"
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
