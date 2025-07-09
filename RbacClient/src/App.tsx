import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import React, { useState, useEffect } from "react";
import "./App.css";
import AuthPage from "./pages/auth/AuthPage";
import HomePage from "./pages/home/HomePage";

function App() {
  const [isLogged, setIsLogged] = useState(true);

  useEffect(() => {
    setIsLogged(isLogged);
  }, [isLogged]);

  return (
    <BrowserRouter>
      <Routes>
        <Route
          path="/"
          element={isLogged ? <Navigate to="/Home" replace /> : <AuthPage />}
        />
        <Route
          path="/Home"
          element={isLogged ? <HomePage /> : <Navigate to="/" replace />}
        />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
