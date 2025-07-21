import React, { useState } from "react";
import api from "../../api/axios";

interface RegisterFormProps {
  onLoginClick: () => void; // Function to call when "Login here!" is clicked
}

const RegisterForm: React.FC<RegisterFormProps> = ({ onLoginClick }) => {
  // State variables for all required fields from the User model (client-side relevant)
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [username, setUsername] = useState(""); // New field
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [error, setError] = useState("");

  const handleSubmit = async (e: React.FormEvent) => {
    // Made async for potential API call
    e.preventDefault();
    setError("");

    if (password !== confirmPassword) {
      setError("Passwords do not match.");
      return;
    }

    // Basic client-side validation
    if (username.length < 8 || username.length > 50) {
      setError("Username must be between 8 and 50 characters.");
      return;
    }

    // In a real application, you'd send this data to your backend API
    // For example, using fetch or axios:
    try {
      await api.post("auth/register", {
        firstName,
        lastName,
        username,
        email,
        password,
      });

      // --- RESET FORM FIELDS HERE ---
      setFirstName("");
      setLastName("");
      setUsername("");
      setEmail("");
      setPassword("");
      setConfirmPassword("");
      setError(""); // Also clear any previous error message

      alert("Registration successful! Please log in.");
      onLoginClick();
    } catch (err) {
      console.error("Registration API error:", err);
      setError("Network error or server unavailable.");
    }
  };

  return (
    <div className="login-card">
      {" "}
      {/* Re-using login-card styles for consistent look */}
      <h2 className="login-title">Register Account</h2>
      <form onSubmit={handleSubmit}>
        {/* First Name */}
        <div className="form-group">
          <label htmlFor="firstName">First Name:</label>
          <input
            type="text"
            id="firstName"
            value={firstName}
            onChange={(e) => setFirstName(e.target.value)}
            required
            maxLength={50} // Max length as per your model
            autoComplete="given-name"
          />
        </div>

        {/* Last Name */}
        <div className="form-group">
          <label htmlFor="lastName">Last Name:</label>
          <input
            type="text"
            id="lastName"
            value={lastName}
            onChange={(e) => setLastName(e.target.value)}
            required
            maxLength={50} // Max length as per your model
            autoComplete="family-name"
          />
        </div>

        {/* Username */}
        <div className="form-group">
          <label htmlFor="username">Username:</label>
          <input
            type="text"
            id="username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
            minLength={8} // Min length as per your model
            maxLength={50} // Max length as per your model
            autoComplete="username"
          />
        </div>

        {/* Email */}
        <div className="form-group">
          <label htmlFor="email">Email:</label>
          <input
            type="email" // Use type="email" for built-in browser validation
            id="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
            maxLength={100} // Max length as per your model
            autoComplete="email"
          />
        </div>

        {/* Password */}
        <div className="form-group">
          <label htmlFor="password">Password:</label>
          <input
            type="password"
            id="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            autoComplete="new-password"
          />
        </div>

        {/* Confirm Password */}
        <div className="form-group">
          <label htmlFor="confirm-password">Confirm Password:</label>
          <input
            type="password"
            id="confirm-password"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            required
            autoComplete="new-password"
          />
        </div>

        {error && <p className="error-message">{error}</p>}

        <button type="submit" className="login-button">
          {" "}
          {/* Re-using login-button styles for primary action */}
          Register
        </button>
      </form>
      <div className="register-section">
        {" "}
        {/* Re-using this class for separator */}
        <p>Already have an account?</p>
        <button className="register-button" onClick={onLoginClick}>
          {" "}
          {/* Re-using register-button styles for secondary action */}
          Log In Here!
        </button>
      </div>
    </div>
  );
};

export default RegisterForm;
