import React from "react";
import NavBar from "../navbar/NavBar";
import "./HomePage.css";

const HomePage: React.FC = () => {
  return (
    <div className="home-container">
      <NavBar />
      <p>Welcome to the home page!!</p>
    </div>
  );
};

export default HomePage;
