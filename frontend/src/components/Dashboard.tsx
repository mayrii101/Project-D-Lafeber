import React, { useEffect, useState } from "react";
import { Card } from "./card";
import "../styles/Dashboard.css";
import SearchBar from "./Searchbar";
import Filter from "./Filter";

interface Order {
  id: number;
  customerId: number;
  customer: {
    id: number;
    bedrijfsNaam: string;
    contactPersoon: string;
    email: string;
    telefoonNummer: string;
    adres: string;
    isDeleted: boolean;
  };
  productLines: Array<{
    productId: number;
    productName: string;
    quantity: number;
    price: number;
    product: {
      weightKg: number;
    };
  }>;
  status: string;
  orderDate: string;
  expectedDeliveryDate: string;
  actualDeliveryDate?: string;
  deliveryAddress: string;
  totalWeight: number;
  isDeleted: boolean;
}

const Dashboard: React.FC = () => {
  const [orders, setOrders] = useState<Order[]>([]);
  const [filteredOrders, setFilteredOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedStatus, setSelectedStatus] = useState("");
  const [showBestellingen, setShowBestellingen] = useState(false);
  const [selectedOrder, setSelectedOrder] = useState<Order | null>(null);
  const [showProducten, setShowProducten] = useState(false);
  const [showZendingen, setShowZendingen] = useState(false);

  useEffect(() => {
    fetch("http://localhost:5000/api/order")
      .then((res) => res.json())
      .then((data) => {
        setOrders(data);
        setFilteredOrders(data);
        setLoading(false);
      })
      .catch((err) => {
        console.error("Failed to fetch orders:", err);
        setLoading(false);
      });
  }, []);

  useEffect(() => {
    const filtered = orders.filter((order) => {
      const lowerSearch = searchTerm.toLowerCase();

      const matchesSearch =
        order.id.toString().includes(searchTerm) ||
        order.status.toLowerCase().includes(lowerSearch) ||
        order.customer.bedrijfsNaam.toLowerCase().includes(lowerSearch) ||
        order.customer.email.toLowerCase().includes(lowerSearch);

      const matchesStatus =
        !selectedStatus ||
        order.status.toLowerCase() === selectedStatus.toLowerCase();

      return matchesSearch && matchesStatus;
    });

    const sorted = [...filtered].sort(
      (a, b) =>
        new Date(b.orderDate).getTime() - new Date(a.orderDate).getTime()
    );

    setFilteredOrders(sorted);
  }, [searchTerm, orders, selectedStatus]);

  const stats = {
    total: filteredOrders.length,
    delivered: filteredOrders.filter((o) => o.status.toLowerCase() === "delivered").length,
    inTransit: filteredOrders.filter((o) => o.status.toLowerCase() === "shipped").length,
    cancelled: filteredOrders.filter((o) => o.status.toLowerCase() === "cancelled").length,
  };

  const handleOrderClick = (order: Order) => {
    setSelectedOrder(order);
    setShowBestellingen(true);
  };

  const closeModal = () => {
    setShowBestellingen(false);
    setShowProducten(false);
    setShowZendingen(false);
    setSelectedOrder(null);
  };

  return (
    <div className="container">
      <header className="hero" style={{ backgroundImage: `url("/Warehouse.jpg")` }}>
        <div className="overlay">
          <div className="logoWrapper">
            <img src="/logo_witte_letters.png" alt="Lafeber logo" className="logoImage" />
          </div>
        </div>
        <div className="heroControls">
          <SearchBar searchTerm={searchTerm} onSearchChange={setSearchTerm} />
          <Filter selectedStatus={selectedStatus} onStatusChange={setSelectedStatus} />
        </div>
      </header>

      <main className="main">
        <div className="navGrid">
          <Card className="cardDefault card-updates">
            <div className="cardTitleDefault">Updates</div>
            <div className="linkSecondary">→ Bekijken</div>
            <div className="addIcon">＋</div>
          </Card>

          <Card
            className="cardDefault"
            onClick={() => setShowBestellingen(true)}
            style={{ cursor: "pointer" }}
          >
            <div className="cardTitleDefault">Bestellingen</div>
            <div className="linkSecondary">→ Bekijken</div>
          </Card>

          <Card
            className="cardDefault"
            onClick={() => setShowProducten(true)}
            style={{ cursor: "pointer" }}
          >
            <div className="cardTitleDefault">Producten</div>
            <div className="linkSecondary">→ Bekijken</div>
          </Card>

          <Card
            className="cardDefault"
            onClick={() => setShowZendingen(true)}
            style={{ cursor: "pointer" }}
          >
            <div className="cardTitleDefault">Zendingen</div>
            <div className="linkSecondary">→ Bekijken</div>
          </Card>
        </div>

        {!loading && filteredOrders.length > 0 && (
          <div className="recent-orders">
            <h2>Recent Orders</h2>
            <div className="orders-list">
              {filteredOrders.slice(0, 5).map((order) => (
                <div
                  key={order.id}
                  className="order-item clickable"
                  onClick={() => handleOrderClick(order)}
                >
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

      {/* Bestellingen Modal */}
      {showBestellingen && (
        <div className="modal-overlay">
          <div className="modal">
            <div className="modal-header">
              <h2>
                {selectedOrder
                  ? `Order #${selectedOrder.id} Details`
                  : `Bestellingen (${filteredOrders.length})`}
              </h2>
              <div className="modal-header-buttons">
                {selectedOrder && (
                  <button className="back-button" onClick={() => setSelectedOrder(null)}>
                    ←
                  </button>
                )}
                <button className="close-button" onClick={closeModal}>
                  &times;
                </button>
              </div>
            </div>
            <div className="modal-content">
              {selectedOrder ? (
                <div className="order-details">
                  <div className="detail-row">
                    <span className="detail-label">Status:</span>
                    <span className={`status ${selectedOrder.status.toLowerCase()}`}>
                      {selectedOrder.status}
                    </span>
                  </div>
                  <div className="detail-row">
                    <span className="detail-label">Bedrijfsnaam:</span>
                    <span>{selectedOrder.customer.bedrijfsNaam}</span>
                  </div>
                  <div className="detail-row">
                    <span className="detail-label">Contactpersoon:</span>
                    <span>{selectedOrder.customer.contactPersoon}</span>
                  </div>
                  <div className="detail-row">
                    <span className="detail-label">E-mail:</span>
                    <span>{selectedOrder.customer.email}</span>
                  </div>
                  <div className="detail-row">
                    <span className="detail-label">Telefoon:</span>
                    <span>{selectedOrder.customer.telefoonNummer}</span>
                  </div>
                  <div className="detail-row">
                    <span className="detail-label">Order Date:</span>
                    <span>{new Date(selectedOrder.orderDate).toLocaleDateString()}</span>
                  </div>
                  <div className="detail-row">
                    <span className="detail-label">Expected Delivery:</span>
                    <span>{new Date(selectedOrder.expectedDeliveryDate).toLocaleDateString()}</span>
                  </div>
                  {selectedOrder.actualDeliveryDate && (
                    <div className="detail-row">
                      <span className="detail-label">Actual Delivery:</span>
                      <span>{new Date(selectedOrder.actualDeliveryDate).toLocaleDateString()}</span>
                    </div>
                  )}
                  <div className="detail-row">
                    <span className="detail-label">Delivery Address:</span>
                    <span>{selectedOrder.deliveryAddress}</span>
                  </div>
                  <div className="detail-row">
                    <span className="detail-label">Total Weight:</span>
                    <span>{selectedOrder.totalWeight} kg</span>
                  </div>
                </div>
              ) : (
                <div className="all-orders">
                  {filteredOrders.map((order) => (
                    <div
                      key={order.id}
                      className="order-summary clickable"
                      onClick={() => setSelectedOrder(order)}
                    >
                      <span>Order #{order.id}</span>
                      <span className={`status ${order.status.toLowerCase()}`}>{order.status}</span>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>
        </div>
      )}

      {/* Producten Modal */}
      {showProducten && (
        <div className="modal-overlay">
          <div className="modal">
            <div className="modal-header">
              <h2>Producten</h2>
              <button className="close-button" onClick={closeModal}>
                &times;
              </button>
            </div>
            <div className="modal-content">
            </div>
          </div>
        </div>
      )}

      {/* Zendingen Modal */}
      {showZendingen && (
        <div className="modal-overlay">
          <div className="modal">
            <div className="modal-header">
              <h2>Zendingen</h2>
              <button className="close-button" onClick={closeModal}>
                &times;
              </button>
            </div>
            <div className="modal-content">
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default Dashboard;
