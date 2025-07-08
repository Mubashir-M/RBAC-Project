import React, { useState } from "react";
import "./AuthPage.css";
import LoginForm from "./LoginForm";
import RegisterForm from "./RegisterForm";

const AuthPage: React.FC = () => {
  // State to determine which form to show: true for register, false for login
  const [isRegisterMode, setIsRegisterMode] = useState(false);

  const handleRegisterClick = () => {
    setIsRegisterMode(true);
  };

  const handleLoginClick = () => {
    setIsRegisterMode(false);
  };

  return (
    <div className="login-split-container">
      {/* Left Half (Welcome Message) */}
      <div className="left">
        <h1>Welcome to Accessflow</h1>
        <h3>Streamlined Role-Based Access.</h3>
      </div>

      {/* Right Half (Conditionally rendered form) */}
      <div className="right">
        {isRegisterMode ? (
          <RegisterForm onLoginClick={handleLoginClick} />
        ) : (
          <LoginForm onRegisterClick={handleRegisterClick} />
        )}
      </div>
    </div>
  );
};

export default AuthPage;
