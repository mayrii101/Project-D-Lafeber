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

const COLORS = ["#8884d8", "#82ca9d", "#ffc658", "#ff8042", "#d0ed57"];

const OrderStatusChart: React.FC<OrderStatusChartProps> = ({ data }) => {
    if (!data || data.length === 0) {
        return <p>Geen data beschikbaar voor statusoverzicht.</p>;
    }

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
                        outerRadius={230}//radius
                        label
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