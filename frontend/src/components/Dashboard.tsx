import React, { useState } from "react";
import { Link } from "react-router-dom";
import { Card, CardContent } from "./card";
import "../styles/Dashboard.css";
import SearchBar from "./Searchbar";
import Filter from "./Filter";

const Dashboard: React.FC = () => {
  const [selectedStatus, setSelectedStatus] = useState("");

  const handleStatusChange = (status: string) => {
    setSelectedStatus(status);
    console.log("Status gewijzigd naar:", status);
    // Hier kun je eventueel ook filteren op kaarten of data
  };

  return (
    <div className="container">
      <header className="header">
        <div className="logoWrapper">
          <Link to="/" className="logoLink">
            <div className="logoIcon">⬇</div>
            <h1 className="logoText">Lafeber</h1>
          </Link>
        </div>

        <div className="searchWrapper">
          <SearchBar
            searchTerm={""}
            onSearchChange={(term) => console.log(term)}
          />
        </div>

        <div className="filter">
          <Filter
            selectedStatus={selectedStatus}
            onStatusChange={handleStatusChange}
          />
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

          {["Bestellingen", "Producten", "Zendingen"].map((title) => (
            <Card key={title} className="cardDefault">
              <div className="cardTitleDefault">{title}</div>
              <Link to={`/${title.toLowerCase()}`} className="linkSecondary">
                → Bekijken
              </Link>
            </Card>
          ))}
        </div>
      </main>
    </div>
  );
};

export default Dashboard;
