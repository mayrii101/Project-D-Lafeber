import React, { useState, useMemo } from "react";
import {
    Chart as ChartJS,
    CategoryScale,
    LinearScale,
    BarElement,
    Title,
    Tooltip,
    Legend,
} from "chart.js";
import { Bar } from "react-chartjs-2";

ChartJS.register(
    CategoryScale,
    LinearScale,
    BarElement,
    Title,
    Tooltip,
    Legend
);

interface Product {
    id: number;
    productName: string;
}

interface ProductLine {
    productId: number;
    productName: string;
    quantity: number;
    price: number;
}

interface Order {
    orderDate: string;
    productLines: ProductLine[];
}

interface OrdersLast6MonthsChartProps {
    orders: Order[];
    products: Product[];
    title?: string;
}

const getOrdersCountLast6Months = (
    orders: Order[],
    filterProductName?: string
) => {
    const now = new Date();
    const monthNames = [
        "Jan", "Feb", "Mar", "Apr", "May", "Jun",
        "Jul", "Aug", "Sep", "Oct", "Nov", "Dec",
    ];

    const last6MonthsKeys: string[] = [];
    for (let i = 5; i >= 0; i--) {
        const d = new Date(now.getFullYear(), now.getMonth() - i, 1);
        last6MonthsKeys.push(
            `${d.getFullYear()}-${(d.getMonth() + 1).toString().padStart(2, "0")}`
        );
    }

    const counts: Record<string, number> = {};
    last6MonthsKeys.forEach((key) => (counts[key] = 0));

    orders.forEach((order) => {
        if (
            filterProductName &&
            !order.productLines.some(
                (pl) => pl.productName.toLowerCase() === filterProductName.toLowerCase()
            )
        ) {
            return;
        }

        const date = new Date(order.orderDate);
        const key = `${date.getFullYear()}-${(date.getMonth() + 1).toString().padStart(2, "0")}`;

        if (counts.hasOwnProperty(key)) {
            counts[key]++;
        }
    });

    return last6MonthsKeys.map((key) => {
        const [year, month] = key.split("-");
        return {
            month: `${monthNames[parseInt(month, 10) - 1]} ${year}`,
            orders: counts[key],
        };
    });
};

const OrdersLast6MonthsChart: React.FC<OrdersLast6MonthsChartProps> = ({
    orders,
    products,
    title = "Aangemaakte bestellingen laatste 6 maanden",
}) => {
    const [selectedProduct, setSelectedProduct] = useState<string>("");

    const dataPoints = useMemo(
        () => getOrdersCountLast6Months(orders, selectedProduct || undefined),
        [orders, selectedProduct]
    );

    const chartData = {
        labels: dataPoints.map((d) => d.month),
        datasets: [
            {
                label: "Bestellingen",
                data: dataPoints.map((d) => d.orders),
                backgroundColor: "#4dabf7",
            },
        ],
    };

    const options = {
        indexAxis: "y" as const,
        responsive: true,
        maintainAspectRatio: false,
        scales: {
            x: {
                title: {
                    display: true,
                    text: "Hoeveelheid bestellingen",
                    color: "#fff",
                    font: { weight: "bold" as const, size: 14 },
                },
                ticks: { color: "#fff" },
                grid: {
                    display: false, // hides all grid lines on x axis
                },
            },
            y: {
                title: {
                    display: true,
                    text: "Maand",
                    color: "#fff",
                    font: { weight: "bold" as const, size: 14 },
                },
                ticks: { color: "#fff" },
                grid: {
                    display: false, // hides all grid lines on y axis
                },
            },
        },
        plugins: {
            legend: {
                labels: {
                    color: "#fff",
                    font: { weight: "bold" as const },
                },
            },
            title: {
                display: true,
                text: title,
                color: "#fff",
                font: {
                    weight: 900 as const,
                    size: 24,
                    family: "'Poppins', sans-serif",
                },
            },
            tooltip: {
                enabled: true,
            },
        },
    };

    return (
        <div
            style={{
                width: 850,
                height: 400,
                marginLeft: -20,
                marginTop: 60,
                display: "flex",
                alignItems: "flex-start",
                gap: 20,
                color: "#fff",
            }}
        >
            {/* Chart */}
            <div style={{ flex: "1 1 auto", height: "100%" }}>
                {dataPoints.length === 0 || dataPoints.every((d) => d.orders === 0) ? (
                    <p>Geen data van de laatste 6 maanden.</p>
                ) : (
                    <Bar data={chartData} options={options} />
                )}
            </div>

            {/* Filter */}
            <div
                style={{
                    minWidth: 180,
                    display: "flex",
                    flexDirection: "column",
                    justifyContent: "flex-start",
                    marginTop: 40,
                }}
            >
                <label
                    htmlFor="productFilter"
                    style={{
                        fontWeight: "bold",
                        fontSize: "12px", // Smaller font size
                        marginBottom: 6,
                        color: "#fff",    // Optional: make it match your UI
                    }}
                >
                    Filter op product:
                </label>
                <select
                    id="productFilter"
                    value={selectedProduct}
                    onChange={(e) => setSelectedProduct(e.target.value)}
                    style={{
                        padding: "4px 8px",
                        fontSize: 14,
                        borderRadius: 4,
                        border: "1px solid #ccc",
                        backgroundColor: "#222",
                        color: "#fff",
                        cursor: "pointer",
                    }}
                >
                    <option value="">Alle producten</option>
                    {products.map((p) => (
                        <option key={p.id} value={p.productName}>
                            {p.productName}
                        </option>
                    ))}
                </select>
            </div>
        </div>
    );
};

export default OrdersLast6MonthsChart;