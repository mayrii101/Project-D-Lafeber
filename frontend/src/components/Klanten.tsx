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
  searchName: string;
  onSearchNameChange: (term: string) => void;
  searchCompany: string;
  onSearchCompanyChange: (term: string) => void;
  searchAddress: string;
  onSearchAddressChange: (term: string) => void;
}

const Klanten: React.FC<KlantenProps> = ({
  customers,
  selectedCustomer,
  onSelectCustomer,
  onBack,
  onClose,
  searchName,
  onSearchNameChange,
  searchCompany,
  onSearchCompanyChange,
  searchAddress,
  onSearchAddressChange,
}) => {
  const filtered = customers.filter((customer) => {
    return (
      customer.contactPersoon.toLowerCase().includes(searchName.toLowerCase()) &&
      customer.bedrijfsNaam.toLowerCase().includes(searchCompany.toLowerCase()) &&
      customer.adres.toLowerCase().includes(searchAddress.toLowerCase())
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
              <div className="klanten-search-wrapper">
                <div className="search-row">
                  <SearchBar
                    searchTerm={searchName}
                    onSearchChange={onSearchNameChange}
                    placeholder="Zoek op naam/contactpersoon..."
                  />
                  <SearchBar
                    searchTerm={searchCompany}
                    onSearchChange={onSearchCompanyChange}
                    placeholder="Zoek op bedrijfsnaam..."
                  />
                </div>

                <div className="search-row-bottom">
                  <SearchBar
                    searchTerm={searchAddress}
                    onSearchChange={onSearchAddressChange}
                    placeholder="Zoek op adres/stad..."
                  />
                  <button
                    className="reset-button"
                    onClick={() => {
                      onSearchNameChange("");
                      onSearchCompanyChange("");
                      onSearchAddressChange("");
                    }}
                  >
                    Reset
                  </button>
                </div>
              </div>
              <div style={{ borderBottom: "1px solid #eee", margin: "1rem 0" }}></div>

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
