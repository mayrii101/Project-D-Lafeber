import React, { useState, useEffect } from "react";

interface Props {
    onClose: () => void;
    onSuccess: () => void;
    orderIds: number[];
    expectedDeliveryDate: string;
    expectedDeliveryTime: string;
    vehicles: { id: number; licensePlate: string }[];
    drivers: { id: number; name: string }[];
}

const ShipmentAanmakenModal: React.FC<Props> = ({
    onClose,
    onSuccess,
    orderIds,
    expectedDeliveryDate,
    expectedDeliveryTime,
    vehicles,
    drivers,
}) => {
    const [form, setForm] = useState({
        vehicleId: vehicles[0]?.id || 0,
        driverId: drivers[0]?.id || 0,
        departureDate: "",
        departureTime: "",
    });

    const [formError, setFormError] = useState("");

    useEffect(() => {
        const now = new Date();

        const pad = (num: number) => num.toString().padStart(2, "0");

        const day = pad(now.getDate());
        const month = pad(now.getMonth() + 1);
        const year = now.getFullYear();

        const hours = pad(now.getHours());
        const minutes = pad(now.getMinutes());

        setForm((prev) => ({
            ...prev,
            departureDate: `${day}-${month}-${year}`,
            departureTime: `${hours}:${minutes}`,
        }));
    }, []);

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
                        <select
                            name="vehicleId"
                            value={form.vehicleId}
                            onChange={handleChange}
                            required
                        >
                            {vehicles.map((v) => (
                                <option key={v.id} value={v.id}>
                                    {v.licensePlate}
                                </option>
                            ))}
                        </select>
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

                    {/* departure date and time van gemaakte order */}
                    <input type="hidden" name="departureDate" value={form.departureDate} readOnly />
                    <input type="hidden" name="departureTime" value={form.departureTime} readOnly />

                    {formError && (
                        <div className="error-message" style={{ color: "red", marginTop: "5px" }}>
                            {formError}
                        </div>
                    )}

                    <button type="submit" className="submit-button">
                        Verzending Aanmaken
                    </button>
                </form>
            </div>
        </div>
    );
};

export default ShipmentAanmakenModal;