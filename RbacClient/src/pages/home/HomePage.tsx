import React from "react";
import "./HomePage.css";
import { useSelector } from "react-redux";
import { type RootState } from "../../store/store";
import AdminPage from "../admin/AdminPage";
import ManagerPage from "../manager/ManagerPage";
import UserPage from "../user/UserPage";

const HomePage: React.FC = () => {
  const user = useSelector((state: RootState) => state.user.user);

  return (
    <div className="home-container">
      {user?.roles.some((role) => role.name === "Admin") && <AdminPage />}
      {user?.roles.some((role) => role.name === "Manager") && <ManagerPage />}
      {user?.roles.some((role) => role.name === "User") && <UserPage />}
    </div>
  );
};

export default HomePage;
