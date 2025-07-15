import React from "react";
import { Outlet } from "react-router-dom";
import NavBar from "../navbar/NavBar";
import "./DashboardLayout.css";

const DashboardLayout: React.FC = () => {
  return (
    <>
      <NavBar />
      <main className="page-content">
        <Outlet />
      </main>
    </>
  );
};

export default DashboardLayout;
