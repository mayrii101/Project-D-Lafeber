import React from "react";
import XmlUpload from "./XMLupload";

interface XmlUploadModalProps {
  onClose: () => void;
}

const XmlUploadModal: React.FC<XmlUploadModalProps> = ({ onClose }) => {
  return (
    <div className="modal-overlay">
      <div className="modal">
        <div className="modal-header">
          <h2>XML Upload</h2>
          <div className="modal-header-buttons">
            <button onClick={onClose}>&times;</button>
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