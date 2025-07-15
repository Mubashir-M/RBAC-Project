import React, { useEffect } from "react";
import "./AdminPage.css";
import { useAppDispatch, useAppSelector } from "../../features/hooks";
import { fetchUsers } from "../../features/usersListSlice";
import { type RootState } from "../../store/store";

const AdminPage: React.FC = () => {
  const dispatch = useAppDispatch();
  const user = useAppSelector((state: RootState) => state.user);
  const { users, loading, error } = useAppSelector(
    (state: RootState) => state.userList
  );

  useEffect(() => {
    if (user.user?.roles.some((role) => role.name === "Admin")) {
      dispatch(fetchUsers());
    }
  }, [dispatch, user]);

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
          <p>5(hard coded)</p> {/* Placeholder, replace with actual data */}
        </div>
      </div>

      {/* Quick Links */}
      <div className="quick-links">
        <h3>Quick Links</h3>
        <ul>
          <li>
            <a href="/users">Manage Users</a>
          </li>
          <li>
            <a href="/roles">Manage Roles</a>
          </li>
          <li>
            <a href="/reports">View Reports</a>
          </li>
          <li>
            <a href="/logs">System Logs</a>
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
