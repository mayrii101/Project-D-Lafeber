import React from "react";
import SearchBar from "./Searchbar";
import Filter from "./Filter";

interface BestellingenProps {
  filteredOrders: Order[];
  selectedOrder: Order | null;
  onSelectOrder: (order: Order) => void;
  onBack: () => void;
  onClose: () => void;
  searchTerm: string;
  onSearchChange: (term: string) => void;
  selectedStatus: string;
  onStatusChange: (status: string) => void;
}

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

const Bestellingen: React.FC<BestellingenProps> = ({
  filteredOrders,
  selectedOrder,
  onSelectOrder,
  onBack,
  onClose,
  searchTerm,
  onSearchChange,
  selectedStatus,
  onStatusChange,
}) => {
  return (
    <div className="modal-overlay">
      <div className="modal">
        <div className="modal-header">
          <div style={{ display: "flex", flexDirection: "column", width: "100%" }}>
            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
              <h2>
                {selectedOrder
                  ? `Order #${selectedOrder.id} Details`
                  : `Bestellingen (${filteredOrders.length})`}
              </h2>
              <div className="modal-header-buttons">
                {selectedOrder && <button onClick={onBack}>←</button>}
                <button onClick={onClose}>&times;</button>
              </div>
            </div>

            {!selectedOrder && (
              <div style={{ display: "flex", gap: "1rem", marginTop: "1rem" }}>
                <SearchBar
                  searchTerm={searchTerm}
                  onSearchChange={onSearchChange}
                  placeholder="Zoek bestellingen..."
                />
                <Filter selectedStatus={selectedStatus} onStatusChange={onStatusChange} />
              </div>
            )}
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
              {filteredOrders.length === 0 ? (
                <div style={{ textAlign: "center", marginTop: "2rem" }}>
                  Geen bestellingen gevonden.
                </div>
              ) : (
                filteredOrders.map((order) => (
                  <div
                    key={order.id}
                    className="order-summary clickable"
                    onClick={() => onSelectOrder(order)}
                  >
                    <span>Order #{order.id}</span>
                    <span className={`status ${order.status.toLowerCase()}`}>{order.status}</span>
                  </div>
                ))
              )}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default Bestellingen;
