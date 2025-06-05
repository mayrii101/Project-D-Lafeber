import React from "react";
import SearchBar from "./Searchbar";

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

interface ProductenProps {
  products: Product[];
  selectedProduct: Product | null;
  onSelectProduct: (product: Product) => void;
  onBack: () => void;
  onClose: () => void;
  searchTerm: string;
  onSearchChange: (term: string) => void;
}

const Producten: React.FC<ProductenProps> = ({
  products,
  selectedProduct,
  onSelectProduct,
  onBack,
  onClose,
  searchTerm,
  onSearchChange,
}) => {
  const filtered = products.filter((product) =>
    product.productName.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="modal-overlay">
      <div className="modal">
        <button onClick={onClose} className="close-button">&times;</button>
        <div className="modal-header">
          <h2>
            {selectedProduct
              ? `Product: ${selectedProduct.productName}`
              : `Producten (${filtered.length})`}
          </h2>
          <div className="modal-header-buttons">

          </div>
        </div>

        <div className="modal-content">
          {!selectedProduct ? (
            <>
              <SearchBar
                searchTerm={searchTerm}
                onSearchChange={onSearchChange}
                placeholder="Zoek producten..."
              />
              <div style={{ borderBottom: "1px solid #eee", margin: "0.5rem 0 1rem" }}></div>

              <div className="all-products">
                {filtered.map((product) => (
                  <div
                    key={product.id}
                    className="clickable order-summary"
                    onClick={() => onSelectProduct(product)}
                  >
                    <span>{product.productName}</span>
                  </div>
                ))}
              </div>
            </>
          ) : (
            <div className="product-details">
              <p>Naam: {selectedProduct.productName}</p>
              <p>Prijs: €{selectedProduct.price}</p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default Producten;