import React, { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { Card } from "./card";
import "../styles/Dashboard.css";
import SearchBar from './Searchbar';
import Filter from './Filter';

interface Order {
  id: number;
  status: string;
  customer: {
    bedrijfsNaam: string;
  };
  orderDate: string;
}

const Dashboard: React.FC = () => {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedStatus, setSelectedStatus] = useState("");

  useEffect(() => {
    // Replace with your actual API call
    const mockData: Order[] = [
      { id: 1001, status: "DELIVERED", customer: { bedrijfsNaam: "Goldia Rolland" }, orderDate: "2025-05-23" },
      // Add more mock data as needed
    ];

    setOrders(mockData);
    setLoading(false);
  }, []);

  const filteredOrders = orders.filter(order => {
    const matchesSearch =
      order.id.toString().includes(searchTerm) ||
      order.customer.bedrijfsNaam.toLowerCase().includes(searchTerm.toLowerCase());

    const matchesStatus =
      !selectedStatus ||
      order.status.toLowerCase() === selectedStatus.toLowerCase();

    return matchesSearch && matchesStatus;
  });

  return (
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

        <div className="heroControls">
          <SearchBar
            searchTerm={searchTerm}
            onSearchChange={setSearchTerm}
          />
          <Filter
            selectedStatus={selectedStatus}
            onStatusChange={setSelectedStatus}
          />
        </div>
      </header>

      <main className="main">
        {/* Navigation cards only - statistics cards removed */}
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

        {/* Recent orders section */}
        {!loading && filteredOrders.length > 0 && (
          <div className="recent-orders">
            <h2>Recent Orders</h2>
            <div className="orders-list">
              {filteredOrders.slice(0, 5).map(order => (
                <div key={order.id} className="order-item">
                  <span>Order #{order.id}</span>
                  <span className={`status ${order.status.toLowerCase()}`}>
                    {order.status}
                  </span>
                </div>
              ))}
            </div>
          </div>
        )}
      </main>
    </div>
  );
};

export default Dashboard;