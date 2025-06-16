import React, { useState } from "react";

interface Props {
    onClose: () => void;
    onSuccess: () => void;
}

const ProductAanmakenModal: React.FC<Props> = ({ onClose, onSuccess }) => {
    const [form, setForm] = useState({
        productName: "",
        sku: "",
        weightKg: "",
        material: "",
        batchNumber: "",
        price: "",
        category: "",
        expirationDate: "",
    });

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setForm({ ...form, [e.target.name]: e.target.value });
    };

    const handleSubmit = async () => {
        try {
            const response = await fetch("http://localhost:5000/api/product", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(form),
            });

            if (response.ok) {
                onSuccess();
                onClose();
            } else {
                console.error("Fout bij opslaan product");
            }
        } catch (err) {
            console.error("Netwerkfout:", err);
        }
    };

    return (
        <div className="modal-overlay">
            <div className="modal modal-updates klant-form">
                <button onClick={onClose} className="close-button">&times;</button>
                <h2>Product Aanmaken</h2>

                <form className="form" onSubmit={(e) => { e.preventDefault(); handleSubmit(); }}>
                    {Object.entries(form).map(([key, value]) => (
                        <div className="form-group" key={key}>
                            <label htmlFor={key}>{key}</label>
                            <input
                                id={key}
                                type="text"
                                name={key}
                                value={value}
                                onChange={handleChange}
                                required={key !== "expirationDate"}
                            />
                        </div>
                    ))}

                    <button type="submit" className="submit-button">Aanmaken</button>
                </form>
            </div>
        </div>
    );
};

export default ProductAanmakenModal;