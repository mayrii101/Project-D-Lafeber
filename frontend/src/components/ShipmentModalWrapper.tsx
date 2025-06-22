import React, { useEffect, useState } from "react";
import ShipmentAanmakenModal from "./ShipmentAanmakenModal";

interface Props {
    orderIds: number[];
    expectedDeliveryDate: string;
    expectedDeliveryTime: string;
    onClose: () => void;
    onSuccess: () => void;
}

const ShipmentModalWrapper: React.FC<Props> = ({
    orderIds,
    expectedDeliveryDate,
    expectedDeliveryTime,
    onClose,
    onSuccess,
}) => {
    const [vehicles, setVehicles] = useState<{ id: number; licensePlate: string; capacityKg: number }[]>([]);
    const [drivers, setDrivers] = useState<{ id: number; name: string }[]>([]);
    const [orderWeight, setOrderWeight] = useState<number>(0);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const [vehicleRes, driverRes] = await Promise.all([
                    fetch("http://localhost:5000/api/vehicle"),
                    fetch("http://localhost:5000/api/employee"),
                ]);

                const [vehicleData, driverData] = await Promise.all([
                    vehicleRes.json(),
                    driverRes.json(),
                ]);

                setVehicles(vehicleData.filter((v: any) => !v.isDeleted));
                setDrivers(driverData.filter((e: any) => !e.isDeleted && e.role.toLowerCase() === "driver"));

                // Fetch total weight by calling each order API
                const weights = await Promise.all(
                    orderIds.map(async (id) => {
                        const res = await fetch(`http://localhost:5000/api/order/${id}`);
                        const data = await res.json();
                        return data.totalWeight ?? 0;
                    })
                );

                const total = weights.reduce((acc, w) => acc + w, 0);
                setOrderWeight(total);
            } catch (error) {
                console.error("Fout bij ophalen voertuigen, chauffeurs of gewicht:", error);
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, [orderIds]);

    if (loading) return <div>Laden...</div>;

    return (
        <ShipmentAanmakenModal
            orderIds={orderIds}
            expectedDeliveryDate={expectedDeliveryDate}
            expectedDeliveryTime={expectedDeliveryTime}
            onClose={onClose}
            onSuccess={onSuccess}
            vehicles={vehicles}
            drivers={drivers}
            orderWeight={orderWeight}
        />
    );
};

export default ShipmentModalWrapper;