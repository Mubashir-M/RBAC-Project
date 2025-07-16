import React, { useState } from "react";
import { useAppDispatch, useAppSelector } from "../../features/hooks";
import { type RootState } from "../../store/store";
import { fetchUsers, updateUserRole } from "../../features/usersListSlice";

import "./UsersPage.css";

const UsersPage: React.FC = () => {
  const [search, setSearch] = useState("");
  const dispatch = useAppDispatch();
  const currentUser = useAppSelector((state: RootState) => state.user);
  const roles = useAppSelector((state: RootState) => state.roles.roles);
  const { users, loading, error } = useAppSelector(
    (state: RootState) => state.userList
  );

  const filteredUsers = users.filter((u) =>
    `${u.username} ${u.firstName} ${u.lastName} ${u.email}`
      .toLowerCase()
      .includes(search.toLowerCase())
  );

  const handleRoleChange = async (
    userId: number,
    newRoleId: number,
    username: string,
    currentRoleName?: string
  ) => {
    const selectedRole = roles.find((r) => r.roleId === newRoleId);

    if (!selectedRole || selectedRole.name === currentRoleName) return;

    const confirmChange = window.confirm(
      `Change ${username}'s role to ${selectedRole.name}?`
    );

    if (!confirmChange) return;
    const newRoleName = selectedRole.name;
    const resultAction = await dispatch(
      updateUserRole({ userId, newRoleId, newRoleName })
    );

    if (updateUserRole.fulfilled.match(resultAction)) {
      dispatch(fetchUsers());
    } else {
      console.error("Failed to update user role");
    }
  };

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
        Current Admin: <span>{currentUser?.user?.username}</span>
      </h2>

      <input
        type="text"
        placeholder="Search users..."
        value={search}
        onChange={(e) => setSearch(e.target.value)}
        className="search-input"
      />

      <div className="table-container">
        <table className="user-table">
          <thead>
            <tr>
              <th>Username</th>
              <th>First Name</th>
              <th>Last Name</th>
              <th>Email</th>
              <th>Roles</th>
            </tr>
          </thead>
          <tbody>
            {filteredUsers.length === 0 ? (
              <tr>
                <td colSpan={5} className="no-users">
                  No users found.
                </td>
              </tr>
            ) : (
              filteredUsers.map((user) => (
                <tr key={`${user.userId}-${user.username}`}>
                  <td>{user.username}</td>
                  <td>{user.firstName}</td>
                  <td>{user.lastName}</td>
                  <td>{user.email}</td>
                  <td>
                    <select
                      value={user.roles[0].roleId || ""}
                      onChange={(e) =>
                        handleRoleChange(
                          user.userId,
                          Number(e.target.value),
                          user.username,
                          user.roles[0]?.name
                        )
                      }
                    >
                      {roles.map((role) => (
                        <option key={role.roleId} value={role.roleId}>
                          {role.name}
                        </option>
                      ))}
                    </select>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default UsersPage;
