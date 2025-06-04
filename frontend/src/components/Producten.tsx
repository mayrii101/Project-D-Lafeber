import React from "react";

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
}

const Producten: React.FC<ProductenProps> = ({
  products,
  selectedProduct,
  onSelectProduct,
  onBack,
  onClose,
}) => {
  return (
    <div className="modal-overlay">
      <div className="modal">
        <div className="modal-header">
          <h2>
            {selectedProduct
              ? `Product #${selectedProduct.id} Details`
              : `Producten (${products.length})`}
          </h2>
          <div className="modal-header-buttons">
            {selectedProduct && (
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
          {selectedProduct ? (
            <div className="order-details">
              <div className="detail-row">
                <span className="detail-label">Naam:</span>
                <span>{selectedProduct.productName}</span>
              </div>
              <div className="detail-row">
                <span className="detail-label">SKU:</span>
                <span>{selectedProduct.sku}</span>
              </div>
              <div className="detail-row">
                <span className="detail-label">Gewicht:</span>
                <span>{selectedProduct.weightKg} kg</span>
              </div>
              <div className="detail-row">
                <span className="detail-label">Materiaal:</span>
                <span>{selectedProduct.material}</span>
              </div>
              <div className="detail-row">
                <span className="detail-label">Batch:</span>
                <span>{selectedProduct.batchNumber}</span>
              </div>
              <div className="detail-row">
                <span className="detail-label">Prijs:</span>
                <span>€{selectedProduct.price.toFixed(2)}</span>
              </div>
              <div className="detail-row">
                <span className="detail-label">Categorie:</span>
                <span>{selectedProduct.category}</span>
              </div>
              {selectedProduct.expirationDate && (
                <div className="detail-row">
                  <span className="detail-label">Houdbaar tot:</span>
                  <span>{new Date(selectedProduct.expirationDate).toLocaleDateString()}</span>
                </div>
              )}
            </div>
          ) : (
            <div className="all-orders">
              {products.map((product) => (
                <div
                  key={product.id}
                  className="order-summary clickable"
                  onClick={() => onSelectProduct(product)}
                >
                  <span>{product.productName}</span>
                  <span>€{product.price.toFixed(2)}</span>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default Producten;