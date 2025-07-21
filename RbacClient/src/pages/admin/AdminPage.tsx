import React from "react";
import { Link } from "react-router-dom";
import "./AdminPage.css";
import { useAppSelector } from "../../features/hooks";
import { type RootState } from "../../store/store";

const AdminPage: React.FC = () => {
  const user = useAppSelector((state: RootState) => state.user);
  const { users, loading, error } = useAppSelector(
    (state: RootState) => state.userList
  );

  if (loading) {
    return <div>Loading users...</div>;
  }

  if (error) {
    return <div>Error: {error}</div>;
  }
  return (
    <div className="admin-panel">
      <h1 className="panel-title">Admin Panel</h1>
      <h2 className="admin-name">
        Current Admin: <span>{user?.user?.username}</span>
      </h2>

      {/* Admin Stats Section */}
      <div className="overview">
        <div className="card">
          <h3>Total Users</h3>
          <p>{users.length}</p>
        </div>
        <div className="card">
          <h3>Active Users</h3>
          <p>{users.filter((user) => user.isActive).length}</p>
        </div>
        <div className="card">
          <h3>Pending Approvals</h3>
          <p>
            {users.filter((user) => user.roles[0].name == "Pending").length}
          </p>{" "}
          {/* Placeholder, replace with actual data */}
        </div>
      </div>

      {/* Quick Links */}
      <div className="quick-links">
        <h3>Quick Links</h3>
        <ul>
          <li>
            <Link to="/users">Manage Users</Link>
          </li>
          <li>
            <Link to="/roles">Manage Roles</Link>
          </li>
          <li>
            <Link to="/logs">System Logs</Link>
          </li>
        </ul>
      </div>

      {/* Recent Activity */}
      <div className="recent-activity">
        <h3>Recent Activity</h3>
        <ul>
          <li>User JohnDoe registered</li>
          <li>Role "Manager" updated for JaneDoe</li>
          <li>System health report generated</li>
        </ul>
      </div>

      {/* Notifications/Alerts */}
      <div className="alerts">
        <h3>System Alerts</h3>
        <ul>
          <li>Critical error: Database connection issue</li>
          <li>Update required: Security patch pending</li>
        </ul>
      </div>
    </div>
  );
};

export default AdminPage;
