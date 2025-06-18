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

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        setForm((prev) => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async () => {
        const body = {
            customerId: Number(form.customerId),
            orderDate: form.orderDate,
            orderTime: form.orderTime,
            deliveryAddress: form.deliveryAddress,
            expectedDeliveryDate: form.expectedDeliveryDate,
            expectedDeliveryTime: form.expectedDeliveryTime,
            status: "Pending", // altijd automatisch Pending
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
                onSuccess();
                onClose();
            } else {
                console.error("Fout bij opslaan order");
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
                        <input type="number" name="quantity" value={form.quantity} onChange={handleChange} required />
                    </div>

                    <div className="form-group">
                        <label>Orderdatum</label>
                        <input type="date" name="orderDate" value={form.orderDate} onChange={handleChange} required />
                    </div>

                    <div className="form-group">
                        <label>Ordertijd</label>
                        <input type="time" name="orderTime" value={form.orderTime} onChange={handleChange} required />
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
