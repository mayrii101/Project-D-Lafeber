// ShipmentModalWrapper.tsx
import React, { useEffect, useState } from "react";
import ShipmentAanmakenModal from "./ShipmentAanmakenModal";

interface Props {
    orderId: number;
    expectedDeliveryDate: string;
    expectedDeliveryTime: string;
    onClose: () => void;
    onSuccess: () => void;
}

const ShipmentModalWrapper: React.FC<Props> = ({ orderId, expectedDeliveryDate, expectedDeliveryTime, onClose, onSuccess }) => {
    const [vehicles, setVehicles] = useState<{ id: number; licensePlate: string }[]>([]);
    const [drivers, setDrivers] = useState<{ id: number; name: string }[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const vehicleRes = await fetch("http://localhost:5000/api/vehicle");
                const driverRes = await fetch("http://localhost:5000/api/employee");

                const vehiclesData = await vehicleRes.json();
                const driversData = await driverRes.json();

                setVehicles(vehiclesData.filter((v: any) => !v.isDeleted));
                setDrivers(driversData.filter((e: any) => !e.isDeleted && e.role.toLowerCase() === "driver"));
                setLoading(false);
            } catch (error) {
                console.error("Fout bij ophalen voertuigen of chauffeurs:", error);
            }
        };

        fetchData();
    }, []);

    if (loading) return <div>Laden...</div>;

    return (
        <ShipmentAanmakenModal
            orderId={orderId}
            expectedDeliveryDate={expectedDeliveryDate}
            expectedDeliveryTime={expectedDeliveryTime}
            onClose={onClose}
            onSuccess={onSuccess}
            vehicles={vehicles}
            drivers={drivers}
        />
    );
};

export default ShipmentModalWrapper;