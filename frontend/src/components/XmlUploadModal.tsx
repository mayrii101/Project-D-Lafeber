import React from "react";
import XmlUpload from "./XMLupload";

interface XmlUploadModalProps {
  onClose: () => void;
}

const XmlUploadModal: React.FC<XmlUploadModalProps> = ({ onClose }) => {
  return (
    <div className="modal-overlay">
      <div className="modal modal-xml">
        <button onClick={onClose} className="close-button" aria-label="Close modal">
          &times;
        </button>
        <div className="modal-header">
          <h2>Upload XML bestanden naar database</h2>
          <div className="modal-header-buttons"></div>
        </div>
        <div className="modal-content">
          <XmlUpload />
        </div>
      </div>
    </div>
  );
};

export default XmlUploadModal;