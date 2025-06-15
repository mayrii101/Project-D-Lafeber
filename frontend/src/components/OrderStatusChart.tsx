import React from "react";
import {
    PieChart,
    Pie,
    Cell,
    Tooltip,
    Legend,
    ResponsiveContainer,
} from "recharts";

interface StatusData {
    name: string;
    value: number;
}

interface OrderStatusChartProps {
    data: StatusData[];
}


const BLUE_GRADIENTS = [
    ["#193a80", "#2555d4"],  // Deep navy blue to bright royal blue
    ["#2e86ff", "#5aa0f0"],  // Vibrant blue to soft sky blue (kept light)
    ["#1e4a96", "#3a7ee6"],  // Darker blue to lighter bright blue
    ["#123b91", "#4a7fc8"],  // Navy to medium blue
    ["#1d3faa", "#3b63b1"]   // Bold dark blue to medium dark blue
];

const OrderStatusChart: React.FC<OrderStatusChartProps> = ({ data }) => {
    if (!data || data.length === 0) {
        return <p>Geen data beschikbaar voor statusoverzicht.</p>;
    }

    const renderLabel = (props: any) => {
        const { cx, cy, midAngle, outerRadius, index, percent } = props;
        const RADIAN = Math.PI / 180;
        const labelRadius = outerRadius + 20;

        const x = cx + labelRadius * Math.cos(-midAngle * RADIAN);
        const y = cy + labelRadius * Math.sin(-midAngle * RADIAN);

        return (
            <text
                x={x}
                y={y}
                fill="black"
                textAnchor={x > cx ? "start" : "end"}
                dominantBaseline="central"
                fontSize={14}
                fontWeight="600"
            >
                {`${data[index].name}: ${(percent * 100).toFixed(0)}%`}
            </text>
        );
    };

    return (
        <div style={{ width: "550px", height: 550 }}>
            <ResponsiveContainer width="100%" height="100%">
                <PieChart>
                    <defs>
                        {BLUE_GRADIENTS.map(([start, end], index) => (
                            <linearGradient
                                key={index}
                                id={`grad-${index}`}
                                x1="0%"
                                y1="0%"
                                x2="100%"
                                y2="100%"
                            >
                                <stop offset="0%" stopColor={start} stopOpacity={1} />
                                <stop offset="100%" stopColor={end} stopOpacity={1} />
                            </linearGradient>
                        ))}

                        <filter id="soft-drop-shadow" height="180%" width="180%" x="-40%" y="-40%" colorInterpolationFilters="sRGB">
                            <feDropShadow dx="0" dy="1" stdDeviation="10" floodColor="#00008b" floodOpacity="0.1" />
                            <feDropShadow dx="0" dy="2" stdDeviation="10" floodColor="#00008b" floodOpacity="0.07" />
                        </filter>
                    </defs>

                    <Pie
                        data={data}
                        dataKey="value"
                        nameKey="name"
                        cx="50%"
                        cy="50%"
                        outerRadius={150}
                        labelLine={false}
                        label={renderLabel}
                        stroke="#ffffff"
                        strokeWidth={0.5}
                    >
                        {data.map((entry, index) => (
                            <Cell
                                key={`cell-${index}`}
                                fill={`url(#grad-${index % BLUE_GRADIENTS.length})`}
                                filter="url(#soft-drop-shadow)"
                            />
                        ))}
                    </Pie>
                    <Tooltip />
                    <Legend />
                </PieChart>
            </ResponsiveContainer>
        </div>
    );
};

export default OrderStatusChart;