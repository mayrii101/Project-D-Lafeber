import React, { useState } from "react";
import "../styles/Error.css";

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

    const [errors, setErrors] = useState({
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
        setErrors({ ...errors, [name]: "" }); // Clear error on change
    };

    const validate = () => {
        const newErrors = { ...errors };
        let isValid = true;

        if (!form.productName.trim()) {
            newErrors.productName = "Productnaam mag niet leeg zijn.";
            isValid = false;
        }

        if (form.sku && !/^[a-zA-Z0-9\-]+$/.test(form.sku)) {
            newErrors.sku = "SKU mag alleen letters en cijfers bevatten.";
            isValid = false;
        }

        const weight = parseFloat(form.weightKg);
        if (!form.weightKg || isNaN(weight) || weight <= 0 || !Number.isInteger(weight)) {
            newErrors.weightKg = "Gewicht moet een positief geheel getal zijn.";
            isValid = false;
        }

        const batch = parseInt(form.batchNumber);
        if (!form.batchNumber || isNaN(batch) || batch <= 0) {
            newErrors.batchNumber = "Batchnummer moet een positief geheel getal zijn.";
            isValid = false;
        }

        const price = parseFloat(form.price);
        if (!form.price || isNaN(price) || price < 0) {
            newErrors.price = "Prijs moet 0 of hoger zijn.";
            isValid = false;
        }

        if (!form.category.trim()) {
            newErrors.category = "Categorie mag niet leeg zijn.";
            isValid = false;
        }

        setErrors(newErrors);
        return isValid;
    };

    const handleSubmit = async () => {
        if (!validate()) return;

        try {
            const body = {
                productName: form.productName,
                sku: form.sku,
                weightKg: parseFloat(form.weightKg),
                material: form.material,
                batchNumber: parseInt(form.batchNumber),
                price: parseFloat(form.price),
                category: form.category,
                isDeleted: false,
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
                            />
                            {errors[name as keyof typeof errors] && (
                                <p className="error">{errors[name as keyof typeof errors]}</p>
                            )}
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
