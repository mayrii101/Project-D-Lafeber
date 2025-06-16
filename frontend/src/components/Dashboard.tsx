import React, { useEffect, useState } from "react";
import { Card } from "./card";
import "../styles/Dashboard.css";
import Bestellingen from "./Bestellingen";
import Producten from "./Producten";
import Klanten from "./Klanten";
import XmlUploadModal from "./XmlUploadModal";
import KlantAanmakenModal from "./KlantAanmakenModal";

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
  const [showXmlUpload, setShowXmlUpload] = useState(false);

  const [customerSearchName, setCustomerSearchName] = useState("");
  const [customerSearchCompany, setCustomerSearchCompany] = useState("");
  const [customerSearchAddress, setCustomerSearchAddress] = useState("");

  const [showKlantForm, setShowKlantForm] = useState(false);

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
    setShowXmlUpload(false);
  };

  return (
    <div className="container">
      <header
        className="hero"
        style={{ backgroundImage: `url('/warehouseee.jpg')` }}
      >
        <div className="overlay">
          <div className="logoWrapper">
            <div className="logoText">Lafeber Insights</div>          </div>
        </div>
      </header>

      <main className="main">
        <div className="navGrid">

          <Card className="cardDefault" onClick={() => setShowBestellingen(true)} style={{ cursor: "pointer" }}>
            <div className="cardTitleDefault">Bestellingen ({filteredOrders.length})</div>
            <div className="linkSecondary">→ Bekijken</div>
          </Card>

          <Card className="cardDefault" onClick={() => setShowProducten(true)} style={{ cursor: "pointer" }}>
            <div className="cardTitleDefault">Producten ({products.length})</div>
            <div className="linkSecondary">→ Bekijken</div>
          </Card>

          <Card
            className="cardDefault"
            onClick={() => setShowKlanten(true)}
            style={{ cursor: "pointer", position: "relative" }}
          >
            <div className="cardTitleDefault">Klanten ({customers.length})</div>
            <div className="linkSecondary">→ Bekijken</div>
            <div
              className="addIcon"
              onClick={(e) => {
                e.stopPropagation();
                setShowKlantForm(true);
              }}
            >
              ＋
            </div>
          </Card>

          <Card
            className="cardDefault cardUpdates"
            onClick={() => setShowXmlUpload(true)}
            style={{ cursor: "pointer" }}
          >
            <div className="cardTitleDefault">XML uploaden</div>
            <div className="text-sm text-muted-foreground italic">
              Alleen voor technisch personeel
            </div>
            <div className="linkSecondary">→ Bekijken</div>
            <div className="addIcon">＋</div>
          </Card>
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
            // herlaad klanten
            fetch("http://localhost:5000/api/customer")
              .then((res) => res.json())
              .then((data) => setCustomers(data));
          }}
        />
      )}


      {showXmlUpload && (
        <XmlUploadModal onClose={closeModal} />
      )}
    </div>
  );
};

export default Dashboard;
