// ShipmentAanmakenModal.tsx
import React, { useState } from "react";

interface Props {
    onClose: () => void;
    onSuccess: () => void;
    orderId: number;
    expectedDeliveryDate: string;
    expectedDeliveryTime: string;
    vehicles: { id: number; licensePlate: string }[];
    drivers: { id: number; name: string }[];
}

const ShipmentAanmakenModal: React.FC<Props> = ({ onClose, onSuccess, orderId, expectedDeliveryDate, expectedDeliveryTime, vehicles, drivers }) => {
    const [form, setForm] = useState({
        vehicleId: vehicles[0]?.id || 0,
        driverId: drivers[0]?.id || 0,
    });

    const [formError, setFormError] = useState("");

    const handleChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const { name, value } = e.target;
        setForm((prev) => ({ ...prev, [name]: value }));
        if (formError) setFormError("");
    };

    const handleSubmit = async () => {
        const body = {
            orderId,
            vehicleId: Number(form.vehicleId),
            driverId: Number(form.driverId),
            expectedDeliveryDate,
            expectedDeliveryTime,
        };

        try {
            const response = await fetch("http://localhost:5000/api/shipment", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(body),
            });

            if (response.ok) {
                onSuccess();
                onClose();
            } else if (response.status === 422) {
                const errorText = await response.text();
                setFormError(errorText);
            } else {
                setFormError("Er is een fout opgetreden bij het aanmaken van de verzending.");
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
                <h2>Verzending Aanmaken</h2>

                <form className="form" onSubmit={(e) => { e.preventDefault(); handleSubmit(); }}>
                    <div className="form-group">
                        <label>Voertuig</label>
                        <select name="vehicleId" value={form.vehicleId} onChange={handleChange} required>
                            {vehicles.map(v => (
                                <option key={v.id} value={v.id}>{v.licensePlate}</option>
                            ))}
                        </select>
                    </div>

                    <div className="form-group">
                        <label>Chauffeur</label>
                        <select name="driverId" value={form.driverId} onChange={handleChange} required>
                            {drivers.map(d => (
                                <option key={d.id} value={d.id}>{d.name}</option>
                            ))}
                        </select>
                    </div>

                    {formError && (
                        <div className="error-message" style={{ color: "red", marginTop: "5px" }}>
                            {formError}
                        </div>
                    )}

                    <button type="submit" className="submit-button">Verzending Aanmaken</button>
                </form>
            </div>
        </div>
    );
};

export default ShipmentAanmakenModal;