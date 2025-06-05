import React from "react";
import SearchBar from "./Searchbar";

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
  searchTerm: string;
  onSearchChange: (term: string) => void;
}

const Klanten: React.FC<KlantenProps> = ({
  customers,
  selectedCustomer,
  onSelectCustomer,
  onBack,
  onClose,
  searchTerm,
  onSearchChange,
}) => {
  const filtered = customers.filter((customer) => {
    const s = searchTerm.toLowerCase();
    return (
      customer.bedrijfsNaam.toLowerCase().includes(s) ||
      customer.contactPersoon.toLowerCase().includes(s) ||
      customer.email.toLowerCase().includes(s) ||
      customer.telefoonNummer.toLowerCase().includes(s) ||
      customer.adres.toLowerCase().includes(s)
    );
  });

  return (
    <div className="modal-overlay">
      <div className="modal">
        <div className="modal-header">
          <button onClick={onClose} className="close-button">&times;</button>
          <h2>
            {selectedCustomer
              ? `Klant: ${selectedCustomer.bedrijfsNaam}`
              : `Klanten (${filtered.length})`}
          </h2>
          <div className="modal-header-buttons">
            {selectedCustomer && <button onClick={onBack}>←</button>}
          </div>
        </div>

        <div className="modal-content">
          {!selectedCustomer ? (
            <>
              <SearchBar
                searchTerm={searchTerm}
                onSearchChange={onSearchChange}
                placeholder="Zoek klanten..."
              />
              <div style={{ borderBottom: "1px solid #eee", margin: "0.5rem 0 1rem" }}></div>

              <div className="all-customers">
                {filtered.map((customer) => (
                  <div
                    key={customer.id}
                    className="clickable order-summary"
                    onClick={() => onSelectCustomer(customer)}
                  >
                    <span>{customer.bedrijfsNaam}</span>
                  </div>
                ))}
              </div>
            </>
          ) : (
            <div className="customer-details">
              <p>Naam: {selectedCustomer.contactPersoon}</p>
              <p>Email: {selectedCustomer.email}</p>
              <p>Tel: {selectedCustomer.telefoonNummer}</p>
              <p>Adres: {selectedCustomer.adres}</p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default Klanten;
