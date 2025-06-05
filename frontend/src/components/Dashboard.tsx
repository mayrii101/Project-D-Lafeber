import React, { useEffect, useState } from "react";
import { Card } from "./card";
import "../styles/Dashboard.css";
import SearchBar from "./Searchbar";
import Filter from "./Filter";
import Bestellingen from "./Bestellingen";
import Producten from "./Producten";
import Klanten from "./Klanten";
import OrderStatusChart from "./OrderStatusChart";
import OrderBarChart from "./OrderBarChart";

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

const getOrderStatusCounts = (orders: Order[]) => {
  const counts: Record<string, number> = {};
  orders.forEach(order => {
    const status = order.status;
    counts[status] = (counts[status] || 0) + 1;
  });
  return Object.entries(counts).map(([status, count]) => ({
    name: status,
    value: count
  }));
};

const Dashboard: React.FC = () => {
  const [orders, setOrders] = useState<Order[]>([]);
  const [filteredOrders, setFilteredOrders] = useState<Order[]>([]);
  const [products, setProducts] = useState<Product[]>([]);
  const [customers, setCustomers] = useState<Customer[]>([]);

  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedStatus, setSelectedStatus] = useState("");

  const [productSearchTerm, setProductSearchTerm] = useState("");
  const [customerSearchTerm, setCustomerSearchTerm] = useState("");

  const [showBestellingen, setShowBestellingen] = useState(false);
  const [selectedOrder, setSelectedOrder] = useState<Order | null>(null);

  const [showProducten, setShowProducten] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState<Product | null>(null);

  const [showKlanten, setShowKlanten] = useState(false);
  const [selectedCustomer, setSelectedCustomer] = useState<Customer | null>(null);

  useEffect(() => {
    const fetchAllData = async () => {
      try {
        const [ordersRes, productsRes, customersRes] = await Promise.all([
          fetch("http://localhost:5000/api/order"),
          fetch("http://localhost:5000/api/product"),
          fetch("http://localhost:5000/api/customer"),
        ]);

        const [ordersData, productsData, customersData] = await Promise.all([
          ordersRes.json(),
          productsRes.json(),
          customersRes.json(),
        ]);

        setOrders(ordersData);
        setFilteredOrders(ordersData);
        setProducts(productsData);
        setCustomers(customersData);
        setLoading(false);
      } catch (err) {
        console.error("Fout bij ophalen data:", err);
        setLoading(false);
      }
    };

    fetchAllData();
  }, []);

  useEffect(() => {
    const filtered = orders.filter((order) => {
      const matchesSearch =
        !searchTerm || order.id.toString().includes(searchTerm);
      const matchesStatus =
        !selectedStatus || order.status.toLowerCase() === selectedStatus.toLowerCase();
      return matchesSearch && matchesStatus;
    });

    setFilteredOrders(filtered);
  }, [searchTerm, orders, selectedStatus]);

  const closeModal = () => {
    setShowBestellingen(false);
    setShowProducten(false);
    setShowKlanten(false);
    setSelectedOrder(null);
    setSelectedProduct(null);
    setSelectedCustomer(null);
  };

  return (
    <div className="container">
      <header
        className="hero"
        style={{ backgroundImage: `url('/Warehouse.jpg')` }}
      >
        <div className="overlay">
          <div className="logoWrapper">
            <div className="logoText">Lafeber Insights</div>
          </div>
        </div>
      </header>

      <main className="main">
        <div className="navGrid">
          <Card className="cardDefault cardUpdates">
            <div className="cardTitleDefault">Updates</div>
            <div className="linkSecondary">→ Bekijken</div>
            <div className="addIcon">＋</div>
          </Card>

          <Card className="cardDefault" onClick={() => setShowBestellingen(true)} style={{ cursor: "pointer" }}>
            <div className="cardTitleDefault">Bestellingen ({filteredOrders.length})</div>
            <div className="linkSecondary">→ Bekijken</div>
          </Card>

          <Card className="cardDefault" onClick={() => setShowProducten(true)} style={{ cursor: "pointer" }}>
            <div className="cardTitleDefault">Producten ({products.length})</div>
            <div className="linkSecondary">→ Bekijken</div>
          </Card>

          <Card className="cardDefault" onClick={() => setShowKlanten(true)} style={{ cursor: "pointer" }}>
            <div className="cardTitleDefault">Klanten ({customers.length})</div>
            <div className="linkSecondary">→ Bekijken</div>
          </Card>
        </div>

        {/* Chart section */}
        <div className="chart-container" style={{ display: "flex", gap: "2rem" }}>
          <div style={{ flex: 1 }}>
            <OrderStatusChart data={getOrderStatusCounts(filteredOrders)} />
          </div>
          <div style={{ flex: 1, paddingTop: "7rem" }}>
            <OrderBarChart orders={filteredOrders} />
          </div>
        </div>
      </main>

      {showBestellingen && (
        <Bestellingen
          filteredOrders={filteredOrders}
          selectedOrder={selectedOrder}
          onSelectOrder={setSelectedOrder}
          onBack={() => setSelectedOrder(null)}
          onClose={closeModal}
          searchTerm={searchTerm}
          onSearchChange={setSearchTerm}
          selectedStatus={selectedStatus}
          onStatusChange={setSelectedStatus}
        />
      )}

      {showProducten && (
        <Producten
          products={products}
          selectedProduct={selectedProduct}
          onSelectProduct={setSelectedProduct}
          onBack={() => setSelectedProduct(null)}
          onClose={closeModal}
          searchTerm={productSearchTerm}
          onSearchChange={setProductSearchTerm}
        />
      )}

      {showKlanten && (
        <Klanten
          customers={customers}
          selectedCustomer={selectedCustomer}
          onSelectCustomer={setSelectedCustomer}
          onBack={() => setSelectedCustomer(null)}
          onClose={closeModal}
          searchTerm={customerSearchTerm}
          onSearchChange={setCustomerSearchTerm}
        />
      )}
    </div>
  );
};

export default Dashboard;
