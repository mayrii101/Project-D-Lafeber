import React, { useRef, useEffect, useState } from "react";
import { Chart as ChartJS, ArcElement, Tooltip, Legend } from "chart.js";
import { Pie } from "react-chartjs-2";

ChartJS.register(ArcElement, Tooltip, Legend);

interface StatusData {
    name: string;
    value: number;
}

interface OrderStatusChartProps {
    data: StatusData[];
}

const gradientColors = [
    ["#3a4dbf", "#9a7eff"],
    ["#43cea2", "#185a9d"],
    ["#2ecc71", "#27ae60"],
    ["#f7971e", "#ffd200"],
    ["#ff6a00", "#ee0979"],
];

const OrderStatusChart: React.FC<OrderStatusChartProps> = ({ data }) => {
    const chartRef = useRef<any>(null);
    const [backgroundColors, setBackgroundColors] = useState<string[]>([]);

    useEffect(() => {
        if (!chartRef.current) return;

        const chart = chartRef.current;
        const ctx = chart.ctx;
        const chartArea = chart.chartArea;

        if (!chartArea) return; // chart not ready yet

        // Create gradients for each slice
        const grads = data.map((_, index) => {
            const gradient = ctx.createLinearGradient(
                chartArea.left,
                chartArea.top,
                chartArea.right,
                chartArea.bottom
            );
            const [start, end] = gradientColors[index % gradientColors.length];
            gradient.addColorStop(0, start);
            gradient.addColorStop(1, end);
            return gradient;
        });

        setBackgroundColors(grads);
    }, [data]);

    if (!data || data.length === 0) {
        return <p>Geen data beschikbaar voor statusoverzicht.</p>;
    }

    const chartData = {
        labels: data.map((d) => d.name),
        datasets: [
            {
                data: data.map((d) => d.value),
                backgroundColor: backgroundColors.length === data.length ? backgroundColors : undefined,
                borderColor: "#fff",
                borderWidth: 1,
            },
        ],
    };

    const options = {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
            legend: {
                position: "bottom" as const,
                labels: {
                    color: "#000",
                    font: {
                        weight: "bold" as const,
                    },
                },
            },
            tooltip: {
                enabled: true,
            },
        },
    };

    return (
        <div style={{ width: 350, height: 350, marginLeft: 80, marginTop: 80 }}>
            <Pie ref={chartRef} data={chartData} options={options} />
        </div>
    );
};

export default OrderStatusChart;