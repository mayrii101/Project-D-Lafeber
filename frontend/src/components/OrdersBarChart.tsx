import React from "react";
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

// Register chart components
ChartJS.register(
    CategoryScale,
    LinearScale,
    BarElement,
    Title,
    Tooltip,
    Legend
);

// Interfaces
interface Order {
    orderDate: string;
}

interface OrdersLast6MonthsChartProps {
    orders: Order[];
    title?: string;
}

// Utility: Generate last 6 months and order counts
const getOrdersCountLast6Months = (orders: Order[]) => {
    const now = new Date();
    const monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    const last6MonthsKeys: string[] = [];
    for (let i = 5; i >= 0; i--) {
        const d = new Date(now.getFullYear(), now.getMonth() - i, 1);
        last6MonthsKeys.push(`${d.getFullYear()}-${(d.getMonth() + 1).toString().padStart(2, "0")}`);
    }

    const counts: Record<string, number> = {};
    last6MonthsKeys.forEach(key => counts[key] = 0);

    orders.forEach(order => {
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
    title = "Aangemaakte bestellingen laatste 6 maanden",
}) => {
    const dataPoints = getOrdersCountLast6Months(orders);

    if (!dataPoints || dataPoints.length === 0) {
        return <p>Geen data van de laatste 6 maanden.</p>;
    }

    const chartData = {
        labels: dataPoints.map((d) => d.month),
        datasets: [
            {
                label: "Bestellingen",
                data: dataPoints.map((d) => d.orders),
                backgroundColor: "#00bfae",
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
            },
            y: {
                title: {
                    display: true,
                    text: "Maand",
                    color: "#fff",
                    font: { weight: "bold" as const, size: 14 },
                },
                ticks: { color: "#fff" },
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
        <div style={{ width: 750, height: 450, marginLeft: -20, marginTop: 40 }}>
            <Bar data={chartData} options={options} />
        </div>
    );
};

export default OrdersLast6MonthsChart;