// src/Bestellingen.tsx
import React from "react";
import { Link } from "react-router-dom";
import "./Dashboard.css";            // shared styles like header, logo, etc.
import "./Bestellingen.css";  // custom styles for this page

const Bestellingen: React.FC = () => (
  <div className="container">
    <header className="header">
      <div className="logoArea">
        <Link to="/" className="logoLink">
          <div className="logoIcon">⬇</div>
          <h1 className="logoText">Lafeber</h1>
        </Link>
      </div>
    </header>

    <main className="main">
      <h2 className="pageTitle">Bestellingen</h2>
      <div className="orderList">
        <div className="orderItem">#1001 – 23 mei 2025</div>
        <div className="orderItem">#1002 – 24 mei 2025</div>
        <div className="orderItem">#1003 – 25 mei 2025</div>
      </div>
    </main>
  </div>
);

export default Bestellingen;
