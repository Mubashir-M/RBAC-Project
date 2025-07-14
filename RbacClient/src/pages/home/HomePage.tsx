import React from "react";
import NavBar from "../navbar/NavBar";
import "./HomePage.css";
import { useSelector } from "react-redux";
import { type RootState } from "../../store/store";

const HomePage: React.FC = () => {
  const user = useSelector((state: RootState) => state.user.user);

  return (
    <div className="home-container">
      <NavBar />
      <p>
        Welcome to the home page, {user?.firstName} {user?.lastName}!!
      </p>
    </div>
  );
};

export default HomePage;
