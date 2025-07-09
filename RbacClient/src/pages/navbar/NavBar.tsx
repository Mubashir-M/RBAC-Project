import React from "react";
import { Link } from "react-router-dom";
import "./NavBar.css";

const NavBar: React.FC = () => {
  // Temporary mock user object (replace later with API response)
  const user = {
    username: "Mikko",
    role: "Admin", // Try "User" or "Manager" for testing other roles
  };
  return (
    <nav className="navbar">
      <Link to="/Home">Home</Link>
      <Link to="/Home">Dashboard</Link>
      {user.role === "Admin" && (
        <>
          <Link to="/Home">Users</Link>
          <Link to="/Home">Roles</Link>
          <Link to="/Home">Audit Logs</Link>
        </>
      )}
      {user.role === "Manager" && (
        <>
          <Link to="/Home">Reports</Link>
        </>
      )}
      {user.role === "User" && (
        <>
          <Link to="/Home">My Tasks</Link>
        </>
      )}
      <Link to="/Home">Profile</Link>
      <button>Logout</button>
    </nav>
  );
};

export default NavBar;
