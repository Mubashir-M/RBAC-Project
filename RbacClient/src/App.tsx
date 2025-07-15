import { BrowserRouter, Routes, Route } from "react-router-dom";
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

  if (!isLogged) {
    return <AuthPage />;
  }

  return (
    <BrowserRouter>
      <Routes>
        {/* Protected routes */}
        {isLogged && (
          <Route element={<DashboardLayout />}>
            <Route path="/home" element={<HomePage />} />
            <Route path="/users" element={<UsersPage />} />
            <Route path="/roles" element={<RolesPage />} />
            <Route path="/permissions" element={<PermissionsPage />} />
            <Route path="/logs" element={<LogsPage />} />
            {/* Add more routes as needed */}
          </Route>
        )}
      </Routes>
    </BrowserRouter>
  );
}

export default App;
