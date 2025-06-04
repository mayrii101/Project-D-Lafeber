import React from "react";
import XmlUpload from "./XMLupload";

interface XmlUploadModalProps {
  onClose: () => void;
}

const XmlUploadModal: React.FC<XmlUploadModalProps> = ({ onClose }) => {
  return (
    <div className="modal">
      <button
        onClick={onClose}
        style={{ float: "right", margin: "10px", fontSize: "18px" }}
      >
        ✖ Close
      </button>
      <XmlUpload />
    </div>
  );
};

export default XmlUploadModal;