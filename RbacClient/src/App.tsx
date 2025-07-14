import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import "./App.css";
import AuthPage from "./pages/auth/AuthPage";
import HomePage from "./pages/home/HomePage";
import { type RootState } from "./store/store";
import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { setCredentials } from "./features/userSlice";

function App() {
  const token = useSelector((state: RootState) => state.user.token);
  const dispatch = useDispatch();

  useEffect(() => {
    const tokenInStorage = localStorage.getItem("token");
    const userInStorage = localStorage.getItem("user");

    if (tokenInStorage && userInStorage && !token) {
      dispatch(
        setCredentials({
          token: tokenInStorage,
          user: JSON.parse(userInStorage),
        })
      );
    }
  }, [dispatch, token]);

  const isLogged = !!token;

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
