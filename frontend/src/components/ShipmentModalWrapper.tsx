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
                const [vehicleRes, driverRes, orderRes] = await Promise.all([
                    fetch("http://localhost:5000/api/vehicle"),
                    fetch("http://localhost:5000/api/employee"),
                    fetch(`http://localhost:5000/api/order/totalWeight?orderIds=${orderIds.join(",")}`),
                ]);

                const [vehicleData, driverData, weightData] = await Promise.all([
                    vehicleRes.json(),
                    driverRes.json(),
                    orderRes.json(),
                ]);

                setVehicles(vehicleData.filter((v: any) => !v.isDeleted));
                setDrivers(driverData.filter((e: any) => !e.isDeleted && e.role.toLowerCase() === "driver"));
                setOrderWeight(weightData.totalWeight ?? 0);
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