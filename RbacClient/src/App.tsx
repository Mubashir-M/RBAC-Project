import { Navigate, Routes, Route } from "react-router-dom";
import "./App.css";
import AuthPage from "./pages/auth/AuthPage";
import HomePage from "./pages/home/HomePage";
import LogsPage from "./pages/logs/LogsPage";
import PermissionsPage from "./pages/permissions/PermissionsPage";
import DashboardLayout from "./pages/dashboard/DashboardLayout";
import UsersPage from "./pages/users/UsersPage";
import RolesPage from "./pages/roles/RolesPage";
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
    <Routes>
      {/* Public/Auth Route */}
      <Route
        path="/auth"
        element={isLogged ? <Navigate to="/home" replace /> : <AuthPage />}
      />

      {/* Redirect root to /auth if not logged in, or /home if logged in */}
      <Route
        path="/"
        element={
          isLogged ? (
            <Navigate to="/home" replace />
          ) : (
            <Navigate to="/auth" replace />
          )
        }
      />

      {/* Protected routes (rendered only if isLogged is true) */}
      {isLogged ? (
        <Route element={<DashboardLayout />}>
          <Route path="/home" element={<HomePage />} />
          <Route path="/users" element={<UsersPage />} />
          <Route path="/roles" element={<RolesPage />} />
          <Route path="/permissions" element={<PermissionsPage />} />
          <Route path="/logs" element={<LogsPage />} />
          {/* Add more routes as needed */}
          {/* Catch-all for logged-in users if they hit an unknown protected route */}
          <Route path="*" element={<Navigate to="/home" replace />} />
        </Route>
      ) : (
        // Catch-all for non-logged-in users if they try to access protected routes directly
        <Route path="*" element={<Navigate to="/auth" replace />} />
      )}
    </Routes>
  );
}

export default App;
