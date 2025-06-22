import React, { useState } from "react";

interface Props {
    onClose: () => void;
    onSuccess: () => void;
    klanten: { id: number; bedrijfsNaam: string }[];
    producten: { id: number; productName: string }[];
}

const OrderAanmakenModal: React.FC<Props> = ({ onClose, onSuccess, klanten, producten }) => {
    const [form, setForm] = useState({
        customerId: klanten[0]?.id || 0,
        productId: producten[0]?.id || 0,
        quantity: 1,
        deliveryAddress: "",
        orderDate: "",
        orderTime: "",
        expectedDeliveryDate: "",
        expectedDeliveryTime: "",
    });

    // New state for form error message
    const [formError, setFormError] = useState("");

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        setForm((prev) => ({ ...prev, [name]: value }));
        if (formError) setFormError(""); // Clear error when user changes input
    };

    const formatDateToDDMMYYYY = (dateStr: string) => {
        const [year, month, day] = dateStr.split("-");
        return `${day}-${month}-${year}`;
    };

    const CurrentDatetime = () => {
        const now = new Date();
        const day = String(now.getDate()).padStart(2, "0");
        const month = String(now.getMonth() + 1).padStart(2, "0");
        const year = now.getFullYear();
        const hours = String(now.getHours()).padStart(2, "0");
        const minutes = String(now.getMinutes()).padStart(2, "0");

        return {
            date: `${day}-${month}-${year}`,
            time: `${hours}:${minutes}`
        };
    };

    const handleSubmit = async () => {
        setFormError(""); // reset previous error

        const now = CurrentDatetime();

        const body = {
            customerId: Number(form.customerId),
            orderDate: now.date,
            orderTime: now.time,
            deliveryAddress: form.deliveryAddress,
            expectedDeliveryDate: formatDateToDDMMYYYY(form.expectedDeliveryDate),
            expectedDeliveryTime: form.expectedDeliveryTime,
            status: "Pending",
            productLines: [
                {
                    productId: Number(form.productId),
                    quantity: Number(form.quantity),
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
                const result = await response.json();
                onSuccess();
                onClose();
            } else if (response.status === 422) {
                const errorText = await response.text();
                setFormError(errorText);
            } else {
                setFormError("Er is een fout opgetreden bij het aanmaken van de bestelling.");
            }
        } catch (err) {
            console.error("Netwerkfout:", err);
            setFormError("Kan geen verbinding maken met de server.");
        }
    };

    return (
        <div className="modal-overlay">
            <div className="modal modal-updates klant-form">
                <button onClick={onClose} className="close-button">&times;</button>
                <h2>Bestelling Aanmaken</h2>

                <form className="form" onSubmit={(e) => { e.preventDefault(); handleSubmit(); }}>
                    <div className="form-group">
                        <label>Klant</label>
                        <select name="customerId" value={form.customerId} onChange={handleChange}>
                            {klanten.map(k => (
                                <option key={k.id} value={k.id}>{k.bedrijfsNaam}</option>
                            ))}
                        </select>
                    </div>

                    <div className="form-group">
                        <label>Product</label>
                        <select name="productId" value={form.productId} onChange={handleChange}>
                            {producten.map(p => (
                                <option key={p.id} value={p.id}>{p.productName}</option>
                            ))}
                        </select>
                    </div>

                    <div className="form-group">
                        <label>Aantal</label>
                        <input
                            type="number"
                            name="quantity"
                            value={form.quantity}
                            onChange={handleChange}
                            required
                        />
                        {formError && (
                            <div className="error-message" style={{ color: "red", marginTop: "5px" }}>
                                {formError}
                            </div>
                        )}
                    </div>

                    <div className="form-group">
                        <label>Verzendadres</label>
                        <input type="text" name="deliveryAddress" value={form.deliveryAddress} onChange={handleChange} required />
                    </div>

                    <div className="form-group">
                        <label>Verwachte Leverdatum</label>
                        <input type="date" name="expectedDeliveryDate" value={form.expectedDeliveryDate} onChange={handleChange} required />
                    </div>

                    <div className="form-group">
                        <label>Verwachte Levertijd</label>
                        <input type="time" name="expectedDeliveryTime" value={form.expectedDeliveryTime} onChange={handleChange} required />
                    </div>

                    <button type="submit" className="submit-button">Aanmaken</button>
                </form>
            </div>
        </div>
    );
};

export default OrderAanmakenModal;