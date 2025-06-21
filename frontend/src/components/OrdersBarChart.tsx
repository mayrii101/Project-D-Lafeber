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

ChartJS.register(
    CategoryScale,
    LinearScale,
    BarElement,
    Title,
    Tooltip,
    Legend
);

interface MonthOrderData {
    month: string;
    orders: number;
}

interface OrdersLast6MonthsChartProps {
    data: MonthOrderData[];
    title?: string;
}

const OrdersLast6MonthsChart: React.FC<OrdersLast6MonthsChartProps> = ({
    data,
    title = "Orders Sold Last 6 Months",
}) => {
    if (!data || data.length === 0) {
        return <p>No data available for the last 6 months.</p>;
    }

    const chartData = {
        labels: data.map((d) => d.month),
        datasets: [
            {
                label: "Orders",
                data: data.map((d) => d.orders),
                backgroundColor: "#00bfae", // greenish blue from earlier
            },
        ],
    };

    const options = {
        indexAxis: "y" as const, // horizontal bars
        responsive: true,
        maintainAspectRatio: false,
        scales: {
            x: {
                title: {
                    display: true,
                    text: "Amount of Orders",
                    color: "#fff",
                    font: {
                        weight: "bold" as const,
                        size: 14,
                    },
                },
                ticks: {
                    color: "#fff",
                },
            },
            y: {
                title: {
                    display: true,
                    text: "Month",
                    color: "#fff",
                    font: {
                        weight: "bold" as const,
                        size: 14,
                    },
                },
                ticks: {
                    color: "#fff",
                },
            },
        },
        plugins: {
            legend: {
                labels: {
                    color: "#fff",
                    font: {
                        weight: "bold" as const,
                    },
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
        <div style={{ width: 450, height: 300, marginLeft: 80, marginTop: 40 }}>
            <Bar data={chartData} options={options} />
        </div>
    );
};

export default OrdersLast6MonthsChart;