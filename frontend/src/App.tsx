
import React from "react";
import { Card, CardContent } from "./card";
import "./App.css";

const Dashboard: React.FC = () => (
  <div className="container">
    <header className="header">
      <div className="logoArea">
        <div className="logoIcon">⬇</div>
        <h1 className="logoText">Lafeber</h1>
      </div>
    </header>

    <main className="main">
      <div className="navGrid">
        <Card className="cardUpdates">
          <div className="cardTitle">Updates</div>
          <div className="cardAction">
            <a href="#" className="linkPrimary">Bekijken</a>
            <div className="addIcon">＋</div>
          </div>
        </Card>

        {['Bestellingen', 'Producten', 'Zendingen'].map((title) => (
          <Card key={title} className="cardDefault">
            <div className="cardTitleDefault">{title}</div>
            <a href="#" className="linkSecondary">→ Bekijken</a>
          </Card>
        ))}
      </div>

      {/* Chart area removed; will add later */}
    </main>
  </div>
);

export default Dashboard;