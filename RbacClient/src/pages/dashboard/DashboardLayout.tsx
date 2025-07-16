// DashboardLayout.tsx
import React, { useEffect } from "react";
import { Outlet } from "react-router-dom";
import { useAppDispatch, useAppSelector } from "../../features/hooks";
import { type RootState } from "../../store/store";
import { fetchUsers } from "../../features/usersListSlice";
import { fetchRoles } from "../../features/roleSlice";

const DashboardLayout: React.FC = () => {
  const dispatch = useAppDispatch();
  const currentUser = useAppSelector((state: RootState) => state.user);
  const { users } = useAppSelector((state: RootState) => state.userList); // Get users state
  const { roles } = useAppSelector((state: RootState) => state.roles); // Get roles state

  useEffect(() => {
    // Only proceed if currentUser.user exists and is an Admin
    if (currentUser.user?.roles.some((role) => role.name === "Admin")) {
      // Fetch users only if the users list is empty (initial load)
      if (users.length === 0) {
        console.log("DashboardLayout: Dispatching initial fetchUsers.");
        dispatch(fetchUsers());
      }
      // Fetch roles only if the roles list is empty (initial load)
      if (roles.length === 0) {
        console.log("DashboardLayout: Dispatching initial fetchRoles.");
        dispatch(fetchRoles());
      }
    }
  }, [dispatch, currentUser.user, users.length, roles.length]);

  return (
    <div>
      {/* Your dashboard navigation, sidebar, etc. */}
      <Outlet />{" "}
      {/* This renders the nested routes like UsersPage, AdminPage */}
    </div>
  );
};

export default DashboardLayout;
