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

const COLORS = ["#053a94", "#a1c5ff", "#3874ed", "#3d86fa", "#3ddafd"];

const OrderStatusChart: React.FC<OrderStatusChartProps> = ({ data }) => {
    if (!data || data.length === 0) {
        return <p>Geen data beschikbaar voor statusoverzicht.</p>;
    }

    // Calculate total for percentages
    const total = data.reduce((sum, entry) => sum + entry.value, 0);

    // Custom label renderer
    const renderLabel = (props: any) => {
        const { cx, cy, midAngle, outerRadius, index, percent } = props;
        const RADIAN = Math.PI / 180;

        // Push label radius 20 pixels beyond outerRadius to move it outside the pie
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
        <div style={{ width: "600px", height: 600 }}>
            <ResponsiveContainer width="100%" height="100%">
                <PieChart>
                    <Pie
                        data={data}
                        dataKey="value"
                        nameKey="name"
                        cx="50%"
                        cy="50%"
                        outerRadius={190}
                        labelLine={false}  // Disable label lines
                        label={renderLabel} // Custom label
                    >
                        {data.map((entry, index) => (
                            <Cell
                                key={`cell-${index}`}
                                fill={COLORS[index % COLORS.length]}
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