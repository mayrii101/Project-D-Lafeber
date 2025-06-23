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
    });

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setForm({ ...form, [name]: value });
    };

    const handleSubmit = async () => {
        try {
            const body = {
                productName: form.productName,
                sku: form.sku,
                weightKg: parseFloat(form.weightKg),
                material: form.material,
                batchNumber: parseInt(form.batchNumber),
                price: parseFloat(form.price),
                category: form.category,
                isDeleted: false, // belangrijk voor je backend
            };

            const response = await fetch("http://localhost:5000/api/product", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(body),
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

                <form
                    className="form"
                    onSubmit={(e) => {
                        e.preventDefault();
                        handleSubmit();
                    }}
                >
                    {[
                        { name: "productName", label: "Productnaam" },
                        { name: "sku", label: "SKU" },
                        { name: "weightKg", label: "Gewicht (kg)" },
                        { name: "material", label: "Materiaal" },
                        { name: "batchNumber", label: "Batchnummer" },
                        { name: "price", label: "Prijs (€)" },
                        { name: "category", label: "Categorie" },
                    ].map(({ name, label }) => (
                        <div className="form-group" key={name}>
                            <label htmlFor={name}>{label}</label>
                            <input
                                id={name}
                                type={["weightKg", "price", "batchNumber"].includes(name) ? "number" : "text"}
                                name={name}
                                value={(form as any)[name]}
                                onChange={handleChange}
                                required
                            />
                        </div>
                    ))}

                    <button type="submit" className="submit-button">
                        Aanmaken
                    </button>
                </form>
            </div>
        </div>
    );
};

export default ProductAanmakenModal;
