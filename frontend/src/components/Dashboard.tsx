import React, { useEffect, useState } from "react";
import { Card } from "./card";
import "../styles/Dashboard.css";
import SearchBar from "./Searchbar";
import Filter from "./Filter";
import Bestellingen from "./Bestellingen";
import Producten from "./Producten";
import Klanten from "./Klanten";

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

interface Product {
  id: number;
  productName: string;
  sku: string;
  weightKg: number;
  material: string;
  batchNumber: number;
  price: number;
  category: string;
  expirationDate?: string;
  isDeleted: boolean;
}

interface Customer {
  id: number;
  bedrijfsNaam: string;
  contactPersoon: string;
  email: string;
  telefoonNummer: string;
  adres: string;
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
  const [selectedProduct, setSelectedProduct] = useState<Product | null>(null);

  const [showklanten, setShowklanten] = useState(false);
  const [selectedCustomer, setSelectedCustomer] = useState<Customer | null>(null);

  const [products, setProducts] = useState<Product[]>([]);
  const [customers, setCustomers] = useState<Customer[]>([]);


  useEffect(() => {
    // Orders
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

    // Producten
    fetch("http://localhost:5000/api/products")
      .then((res) => res.json())
      .then((data) => setProducts(data))
      .catch((err) => console.error("Failed to fetch products:", err));

    // klanten
    fetch("http://localhost:5000/api/customer")
      .then((res) => res.json())
      .then((data) => setCustomers(data))
      .catch((err) => console.error("Failed to fetch customers:", err));
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

  const handleOrderClick = (order: Order) => {
    setSelectedOrder(order);
    setShowBestellingen(true);
  };

  const closeModal = () => {
    setShowBestellingen(false);
    setShowProducten(false);
    setShowklanten(false);
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
            <div className="cardTitleDefault">Bestellingen ({filteredOrders.length})</div>
            <div className="linkSecondary">→ Bekijken</div>
          </Card>
          <Card
            className="cardDefault"
            onClick={() => setShowProducten(true)}
            style={{ cursor: "pointer" }}
          >
            <div className="cardTitleDefault">Producten ({products.length})</div>
            <div className="linkSecondary">→ Bekijken</div>
          </Card>

          <Card
            className="cardDefault"
            onClick={() => setShowklanten(true)}
            style={{ cursor: "pointer" }}
          >
            <div className="cardTitleDefault">Klanten ({customers.length})</div>
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
                  <span className={`status ${order.status.toLowerCase()}`}>{order.status}</span>
                </div>
              ))}
            </div>
          </div>
        )}
      </main>

      {showBestellingen && (
        <Bestellingen
          filteredOrders={filteredOrders}
          selectedOrder={selectedOrder}
          onSelectOrder={setSelectedOrder}
          onBack={() => setSelectedOrder(null)}
          onClose={closeModal}
        />
      )}

      {showklanten && (
        <Klanten
          customers={customers}
          selectedCustomer={selectedCustomer}
          onSelectCustomer={setSelectedCustomer}
          onBack={() => setSelectedCustomer(null)}
          onClose={closeModal}
        />
      )}


      {showProducten && (
        <Producten
          products={products}
          selectedProduct={selectedProduct}
          onSelectProduct={setSelectedProduct}
          onBack={() => setSelectedProduct(null)}
          onClose={closeModal}
        />
      )}

    </div>
  );
};

export default Dashboard;
