import React, { useState } from "react";
import ShipmentModalWrapper from "./ShipmentModalWrapper";
import "../styles/Error.css";

interface Props {
    onClose: () => void;
    onSuccess: () => void;
    klanten: { id: number; bedrijfsNaam: string }[];
    producten: { id: number; productName: string }[];
}

const OrderAanmakenModal: React.FC<Props> = ({ onClose, onSuccess, klanten, producten }) => {
    const [createdOrder, setCreatedOrder] = useState<null | {
        id: number;
        expectedDeliveryDate: string;
        expectedDeliveryTime: string;
    }>(null);

    const [form, setForm] = useState({
        customerId: klanten[0]?.id || 0,
        productId: producten[0]?.id || 0,
        quantity: 1,
        deliveryAddress: "",
        expectedDeliveryDate: "",
        expectedDeliveryTime: "",
    });

    const [formError, setFormError] = useState("");
    const [errors, setErrors] = useState({
        quantity: "",
        deliveryAddress: "",
        expectedDeliveryDate: "",
    });

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        setForm((prev) => ({ ...prev, [name]: value }));
        setFormError("");
        setErrors((prev) => ({ ...prev, [name]: "" }));
    };

    const formatDateToDDMMYYYY = (dateStr: string) => {
        const [year, month, day] = dateStr.split("-");
        return `${day}-${month}-${year}`;
    };

    const CurrentDatetime = () => {
        const now = new Date();
        const pad = (num: number) => num.toString().padStart(2, "0");
        return {
            date: `${pad(now.getDate())}-${pad(now.getMonth() + 1)}-${now.getFullYear()}`,
            time: `${pad(now.getHours())}:${pad(now.getMinutes())}`,
        };
    };

    const validate = () => {
        let valid = true;
        const newErrors = { quantity: "", deliveryAddress: "", expectedDeliveryDate: "" };

        if (!form.deliveryAddress.trim()) {
            newErrors.deliveryAddress = "Verzendadres mag niet leeg zijn.";
            valid = false;
        }

        const quantity = Number(form.quantity);
        if (!Number.isInteger(quantity) || quantity <= 0) {
            newErrors.quantity = "Aantal moet een geheel getal groter dan 0 zijn.";
            valid = false;
        }

        const today = new Date().setHours(0, 0, 0, 0);
        const deliveryDate = new Date(form.expectedDeliveryDate).setHours(0, 0, 0, 0);
        if (!form.expectedDeliveryDate || deliveryDate < today) {
            newErrors.expectedDeliveryDate = "Verwachte leverdatum mag niet in het verleden liggen.";
            valid = false;
        }

        setErrors(newErrors);
        return valid;
    };

    const handleSubmit = async () => {
        if (!validate()) return;

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
                setCreatedOrder({
                    id: result.id,
                    expectedDeliveryDate: formatDateToDDMMYYYY(form.expectedDeliveryDate),
                    expectedDeliveryTime: form.expectedDeliveryTime,
                });
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

    if (createdOrder) {
        return (
            <ShipmentModalWrapper
                orderIds={[createdOrder.id]}
                expectedDeliveryDate={createdOrder.expectedDeliveryDate}
                expectedDeliveryTime={createdOrder.expectedDeliveryTime}
                onClose={() => setCreatedOrder(null)}
                onSuccess={() => {
                    setCreatedOrder(null);
                    onSuccess();
                    onClose();
                }}
            />
        );
    }

    return (
        <div className="modal-overlay">
            <div className="modal modal-updates klant-form">
                <button onClick={onClose} className="close-button">&times;</button>
                <h2>Bestelling Aanmaken</h2>

                <form className="form" onSubmit={(e) => {
                    e.preventDefault();
                    handleSubmit();
                }}>
                    <div className="form-group">
                        <label>Klant</label>
                        <select name="customerId" value={form.customerId} onChange={handleChange}>
                            {klanten.map((k) => (
                                <option key={k.id} value={k.id}>{k.bedrijfsNaam}</option>
                            ))}
                        </select>
                    </div>

                    <div className="form-group">
                        <label>Product</label>
                        <select name="productId" value={form.productId} onChange={handleChange}>
                            {producten.map((p) => (
                                <option key={p.id} value={p.id}>{p.productName}</option>
                            ))}
                        </select>
                    </div>

                    <div className="form-group">
                        <label>Aantal</label>
                        <input type="number" name="quantity" value={form.quantity} onChange={handleChange} />
                        {errors.quantity && <p className="error">{errors.quantity}</p>}
                    </div>

                    <div className="form-group">
                        <label>Verzendadres</label>
                        <input type="text" name="deliveryAddress" value={form.deliveryAddress} onChange={handleChange} />
                        {errors.deliveryAddress && <p className="error">{errors.deliveryAddress}</p>}
                    </div>

                    <div className="form-group">
                        <label>Verwachte Leverdatum</label>
                        <input type="date" name="expectedDeliveryDate" value={form.expectedDeliveryDate} onChange={handleChange} />
                        {errors.expectedDeliveryDate && <p className="error">{errors.expectedDeliveryDate}</p>}
                    </div>

                    <div className="form-group">
                        <label>Verwachte Levertijd</label>
                        <input type="time" name="expectedDeliveryTime" value={form.expectedDeliveryTime} onChange={handleChange} />
                    </div>

                    {formError && <div className="error-message">{formError}</div>}

                    <button type="submit" className="submit-button">Aanmaken</button>
                </form>
            </div>
        </div>
    );
};

export default OrderAanmakenModal;
