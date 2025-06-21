import React, { useEffect, useState } from "react";
import { Card } from "./card";
import "../styles/Dashboard.css";
import Bestellingen from "./Bestellingen";
import Producten from "./Producten";
import Klanten from "./Klanten";
import XmlUploadModal from "./XmlUploadModal";
import KlantAanmakenModal from "./KlantAanmakenModal";
import ProductAanmakenModal from "./ProductAanmakenModal";
import OrderAanmakenModal from "./OrderAanmakenModal";
import Notitie from "./Notitie";
import OrderStatusChart from "./OrderStatusChart";

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

  const [customerSearchName, setCustomerSearchName] = useState("");
  const [customerSearchCompany, setCustomerSearchCompany] = useState("");
  const [customerSearchAddress, setCustomerSearchAddress] = useState("");

  const [showBestellingen, setShowBestellingen] = useState(false);
  const [selectedOrder, setSelectedOrder] = useState<Order | null>(null);

  const [showProducten, setShowProducten] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState<Product | null>(null);

  const [showKlanten, setShowKlanten] = useState(false);
  const [selectedCustomer, setSelectedCustomer] = useState<Customer | null>(null);

  const [showXmlUpload, setShowXmlUpload] = useState(false);
  const [showKlantForm, setShowKlantForm] = useState(false);
  const [showProductForm, setShowProductForm] = useState(false);
  const [showOrderForm, setShowOrderForm] = useState(false);

  const closeModel = () => {
    setShowBestellingen(false);
    setShowProducten(false);
    setShowKlanten(false);
    setShowXmlUpload(false);
    setSelectedOrder(null);
    setSelectedProduct(null);
    setSelectedCustomer(null);
  };

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
      const matchesSearch = !searchTerm || order.id.toString().includes(searchTerm);
      const matchesStatus = !selectedStatus || order.status.toLowerCase() === selectedStatus.toLowerCase();
      return matchesSearch && matchesStatus;
    });

    setFilteredOrders(filtered);
  }, [searchTerm, orders, selectedStatus]);

  return (
    <div className="container">
      <header className="hero" style={{ backgroundImage: `url('/warehouseee.jpg')` }}>
        <div className="overlay">
          <div className="logoWrapper">
            <div className="logoText">Lafeber Insights</div>
          </div>
        </div>
      </header>

      <main className="main">
        <div className="navGrid">
          <Card className="cardDefault" onClick={() => setShowBestellingen(true)}>
            <div className="cardTitleDefault">Bestellingen ({filteredOrders.length})</div>
            <div className="linkSecondary">→ Bekijken</div>
            <div className="addIcon" onClick={(e) => { e.stopPropagation(); setShowOrderForm(true); }}>＋</div>
          </Card>

          <Card className="cardDefault" onClick={() => setShowProducten(true)}>
            <div className="cardTitleDefault">Producten ({products.length})</div>
            <div className="linkSecondary">→ Bekijken</div>
            <div className="addIcon" onClick={(e) => { e.stopPropagation(); setShowProductForm(true); }}>＋</div>
          </Card>

          <Card className="cardDefault" onClick={() => setShowKlanten(true)}>
            <div className="cardTitleDefault">Klanten ({customers.length})</div>
            <div className="linkSecondary">→ Bekijken</div>
            <div className="addIcon" onClick={(e) => { e.stopPropagation(); setShowKlantForm(true); }}>＋</div>
          </Card>

          <Card className="cardDefault cardUpdates" onClick={() => setShowXmlUpload(true)}>
            <div className="cardTitleDefault">XML uploaden</div>
            <div className="text-sm text-muted-foreground italic">Alleen voor technisch personeel</div>
            <div className="linkSecondary">→ Bekijken</div>
            <div className="addIcon updates">＋</div>
          </Card>
        </div>
        {/* Chart section */}
        <div className="chart-container">
          <div>
            <OrderStatusChart data={getOrderStatusCounts(filteredOrders)} />
          </div>
        </div>

      </main>

      {/* Sticky Note Component */}
      <Notitie />

      {/* Modals & Views */}
      {showBestellingen && (
        <Bestellingen
          filteredOrders={filteredOrders}
          selectedOrder={selectedOrder}
          onSelectOrder={setSelectedOrder}
          onBack={() => setSelectedOrder(null)}
          onClose={closeModel}
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
          onClose={closeModel}
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
          onClose={closeModel}
          searchName={customerSearchName}
          onSearchNameChange={setCustomerSearchName}
          searchCompany={customerSearchCompany}
          onSearchCompanyChange={setCustomerSearchCompany}
          searchAddress={customerSearchAddress}
          onSearchAddressChange={setCustomerSearchAddress}
        />
      )}

      {showKlantForm && (
        <KlantAanmakenModal
          onClose={() => setShowKlantForm(false)}
          onSuccess={() => {
            fetch("http://localhost:5000/api/customer")
              .then((res) => res.json())
              .then((data) => setCustomers(data));
          }}
        />
      )}

      {showProductForm && (
        <ProductAanmakenModal
          onClose={() => setShowProductForm(false)}
          onSuccess={() => {
            fetch("http://localhost:5000/api/product")
              .then((res) => res.json())
              .then((data) => setProducts(data));
          }}
        />
      )}

      {showOrderForm && (
        <OrderAanmakenModal
          onClose={() => setShowOrderForm(false)}
          onSuccess={() => {
            fetch("http://localhost:5000/api/order")
              .then((res) => res.json())
              .then((data) => {
                setOrders(data);
                setFilteredOrders(data);
              });
          }}
          klanten={customers}
          producten={products}
        />
      )}

      {showXmlUpload && <XmlUploadModal onClose={closeModel} />}
    </div>
  );
};

export default Dashboard;
