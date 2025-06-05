import React from "react";
import XmlUpload from "./XMLupload";

interface XmlUploadModalProps {
  onClose: () => void;
}

const XmlUploadModal: React.FC<XmlUploadModalProps> = ({ onClose }) => {
  return (
    <div className="modal-overlay">
      <div className="modal modal-xl">
        <div className="modal-header">
          <h2>Upload XML bestanden naar database</h2>
          <div className="modal-header-buttons">
            <button className="close-button-large" onClick={onClose} aria-label="Close modal">
              &times;
            </button>
          </div>
        </div>

        <div className="modal-content">
          <XmlUpload />
        </div>
      </div>
    </div>
  );
};

export default XmlUploadModal;