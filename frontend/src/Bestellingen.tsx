// src/Bestellingen.tsx
import React from "react";
import "./Dashboard";           // gebruikt nogsteeds de header 
import "./Bestellingen.css";  // custom styles voor deze page

const Bestellingen: React.FC = () => (
  <div className="container">
    <header className="header">
      <div className="logoArea">
        <div className="logoIcon">⬇</div>
        <h1 className="logoText">Lafeber</h1>
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
