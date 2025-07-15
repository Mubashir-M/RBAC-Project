import React from "react";
import { Link } from "react-router-dom";
import "./NavBar.css";
import { useDispatch } from "react-redux";
import { logout } from "../../features/userSlice";
import { useSelector } from "react-redux";
import { type RootState } from "../../store/store";

const NavBar: React.FC = () => {
  const dispatch = useDispatch();
  const user = useSelector((state: RootState) => state.user.user);

  const handleLogout = () => {
    dispatch(logout());
    localStorage.removeItem("token");
    localStorage.removeItem("user");
  };

  return (
    <nav className="navbar">
      <Link to="/Home">Home</Link>
      {user?.roles.some((role) => role.name === "Admin") && (
        <>
          <Link to="/users">Users</Link>
          <Link to="/roles">Roles</Link>
          <Link to="/permissions">Permissions</Link>
          <Link to="/logs">Logs</Link>
        </>
      )}
      {user?.roles.some((role) => role.name === "Manager") && (
        <>
          <Link to="/Home">Reports</Link>
        </>
      )}
      {user?.roles.some((role) => role.name === "User") && (
        <>
          <Link to="/Home">My Tasks</Link>
        </>
      )}
      <Link to="/Home">Settings</Link>
      <button onClick={handleLogout}>Logout</button>
    </nav>
  );
};

export default NavBar;
