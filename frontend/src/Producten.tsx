// src/Zendingen.tsx
import React from "react";
import { Link } from "react-router-dom";
import "./Dashboard.css";            // shared styles like header, logo, etc.
import "./Producten.css";  // custom styles for this page

const Zendingen: React.FC = () => (
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
      <h2 className="pageTitle">Producten</h2>
      <div className="ProductenList">
        <div className="ProductenItem">#1001 – 23 mei 2025</div>
        <div className="ProductenItem">#1002 – 24 mei 2025</div>
        <div className="ProductenItem">#1003 – 25 mei 2025</div>
      </div>
    </main>
  </div>
);

export default Zendingen;
