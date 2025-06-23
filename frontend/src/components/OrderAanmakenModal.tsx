import React, { useState } from "react";
import "../styles/Error.css";

interface Props {
    onClose: () => void;
    onSuccess: () => void;
    klanten: { id: number; bedrijfsNaam: string }[];
    producten: { id: number; productName: string }[];
}

const OrderAanmakenModal: React.FC<Props> = ({ onClose, onSuccess, klanten, producten }) => {
    const [form, setForm] = useState({
        customerId: klanten[0]?.id.toString() || "",
        productId: producten[0]?.id.toString() || "",
        quantity: "1",
        deliveryAddress: "",
        expectedDeliveryDate: "",
        expectedDeliveryTime: "",
    });

    const [errors, setErrors] = useState({
        customerId: "",
        productId: "",
        quantity: "",
        deliveryAddress: "",
        expectedDeliveryDate: "",
    });

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        setForm({ ...form, [name]: value });
        setErrors({ ...errors, [name]: "" });
    };

    const validate = () => {
        const newErrors = { ...errors };
        let isValid = true;

        const qty = parseInt(form.quantity);
        if (!qty || isNaN(qty) || qty < 1) {
            newErrors.quantity = "Aantal moet een geheel getal van 1 of meer zijn.";
            isValid = false;
        }

        if (!form.deliveryAddress.trim()) {
            newErrors.deliveryAddress = "Verzendadres mag niet leeg zijn.";
            isValid = false;
        }

        const today = new Date(new Date().toDateString());
        const expected = new Date(form.expectedDeliveryDate);
        if (!form.expectedDeliveryDate || expected < today) {
            newErrors.expectedDeliveryDate = "Verwachte leverdatum mag niet in het verleden liggen.";
            isValid = false;
        }

        setErrors(newErrors);
        return isValid;
    };

    const formatDateToDDMMYYYY = (dateStr: string) => {
        const [year, month, day] = dateStr.split("-");
        return `${day}-${month}-${year}`;
    };

    const getCurrentDateTime = () => {
        const now = new Date();
        const day = String(now.getDate()).padStart(2, "0");
        const month = String(now.getMonth() + 1).padStart(2, "0");
        const year = now.getFullYear();
        const hours = String(now.getHours()).padStart(2, "0");
        const minutes = String(now.getMinutes()).padStart(2, "0");
        return {
            date: `${day}-${month}-${year}`,
            time: `${hours}:${minutes}`,
        };
    };

    const handleSubmit = async () => {
        if (!validate()) return;

        const now = getCurrentDateTime();

        const body = {
            customerId: parseInt(form.customerId),
            orderDate: now.date,
            orderTime: now.time,
            deliveryAddress: form.deliveryAddress,
            expectedDeliveryDate: formatDateToDDMMYYYY(form.expectedDeliveryDate),
            expectedDeliveryTime: form.expectedDeliveryTime,
            status: "Pending",
            productLines: [
                {
                    productId: parseInt(form.productId),
                    quantity: parseInt(form.quantity),
                },
            ],
        };

        try {
            const response = await fetch("http://localhost:5000/api/order", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(body),
            });

            if (response.ok) {
                onSuccess();
                onClose();
            } else {
                console.error("Fout bij opslaan bestelling.");
            }
        } catch (err) {
            console.error("Netwerkfout:", err);
        }
    };

    return (
        <div className="modal-overlay">
            <div className="modal modal-updates klant-form">
                <button onClick={onClose} className="close-button">&times;</button>
                <h2>Bestelling Aanmaken</h2>

                <form
                    className="form"
                    onSubmit={(e) => {
                        e.preventDefault();
                        handleSubmit();
                    }}
                >
                    <div className="form-group">
                        <label htmlFor="customerId">Klant</label>
                        <select
                            id="customerId"
                            name="customerId"
                            value={form.customerId}
                            onChange={handleChange}
                        >
                            {klanten.map((k) => (
                                <option key={k.id} value={k.id}>
                                    {k.bedrijfsNaam}
                                </option>
                            ))}
                        </select>
                        {errors.customerId && <p className="error">{errors.customerId}</p>}
                    </div>

                    <div className="form-group">
                        <label htmlFor="productId">Product</label>
                        <select
                            id="productId"
                            name="productId"
                            value={form.productId}
                            onChange={handleChange}
                        >
                            {producten.map((p) => (
                                <option key={p.id} value={p.id}>
                                    {p.productName}
                                </option>
                            ))}
                        </select>
                        {errors.productId && <p className="error">{errors.productId}</p>}
                    </div>

                    <div className="form-group">
                        <label htmlFor="quantity">Aantal</label>
                        <input
                            type="number"
                            id="quantity"
                            name="quantity"
                            value={form.quantity}
                            onChange={handleChange}
                        />
                        {errors.quantity && <p className="error">{errors.quantity}</p>}
                    </div>

                    <div className="form-group">
                        <label htmlFor="deliveryAddress">Verzendadres</label>
                        <input
                            type="text"
                            id="deliveryAddress"
                            name="deliveryAddress"
                            value={form.deliveryAddress}
                            onChange={handleChange}
                        />
                        {errors.deliveryAddress && <p className="error">{errors.deliveryAddress}</p>}
                    </div>

                    <div className="form-group">
                        <label htmlFor="expectedDeliveryDate">Verwachte Leverdatum</label>
                        <input
                            type="date"
                            id="expectedDeliveryDate"
                            name="expectedDeliveryDate"
                            value={form.expectedDeliveryDate}
                            onChange={handleChange}
                        />
                        {errors.expectedDeliveryDate && <p className="error">{errors.expectedDeliveryDate}</p>}
                    </div>

                    <div className="form-group">
                        <label htmlFor="expectedDeliveryTime">Verwachte Levertijd</label>
                        <input
                            type="time"
                            id="expectedDeliveryTime"
                            name="expectedDeliveryTime"
                            value={form.expectedDeliveryTime}
                            onChange={handleChange}
                        />
                    </div>

                    <button type="submit" className="submit-button">
                        Aanmaken
                    </button>
                </form>
            </div>
        </div>
    );
};

export default OrderAanmakenModal;
