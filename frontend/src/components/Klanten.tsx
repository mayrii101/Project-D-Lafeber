import React from "react";

interface Customer {
  id: number;
  bedrijfsNaam: string;
  contactPersoon: string;
  email: string;
  telefoonNummer: string;
  adres: string;
  isDeleted: boolean;
}

interface KlantenProps {
  customers: Customer[];
  selectedCustomer: Customer | null;
  onSelectCustomer: (customer: Customer) => void;
  onBack: () => void;
  onClose: () => void;
}

const Klanten: React.FC<KlantenProps> = ({
  customers,
  selectedCustomer,
  onSelectCustomer,
  onBack,
  onClose,
}) => {
  return (
    <div className="modal-overlay">
      <div className="modal">
        <div className="modal-header">
          <h2>
            {selectedCustomer
              ? `Klant #${selectedCustomer.id} Details`
              : `Klanten (${customers.length})`}
          </h2>
          <div className="modal-header-buttons">
            {selectedCustomer && (
              <button className="back-button" onClick={onBack}>
                ←
              </button>
            )}
            <button className="close-button" onClick={onClose}>
              &times;
            </button>
          </div>
        </div>
        <div className="modal-content">
          {selectedCustomer ? (
            <div className="order-details">
              <div className="detail-row">
                <span className="detail-label">Bedrijfsnaam:</span>
                <span>{selectedCustomer.bedrijfsNaam}</span>
              </div>
              <div className="detail-row">
                <span className="detail-label">Contactpersoon:</span>
                <span>{selectedCustomer.contactPersoon}</span>
              </div>
              <div className="detail-row">
                <span className="detail-label">Email:</span>
                <span>{selectedCustomer.email}</span>
              </div>
              <div className="detail-row">
                <span className="detail-label">Telefoonnummer:</span>
                <span>{selectedCustomer.telefoonNummer}</span>
              </div>
              <div className="detail-row">
                <span className="detail-label">Adres:</span>
                <span>{selectedCustomer.adres}</span>
              </div>
            </div>
          ) : (
            <div className="all-orders">
              {customers.map((customer) => (
                <div
                  key={customer.id}
                  className="order-summary clickable"
                  onClick={() => onSelectCustomer(customer)}
                >
                  <span>{customer.bedrijfsNaam}</span>
                  <span>{customer.contactPersoon}</span>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default Klanten;