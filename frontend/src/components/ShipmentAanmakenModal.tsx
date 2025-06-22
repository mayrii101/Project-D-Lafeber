import React, { useState, useEffect } from "react";

interface Props {
    onClose: () => void;
    onSuccess: () => void;
    orderIds: number[];
    expectedDeliveryDate: string;
    expectedDeliveryTime: string;
    vehicles: { id: number; licensePlate: string; capacityKg: number }[];
    drivers: { id: number; name: string }[];
    orderWeight: number;
}

const ShipmentAanmakenModal: React.FC<Props> = ({
    onClose,
    onSuccess,
    orderIds,
    expectedDeliveryDate,
    expectedDeliveryTime,
    vehicles,
    drivers,
    orderWeight,
}) => {
    const [form, setForm] = useState({
        vehicleId: 0,
        driverId: drivers[0]?.id || 0,
        departureDate: "",
        departureTime: "",
    });

    const [formError, setFormError] = useState("");

    const filteredVehicles = vehicles.filter((v) => v.capacityKg >= orderWeight);

    useEffect(() => {
        const now = new Date();
        const pad = (num: number) => num.toString().padStart(2, "0");

        setForm((prev) => ({
            ...prev,
            departureDate: `${pad(now.getDate())}-${pad(now.getMonth() + 1)}-${now.getFullYear()}`,
            departureTime: `${pad(now.getHours())}:${pad(now.getMinutes())}`,
        }));
    }, []);

    useEffect(() => {
        if (filteredVehicles.length > 0) {
            setForm((prev) => ({ ...prev, vehicleId: filteredVehicles[0].id }));
        } else {
            setForm((prev) => ({ ...prev, vehicleId: 0 }));
        }
    }, [filteredVehicles]);

    const handleChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const { name, value } = e.target;
        setForm((prev) => ({ ...prev, [name]: value }));
        if (formError) setFormError("");
    };

    const handleSubmit = async () => {
        const body = {
            orderIds,
            vehicleId: Number(form.vehicleId),
            driverId: Number(form.driverId),
            expectedDeliveryDate,
            expectedDeliveryTime,
            departureDate: form.departureDate,
            departureTime: form.departureTime,
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

                <form
                    className="form"
                    onSubmit={(e) => {
                        e.preventDefault();
                        handleSubmit();
                    }}
                >
                    <div className="form-group">
                        <label>Voertuig</label>
                        {filteredVehicles.length > 0 ? (
                            <select
                                name="vehicleId"
                                value={form.vehicleId}
                                onChange={handleChange}
                                required
                            >
                                {filteredVehicles.map((v) => (
                                    <option key={v.id} value={v.id}>
                                        {v.licensePlate}
                                    </option>
                                ))}
                            </select>
                        ) : (
                            <div style={{ color: "red", marginTop: "5px" }}>
                                Geen voertuigen beschikbaar met voldoende capaciteit voor {orderWeight} kg.
                            </div>
                        )}
                    </div>

                    <div className="form-group">
                        <label>Chauffeur</label>
                        <select
                            name="driverId"
                            value={form.driverId}
                            onChange={handleChange}
                            required
                        >
                            {drivers.map((d) => (
                                <option key={d.id} value={d.id}>
                                    {d.name}
                                </option>
                            ))}
                        </select>
                    </div>

                    {/* depraturedate ne departuretime van aangemaakte order */}
                    <input type="hidden" name="departureDate" value={form.departureDate} readOnly />
                    <input type="hidden" name="departureTime" value={form.departureTime} readOnly />

                    {formError && (
                        <div className="error-message" style={{ color: "red", marginTop: "5px" }}>
                            {formError}
                        </div>
                    )}

                    <button type="submit" className="submit-button" disabled={filteredVehicles.length === 0}>
                        Verzending Aanmaken
                    </button>
                </form>
            </div>
        </div>
    );
};

export default ShipmentAanmakenModal;