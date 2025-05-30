// src/Dashboard.tsx
import React from "react";
import { Link } from "react-router-dom";
import { Card, CardContent } from "./card";
import "../styles/Dashboard.css";
import SearchBar from './Searchbar';
import Filter from './Filter';
import logo from '../public/logo_witte_letters.png'; // or '../assets/logo.png' if renamed

const Dashboard: React.FC = () => (
  <div className="container">
    <header
      className="hero"
      style={{
        backgroundImage: `url("/Warehouse.jpg")`,
        backgroundSize: "cover",
        backgroundPosition: "center",
      }}
    >
      <div className="overlay">
        <div className="logoWrapper">
          <Link to="/" className="logoLink">
            <img src="/logo_witte_letters.png" alt="Lafeber logo" className="logoImage" />
          </Link>
        </div>
      </div>

      {/* NIEUW: zoekbalk en filter rechtsboven */}
      <div className="heroControls">
        <SearchBar
          searchTerm={""}
          onSearchChange={(term) => console.log(term)}
        />
        <Filter
          selectedStatus={""}
          onStatusChange={(status) => console.log(status)}
        />
      </div>
    </header>


    <main className="main">
      <div className="navGrid">
        {["Updates", "Bestellingen", "Producten", "Zendingen"].map((title) => (
          <Card key={title} className={`cardDefault ${title === "Updates" ? "cardUpdates" : ""}`}>
            <div className="cardTitleDefault">{title}</div>
            <Link to={`/${title.toLowerCase()}`} className="linkSecondary">
              → Bekijken
            </Link>
            {title === "Updates" && (
              <div className="addIcon">＋</div>
            )}
          </Card>
        ))}
      </div>
    </main>
  </div>
);

export default Dashboard;
